using MFPS.ServerCharacters;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    static readonly int dataBufferSize = 4096;

    public int id;
    public Player player;
    public TCP tcp;
    public UDP udp;

    public Client(int clientID)
    {
        id = clientID;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;

        NetworkStream stream;
        Packet receivedPacket;
        readonly int id;
        byte[] receiveBuffer;

        public TCP(int id) => this.id = id;
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();
            receivedPacket = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            PacketsToSend.Welcome(id, "Welcome to server!"); // welcome message to just connected client
        }
        void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int bytesLength = stream.EndRead(result);
                if (bytesLength <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] data = new byte[bytesLength];
                Array.Copy(receiveBuffer, data, bytesLength);

                receivedPacket.Reset(HandleData(data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error receiving TCP data { e }");
                Server.clients[id].Disconnect();
            }

        }
        internal void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception e)
            {
                Debug.Log($"Error sending data to player { id } via tcp { e }");
            }
        }
        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_data">The recieved data.</param>
        bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedPacket.SetBytes(_data);

            if (receivedPacket.UnreadLength() >= 4)
            {
                // If client's received data contains a packet
                _packetLength = receivedPacket.ReadInt();
                if (_packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedPacket.UnreadLength())
            {
                // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                byte[] _packetBytes = receivedPacket.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                    }
                });

                _packetLength = 0; // Reset packet length
                if (receivedPacket.UnreadLength() >= 4)
                {
                    // If client's received data contains another packet
                    _packetLength = receivedPacket.ReadInt();
                    if (_packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true; // Reset receivedData instance to allow it to be reused
            }

            return false;
        }

        /// <summary>Closes and cleans up the TCP connection.</summary>
        public void Disconnect()
        {
            socket.Close();
            stream = null;
            receivedPacket = null;
            receiveBuffer = null;
            socket = null;
        }
    }
    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        /// <summary>Initializes the newly connected client's UDP-related info.</summary>
        /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }

        /// <summary>Sends data to the client via UDP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endPoint, _packet);
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_packetData">The packet containing the recieved data.</param>
        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                }
            });
        }

        /// <summary>Cleans up the UDP connection.</summary>
        public void Disconnect()
        {
            endPoint = null;
        }
    }

    /// <summary>Disconnects the client and stops all network traffic.</summary>
    void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;
        });

        tcp.Disconnect();
        udp.Disconnect();

        PacketsToSend.PlayerDisconnected(id);
    }

    /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
    /// <param name="_playerName">The username of the new player.</param>
    public void SendIntoGame(string playerName)
    {
        player = NetworkManager.instance.InstantiatePlayer();
        player.Initialize(id, playerName);
        
        foreach (Client client in Server.clients.Values) // Send all players to the new player
        {
            if (client.player != null)
            {
                if (client.id != id)
                {
                    PacketsToSend.SpawnPlayer(id, client.player);
                    if (client.player.weaponsController.GetCurrentWeapon() != null)
                        PacketsToSend.PlayerChangedWeapon(client.player, client.player.weaponsController.GetCurrentWeapon());
                }
            }
        }
        foreach (Client client in Server.clients.Values)  // Send the new player to all players (including himself)
        {
            if (client.player != null)
            {
                PacketsToSend.SpawnPlayer(client.id, player);        
            }
        }

    }
}

