using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public static Dictionary<int, Packethandler> packetHandlers;
    public delegate void Packethandler(int fromClient, Packet packet);
    public static int MaxPlayers { get; set; }
    public static int Port { get; set; }

    static TcpListener listener;
    static UdpClient udpListener;

    public static void StartServer(int maxPlayers, int port)
    {
        MaxPlayers = maxPlayers;
        Port = port;
        InitializeServerData();

        listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        listener.BeginAcceptTcpClient(new AsyncCallback(TCPCallback), null);

        udpListener = new UdpClient(port);
        udpListener.BeginReceive(UDPReceiveCallback, null);
        Debug.Log($"Server started at port { Port }");
    }

    public static void Stop()
    {
        listener.Stop();
        udpListener.Close();
    }

    static void TCPCallback(IAsyncResult result)
    {
        TcpClient client = listener.EndAcceptTcpClient(result);
        listener.BeginAcceptTcpClient(new AsyncCallback(TCPCallback), null);

        Debug.Log($"Incoming connection from { client.Client.RemoteEndPoint }");

        ConnectNewCLient(client);
    }
    static void ConnectNewCLient(TcpClient client)
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(client);
                return;
            }
        }
        Debug.Log($"Client { client.Client.RemoteEndPoint } failed to connect! Server full!");
    }
    static void UDPReceiveCallback(IAsyncResult _result)  //  Receives incoming UDP data
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null)
                {
                    // If this is a new connection
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    // Ensures that the client is not being impersonated by another by sending a false clientID
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }
    /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
    /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
    /// <param name="_packet">The packet to send.</param>
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }
    static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, Packethandler>()
        {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                {(int)ClientPackets.playerOtherInputs, ServerHandle.PlayerOtherInputs },
                {(int)ClientPackets.setStartingWeapon, ServerHandle.SetStartingWeaponAndAllPositions },
                {(int)ClientPackets.playerAimingAnim, ServerHandle.PlayerAiming },
                {(int)ClientPackets.weaponIndex, ServerHandle.PlayerChangedWeapon},

        };
        Debug.Log("Server packets initialized");
    }
}
