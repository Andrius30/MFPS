using MFPS.ServerCharacters;
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

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        float[] inputs = new float[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadFloat();
        }
        Quaternion rotation = packet.ReadQuaternion();
        Server.clients[fromClient].player.SetInput(inputs, rotation);
    }
    public static void PlayerOtherInputs(int fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Server.clients[fromClient].player.SetOtherInputs(inputs);
    }
    public static void PlayerAiming(int fromClient, Packet packet)
    {
        float angle = packet.ReadFloat();
        Quaternion localRot = packet.ReadQuaternion();

        Server.clients[fromClient].player.UpdateAimingPivotRotation(angle, localRot);
    }
    #region Weapons section
    public static void PlayerChangedWeapon(int fromClient, Packet packet)
    {
        int index = packet.ReadInt();
        Server.clients[fromClient].player.weaponsController.ChangeWeapon(index);
    }
    internal static void SetStartingWeaponAndAllPositions(int fromClient, Packet packet)
    {
        int startingWeaponIndex = packet.ReadInt();
        Player player = Server.clients[fromClient].player;

        foreach (var weapon in player.weaponsController.GetAllWeapons())
        {
            // model position and rotation
            Vector3 modelPosition = packet.ReadVector3();
            Quaternion modelRotation = packet.ReadQuaternion();
            // Shoot position and rotation
            Vector3 shootPosition = packet.ReadVector3();
            Quaternion rot = packet.ReadQuaternion();
            weapon.InitializeWeapons(modelPosition, modelRotation, shootPosition, rot);
        }
        player.weaponsController.SetWeapon(player, startingWeaponIndex);
    }

    #endregion
}
