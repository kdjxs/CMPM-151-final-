using extOSC;
using extOSC.UI;
using UnityEngine;

public class OSCController : MonoBehaviour
{
    [Header("extOSC Components")]
    public OSCReceiver receiver;  // Port: 8001 � receives FROM Pd
    public OSCTransmitter sender;      // Host: 127.0.0.1, Port: 8000 � sends TO Pd

    public PlayerController playerController;
    public coinManager coinMgr;

    void Start()
    {
        // Pd sends to /PD message (from oscformat in oscSend subpatch)
        // Change "PD message" in the Pd patch to something cleaner, or bind to it as-is:
        receiver.Bind("/PD message", OnMessageFromPd);
    }

    private void OnMessageFromPd(OSCMessage message)
    {
        Debug.Log("Received from Pd: " + message);
    }

    // --- Sending to Pd ---
    // Pd's oscReceive routes on /unity first, then trigger/colwall/move etc.

    public void SendCoinPickup()
    {
        var msg = new OSCMessage("/unity/trigger");
        msg.AddValue(OSCValue.Float(1f));
        sender.Send(msg);
    }

    public void SendWallCollision()
    {
        var msg = new OSCMessage("/unity/colwall");
        msg.AddValue(OSCValue.Float(1f));
        sender.Send(msg);
    }

    public void SendPlayerMoving(float intensity)
    {
        var msg = new OSCMessage("/unity/move");
        msg.AddValue(OSCValue.Float(intensity)); 
        sender.Send(msg);
    }

    public void SendMaterial(int materialId)
    {
        var msg = new OSCMessage("/unity/material");
        msg.AddValue(OSCValue.Int(materialId));
        sender.Send(msg);
    }

    public void SendTempo(float bpm)
    {
        var msg = new OSCMessage("/unity/tempo");
        msg.AddValue(OSCValue.Float(bpm));
        sender.Send(msg);
    }
    public void SendMusicState(int state)
    {
        OSCMessage message = new OSCMessage("/unity/musicstate");
        message.AddValue(OSCValue.Int(state));
        sender.Send(message);
    }

    public void SendDeath()
    {
        var msg = new OSCMessage("/unity/death");
        msg.AddValue(OSCValue.Float(1f));
        sender.Send(msg);
    }
}