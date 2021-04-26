using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public static int dataBufferSize = 4096;
    public static Client instance;
    public string ip = "127.0.0.1";
    public int Port = 55555;

    public int id;
    public TCP tcp;
    public UDP udp;

    delegate void PacketHandler(Packet packet);
    static Dictionary<int, PacketHandler> packetHandlers;

    bool isConnected = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }
    void OnApplicationQuit() => Disconnect();
    public void ConnectToServer()
    {
        tcp = new TCP();
        udp = new UDP();
        InitializeClientData();
        isConnected = true;
        tcp.Connect();
    }

    public class TCP
    {
        int id;

        NetworkStream stream;
        Packet receivedPacket;
        public TcpClient socket;

        byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.Port, ConnectCallback, null);
        }
        void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);
            if (!socket.Connected) return;

            stream = socket.GetStream();
            receivedPacket = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error sending data to server { e }");
            }
        }
        void ReceiveCallback(IAsyncResult result) // start read data from the stream
        {
            try
            {
                int bytesLength = stream.EndRead(result);
                if (bytesLength <= 0)
                {
                    Disconnect();
                    return;
                }

                byte[] data = new byte[bytesLength];

                Array.Copy(receiveBuffer, data, bytesLength);

                receivedPacket.Reset(HandleData(data)); // Reset receivedData if all data was handled

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); // continue to read data
            }
            catch (Exception e)
            {
                Debug.Log($"Error receiving TCP data { e }");
                Disconnect();
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
                        packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
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
        void Disconnect()
        {
            instance.Disconnect();
            stream = null;
            receivedPacket = null;
            receiveBuffer = null;
            socket = null;
        }
    }
    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.Port);
        }

        /// <summary>Attempts to connect to the server via UDP.</summary>
        /// <param name="_localPort">The port number to bind the UDP socket to.</param>
        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }
        /// <summary>Sends data to the client via UDP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.id); // Insert the client's ID at the start of the packet
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }
        /// <summary>Receives incoming UDP data.</summary>
        void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }
        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_data">The recieved data.</param>
        void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                }
            });
        }
        /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
        void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int) ServerPackets.welcome, ClientHandle.Welcome },
            #region PLAYER
		    {(int) ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer},
            {(int) ServerPackets.playerPosition,ClientHandle.PlayerPosition },
            {(int) ServerPackets.playerRotation,ClientHandle.PlayerRotation },
            {(int) ServerPackets.playerDisconnected,ClientHandle.PlayerDisconnected },
            {(int) ServerPackets.playerHealth,ClientHandle.PlayerHealth },
            {(int) ServerPackets.playerRespawned,ClientHandle.PlayerRespawned },
            {(int) ServerPackets.attackerAndDmg,ClientHandle.GetAttackerAndDmg},
            {(int) ServerPackets.playerJump,ClientHandle.PlayerJumpAnimation},
            {(int) ServerPackets.headShot,ClientHandle.HeadShot},
            #endregion
            #region WEAPONS
            {(int)ServerPackets.playerChangedWeapon, ClientHandle.PlayerChangedWeapon },
            //{(int)ServerPackets.weaponRotation, ClientHandle.WeaponRotation },
            {(int)ServerPackets.weaponState, ClientHandle.ChangeWeaponState },
            {(int)ServerPackets.updateBullets, ClientHandle.UpdateBullets},
            {(int)ServerPackets.spray, ClientHandle.RotateWeaponCamera},
            #endregion
            #region Enemies
            {(int)ServerPackets.spawnEnemy, ClientHandle.SpawnEnemy},
            {(int)ServerPackets.enemyHealth, ClientHandle.EnemyHealth },
            {(int)ServerPackets.enemyRespawned, ClientHandle.EnemyRespawned },
        	#endregion
            #region Projectiles
            {(int) ServerPackets.spawnProjectile, ClientHandle.SpawnProjectile },
            {(int) ServerPackets.projectilePosition, ClientHandle.ProjectilePosition },
            {(int) ServerPackets.playMoveAnimation, ClientHandle.PlayMoveAnimation },
            {(int) ServerPackets.playerAiming, ClientHandle.PlayerAiming },

        	#endregion
            #region Effects
            {(int) ServerPackets.hitEffect, ClientHandle.CreateHitEffect},

        	#endregion
            #region Items
            {(int) ServerPackets.spawnItem, ClientHandle.SpawnItem},

	        #endregion
        };
        Debug.Log($"Packets initialized :green;".Interpolate());
    }
    void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server. :18:yellow;".Interpolate());
        }
    }
}
