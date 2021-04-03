
using MFPS.ServerCharacters;
using MFPS.Weapons;
using System;
using UnityEngine;

class PacketsToSend
{
    #region Tcp Send Functions
    // These functions preparing data to be sent
    static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].tcp.SendData(packet);
    }
    static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(packet);
        }
    }
    static void SendTCPDataToAll(int ExceptClient, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != ExceptClient)
                Server.clients[i].tcp.SendData(packet);
        }
    }
    #endregion

    #region UDP Send Functions
    static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].udp.SendData(packet);
    }
    static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(packet);
        }
    }
    static void SendUDPDataToAll(int ExceptClient, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != ExceptClient)
                Server.clients[i].udp.SendData(packet);
        }
    }
    #endregion

    public static void Welcome(int toClient, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);

            SendTCPData(toClient, packet);
        }
    }

    public static void SpawnPlayer(int clientId, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(player.id);
            packet.Write(player.userName);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            SendTCPData(clientId, packet);
        }
    }

    #region Player Position and Rotation packets
    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);

            SendUDPDataToAll(packet);
        }
    }
    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(player.id);
            packet.Write(player.transform.rotation);

            SendUDPDataToAll(player.id, packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }
    public static void PlayerHealth(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerHealth))
        {
            packet.Write(player.id);
            packet.Write(player.health);

            SendTCPDataToAll(packet);
        }
    }
    internal static void PlayerRespawned(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRespawned))
        {
            packet.Write(player.id);

            SendTCPDataToAll(packet);
        }
    }
    public static void PlayerChangedWeapon(Player player, BaseWeapon weapon)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerChangedWeapon))
        {
            packet.Write(player.id);
            packet.Write(weapon.weaponName);
            packet.Write((int)weapon.firemode);
            packet.Write(weapon.id);

            SendTCPDataToAll(packet);
        }
    }
    public static void PlayerShoot(Player player)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerShoot))
        {
            packet.Write(player.id);

            SendUDPDataToAll(packet);
        }
    }
    #endregion

    #region Enemies Section
    public static void EnemyHealth(Enemy enemy)
    {
        using (Packet packet = new Packet((int)ServerPackets.enemyHealth))
        {
            packet.Write(enemy.GetHealth());

            SendTCPDataToAll(packet);
        }
    }
    public static void EnemyRespawned(Enemy enemy)
    {
        using(Packet packet = new Packet((int)ServerPackets.enemyRespawned))
        {
            packet.Write(enemy.GetHealth());

            SendTCPDataToAll(packet);
        }
    }
    #endregion
}