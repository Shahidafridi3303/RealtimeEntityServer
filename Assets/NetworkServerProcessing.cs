using System.Collections.Generic;
using UnityEngine;

static public class NetworkServerProcessing
{
    static NetworkServer networkServer;
    static GameLogic gameLogic;

    // List to track active balloons
    static List<BalloonData> activeBalloons = new List<BalloonData>();

    #region Balloon Data Management

    public class BalloonData
    {
        public int ID;
        public float xPercent;
        public float yPercent;

        public BalloonData(int id, float x, float y)
        {
            ID = id;
            xPercent = x;
            yPercent = y;
        }
    }

    public static void SpawnBalloon()
    {
        int balloonID = Random.Range(1, 1000000);
        float xPercent = Random.Range(0f, 1f);
        float yPercent = Random.Range(0f, 1f);

        activeBalloons.Add(new BalloonData(balloonID, xPercent, yPercent));

        string msg = $"{ServerToClientSignifiers.SpawnBalloon},{balloonID},{xPercent},{yPercent}";
        Debug.Log($"Server: Spawning balloon ID {balloonID} at ({xPercent}, {yPercent})");

        foreach (var clientID in networkServer.GetAllConnectedClientIDs())
        {
            Debug.Log($"Server: Sending balloon spawn to client ID {clientID}");
            SendMessageToClient(msg, clientID);
        }
    }


    public static void HandleBalloonPopped(int balloonID)
    {
        activeBalloons.RemoveAll(b => b.ID == balloonID);

        string msg = $"{ServerToClientSignifiers.RemoveBalloon},{balloonID}";
        foreach (var clientID in networkServer.GetAllConnectedClientIDs())
        {
            SendMessageToClient(msg, clientID);
        }
    }

    public static void SendUnpoppedBalloons(int clientID)
    {
        string msg = $"{ServerToClientSignifiers.SendUnpoppedBalloons}";
        foreach (var balloon in activeBalloons)
        {
            msg += $",{balloon.ID},{balloon.xPercent},{balloon.yPercent}";
        }
        SendMessageToClient(msg, clientID);
    }

    #endregion

    #region Event Handling

    public static void ReceivedMessageFromClient(string msg, int clientConnectionID)
    {
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.BalloonPopped)
        {
            int balloonID = int.Parse(csv[1]);
            HandleBalloonPopped(balloonID);
        }
    }

    public static void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log($"Client connected: ID {clientConnectionID}");
        SendUnpoppedBalloons(clientConnectionID);
    }

    public static void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log($"Client disconnected: ID {clientConnectionID}");
    }

    #endregion

    #region Setup

    public static void SetNetworkServer(NetworkServer server) => networkServer = server;
    public static void SetGameLogic(GameLogic logic) => gameLogic = logic;

    public static void SendMessageToClient(string msg, int clientConnectionID)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID);
    }

    public static NetworkServer GetNetworkServer() => networkServer;

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int BalloonPopped = 1;
    public const int RequestUnpoppedBalloons = 2;
}

static public class ServerToClientSignifiers
{
    public const int SpawnBalloon = 1;
    public const int RemoveBalloon = 2;
    public const int SendUnpoppedBalloons = 3;
}
#endregion
