using System;
using UnityEngine;

public class PacketsToSend
{
    static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    #region Packets to send

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.id);
            packet.Write(UIManager.instance.userNameField.text);
            SendTCPData(packet);
        }
    }

    // ================== INPUTS =================================
    public static void SendPlayerInputs(float[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(inputs.Length);
            foreach (float input in inputs)
            {
                packet.Write(input);
            }
            packet.Write(GameManager.players[Client.instance.id].transform.rotation);

            SendUDPData(packet);
        }
    }
    public static void SendOtherInputs(bool[] otherInputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerOtherInputs))
        {
            packet.Write(otherInputs.Length);
            foreach (var input in otherInputs)
            {
                packet.Write(input);
            }
            SendTCPData(packet);
        }
    }
    // ===========================================================

    // ================= WEAPONS =================================
    public static void PlayerChangingWeapon(int index)
    {
        using (Packet packet = new Packet((int)ClientPackets.weaponIndex))
        {
            packet.Write(index);
            SendTCPData(packet);
        }
    }
    public static void InitializeWeaponsAndSetStartingWeapon(ClientWeapon[] weapons, int startingWeaponIndex)
    {
        using (Packet packet = new Packet((int)ClientPackets.setStartingWeapon))
        {
            packet.Write(startingWeaponIndex);
            foreach (var weapon in weapons)
            {
                packet.Write(weapon.model.localPosition);
                packet.Write(weapon.model.localRotation);
                packet.Write(weapon.shootPosition.localPosition);
                packet.Write(weapon.shootPosition.localRotation);
                //Debug.Log($"Send to server weapon {weapon.weaponID} position {weapon.model.localPosition} rotation " +
                //    $" {weapon.model.localRotation}:green:18;".Interpolate());
            }

            SendTCPData(packet);
        }
    }
    public static void PlayerAimingAnim(float angle, Quaternion localRot)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerAimingAnim))
        {
            packet.Write(angle);
            packet.Write(localRot);

            SendUDPData(packet);
        }
    }
    // ===========================================================
    #endregion
}
