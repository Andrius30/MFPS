using System.Net;
using UnityEngine;

public class ClientHandle
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Message from server { msg }");
        Client.instance.id = id;

        PacketsToSend.WelcomeReceived(); // response to server
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port); // Connect udp client
    }

    #region Player
    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string userName = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, userName, position, rotation);
    }
    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        if (GameManager.players.TryGetValue(id, out PlayerManager player))
        {
            player.transform.position = position;
        }
    }
    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        if (GameManager.players.TryGetValue(id, out PlayerManager player))
        {
            player.transform.rotation = rotation;
        }
    }
    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();
        MonoBehaviour.Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }
    public static void PlayerHealth(Packet packet)
    {
        int id = packet.ReadInt();
        float health = packet.ReadFloat();
        // TODO: get dmg
        // TODO: get attacker id

        GameManager.players[id].SetHealth(health);
    }
    public static void PlayerRespawned(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.players[id].Respawn();
    }
    public static void PlayerChangedWeapon(Packet packet)
    {
        int playerID = packet.ReadInt();
        string weaponname = packet.ReadString();
        int fireMode = packet.ReadInt();
        // weapon state
        int currentWeponID = packet.ReadInt();

        GameManager.players[playerID].ChangeWeapon(currentWeponID, weaponname, fireMode/* weapon state*/);
    }
    #endregion

    #region Enemies
    public static void SpawnEnemy(Packet packet)
    {
        int enemyID = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        Quaternion rot = packet.ReadQuaternion();

        GameManager.instance.SpawnEnemy(enemyID, pos, rot);
    }
    //TODO: Update enemy position and rotation
    public static void EnemyHealth(Packet packet)
    {
        float health = packet.ReadFloat();

        Enemy.onEnemyHealthChanged?.Invoke(health);
    }
    public static void EnemyRespawned(Packet packet)
    {
        float health = packet.ReadFloat();

        Enemy.onEnemyRespawned?.Invoke(health);
    }

    #endregion

    #region Effects VFX,SFX
    public static void ChangeWeaponState(Packet packet)
    {
        int id = packet.ReadInt();
        int weaponState = packet.ReadInt();

        GameManager.players[id].newWeapon.ActionsByWeaponState(weaponState);
    }

    public static void SpawnProjectile(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        Quaternion rot = packet.ReadQuaternion();

        GameManager.instance.SpawnProjectile(id, pos, rot);
    }
    public static void ProjectilePosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        Quaternion rot = packet.ReadQuaternion();

        if (GameManager.projectiles.TryGetValue(id, out Projectile projectile))
        {
            if (projectile == null) return;
            projectile.transform.position = pos;
            projectile.transform.rotation = rot;
        }
    }
    #endregion

    #region Player Animations
    internal static void PlayMoveAnimation(Packet packet)
    {
        int playerID = packet.ReadInt();
        float x = packet.ReadFloat();
        float z = packet.ReadFloat();
        int playerState = packet.ReadInt();

        PlayerManager player = GameManager.players[playerID];
        player.playerAnimations.PlayMoveAnimation(x, z);
        player.playerAnimations.PlayAnimationsDependingOnPlayerState(playerState);
    }
    internal static void PlayerAiming(Packet packet)
    {
        int playerID = packet.ReadInt();
        float angle = packet.ReadFloat();
        Quaternion localRot = packet.ReadQuaternion();

        GameManager.players[playerID].playerAnimations.PlayAimingAnimation(angle, localRot);
    }


    #endregion

}
