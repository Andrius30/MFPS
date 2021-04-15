
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
    public static void PlayMovementAnimation(Player player, float x, float z)
    {
        using (Packet packet = new Packet((int)ServerPackets.playMoveAnimation))
        {
            packet.Write(player.id);
            packet.Write(x);
            packet.Write(z);
            packet.Write((int)player.playerMove.GetPlayerState());

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
    public static void SendAttackerAndDamage(Transform attacker, float dmg, int attackedPlayerId, AttackerTypes type)
    {
        using (Packet packet = new Packet((int)ServerPackets.attackerAndDmg))
        {
            packet.Write(attackedPlayerId);
            packet.Write(dmg);
            Player pl = attacker.GetComponent<Player>();
            Enemy en = attacker.GetComponent<Enemy>();

            if (pl != null) packet.Write(pl.id); // if player
            if (en != null) packet.Write(en.id); // if enemy

            packet.Write((int)type);

            SendTCPData(attackedPlayerId, packet);
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
    public static void WeaponState(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.weaponState))
        {
            packet.Write(player.id);
            packet.Write((int)player.weaponsController.GetCurrentWeapon().GetWeaponState());

            SendUDPDataToAll(packet);
        }
    }
    public static void PlayerAimingRotation(Player player, float angle, Quaternion localRot)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerAiming))
        {
            packet.Write(player.id);
            packet.Write(angle);
            packet.Write(localRot);

            SendUDPDataToAll(player.id, packet);
        }
    }
    #endregion

    #region Enemies Section
    public static void SpawnEnemy(Enemy enemy)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            packet.Write(enemy.id);
            packet.Write(enemy.transform.position);
            packet.Write(enemy.transform.rotation);

            SendTCPDataToAll(packet);
        }
    }
    // TODO: Send enemy position and rotation
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
        using (Packet packet = new Packet((int)ServerPackets.enemyRespawned))
        {
            packet.Write(enemy.GetHealth());

            SendTCPDataToAll(packet);
        }
    }

    #endregion
    public static void SpawnProjectile(Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            packet.Write(projectile.nextProjectile);
            packet.Write(projectile.transform.position);
            packet.Write(projectile.transform.rotation);

            SendUDPDataToAll(packet);
        }
    }
    public static void ProjectilePosition(Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectilePosition))
        {
            packet.Write(projectile.nextProjectile);
            packet.Write(projectile.transform.position);
            packet.Write(projectile.transform.rotation);

            SendUDPDataToAll(packet);
        }
    }
}