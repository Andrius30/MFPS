using System;
using UnityEngine;

class ServerHandle
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Debug.Log($"{ Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint } connected successfully! Total connected players count { fromClient }");
        if (fromClient != id)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient }) has assumed the wrong client ID ({ id })!");
        }
        Server.clients[fromClient].SendIntoGame(username);
    }

    internal static void PlayerMovement(int fromClient, Packet packet)
    {
        float[] inputs = new float[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadFloat();
        }
        Quaternion rotation = packet.ReadQuaternion();
        Server.clients[fromClient].player.SetInput(inputs, rotation);
    }

    internal static void PlayerOtherInputs(int fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Server.clients[fromClient].player.SetOtherInputs(inputs);
    }
    public static void PlayerShoot(int fromClient, Packet packet)
    {
        Vector3 viewDirection = packet.ReadVector3();

        Server.clients[fromClient].player.Shoot(viewDirection);
    }

    public static void ThrowProjectile(int fromClient, Packet packet)
    {
        Vector3 direction = packet.ReadVector3();

        if (Server.clients[fromClient].player.CanThrowProjectile())
        {
            Server.clients[fromClient].player.projectileSpawner.SpawnProjectile(direction);
            Server.clients[fromClient].player.DecreaseItemsByOne();
        }
    }
}
