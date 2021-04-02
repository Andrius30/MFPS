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
    public static void PlayerShoot(Vector3 facing)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }
    // ===========================================================
    #endregion

}
