using System;
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

        GameManager.players[id].SetHealth(health);
    }
    public static void PlayerRespawned(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.players[id].Respawn();
    }
    #endregion
    #region Items
    public static void SpawnItem(Packet packet)
    {
        int itemId = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        bool picked = packet.ReadBool();

        ItemSpawner.onItemSpawned?.Invoke(itemId, pos, picked);
    }
    public static void ItemPickedUp(Packet packet)
    {
        int playerID = packet.ReadInt();
        int itemID = packet.ReadInt();
        bool picked = packet.ReadBool();

        GameManager.players[playerID].SetItemsPicked();
        Item.onItemPicked?.Invoke(itemID, picked);
    }
    public static void ItemRespawned(Packet packet)
    {
        int id = packet.ReadInt();
        bool picked = packet.ReadBool();

        Item.onItemRespawned?.Invoke(id, picked);
    }
    #endregion

    #region Projectiles
    public static void SpawnProjectile(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        int thrownByPlayer = packet.ReadInt();

        GameManager.instance.SpawnProjectile(id, position);
        GameManager.players[thrownByPlayer].DecreaseItemsCount();
    }
    public static void ProjectilePosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        if (GameManager.projectiles.TryGetValue(id, out Projectile projectile))
        {
            projectile.transform.position = position;
        }
    }
    public static void Explode(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        if (GameManager.projectiles.ContainsKey(id))
            GameManager.projectiles[id].Explode(position);
    }
    #endregion
}
