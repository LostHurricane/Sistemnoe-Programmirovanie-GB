using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;
    private int port = 5805;
    private int hostID;
    private int reliableChannel;
    private bool isStarted = false;
    private byte error;
    Dictionary<int, string> connectionIDs = new Dictionary<int, string>();

    public void StartServer()
    {
        if (isStarted) return;

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
        hostID = NetworkTransport.AddHost(topology, port);
        isStarted = true;
    }

    public void ShutDownServer()
    {
        if (!isStarted) return;

        NetworkTransport.RemoveHost(hostID);
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.Log((NetworkError)error);
    }

    public void SendMessageToAll(string message)
    {
        foreach (var value in connectionIDs)
        {
            SendMessage(message, value.Key);
        }
    }

    private void Update()
    {
        if (!isStarted) return;
        int recHostId;
        int connectionId;
        int channelId;
        int bufferSize = 1024;
        byte[] recBuffer = new byte[bufferSize];
        int dataSize;

        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, 
            out channelId, recBuffer, bufferSize, out dataSize, out error);
        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    connectionIDs.Add(connectionId, "");
                    SendMessageToAll($"Player Id {connectionId} has connected.");
                    Debug.Log($"Player Id {connectionId} has connected.");
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    if (connectionIDs[connectionId] == "")
                    {
                        connectionIDs[connectionId] = message;
                        SendMessageToAll($"Player Id {connectionId} Set name to {connectionIDs[connectionId]}.");
                        Debug.Log($"Player Id {connectionId} Set name to {connectionIDs[connectionId]}.");
                    }
                    else
                    {
                        SendMessageToAll($"Player {connectionIDs[connectionId]}: {message}");
                        Debug.Log($"Player {connectionIDs[connectionId]}: {message}");
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    connectionIDs.Remove(connectionId);
                    SendMessageToAll($"Player {connectionIDs[connectionId]} has disconnected.");
                    Debug.Log($"Player {connectionIDs[connectionId]} has disconnected.");
                    break;
                case NetworkEventType.BroadcastEvent:
                    break;
            }
            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
            bufferSize, out dataSize, out error);
        }

    }


}
