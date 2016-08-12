using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.Networking;

public class SocketsDiscovery : MonoBehaviour
{
    [SerializeField]int port;
    [SerializeField]float broadcastInterval = 1f;
    [SerializeField]string broadcastDataString = "Application";
    byte[] broadcastData;
    UdpClient udpBroadcaster;
    UdpClient udpListener;
    IPEndPoint ipBroadcast;

    bool isBroadcasting = false;
    bool isListening = false;
    bool foundServer;

    public bool showGui = true;

    void OnGUI()
    {
        if (showGui)
        {
            if (!isBroadcasting & !isListening)
            {
                if (GUI.Button (new Rect(250, 0, 200, 30), "Search and join"))
                {
                    StartListening();
                }
            }
            else
            {
                if (isListening)
                {
                    if (GUI.Button (new Rect(250, 0, 200, 30), "Stop search"))
                    {
                        StopListening();
                    }
                }
            }
        }
    }

    void Start()
    {
        GameNetworkManager netMan = (GameNetworkManager)NetworkManager.singleton;
        netMan.onServerStart += StartBroadcasting;
        netMan.onServerStop += StopDiscovery;
    }

    static byte[] StringToBytes (string str)
    {
        byte[] array = new byte[str.Length * 2];
        Buffer.BlockCopy (str.ToCharArray (), 0, array, 0, array.Length);
        return array;
    }
  
    static string BytesToString (byte[] bytes)
    {
        char[] array = new char[bytes.Length / 2];
        Buffer.BlockCopy (bytes, 0, array, 0, bytes.Length);
        return new string (array);
    }

    #region broadcasting
    public void StartBroadcasting ()
    {
        broadcastData = StringToBytes (broadcastDataString + ":" + NetworkManager.singleton.networkPort.ToString());

        udpBroadcaster = new UdpClient ();
        ipBroadcast = new IPEndPoint (IPAddress.Broadcast, port);
        isBroadcasting = true;
        StartCoroutine (BroadcastServerInfo ());
    }

    IEnumerator BroadcastServerInfo ()
    {
        while (isBroadcasting)
        {
            udpBroadcaster.Send (broadcastData, broadcastData.Length, ipBroadcast);
            yield return new WaitForSeconds (broadcastInterval);
        }
    }

    public void StopBroadcasting ()
    {
        if (isBroadcasting)
        {
            isBroadcasting = false;
            udpBroadcaster.Close ();
            udpBroadcaster = null;
            broadcastData = null;
            ipBroadcast = null;
        }
    }
    #endregion

    #region listener
    public void StartListening ()
    {
        udpListener = new UdpClient (port);
        isListening = true;
        BeginListenerRecieve();
    }

    void BeginListenerRecieve ()
    {
        udpListener.BeginReceive(ListenerReciveData, new System.Object());
    }

    void ListenerReciveData (IAsyncResult ar)
    {
        IPEndPoint ip = null;
        byte[] buffer = udpListener.EndReceive (ar, ref ip);

        string recievedString = BytesToString(buffer);
        Debug.Log ("Got broadcast:" + recievedString);
        string[] items = recievedString.Split(':');

        if (items[0] == broadcastDataString)
        {
            int port = Convert.ToInt32 (items[1]);
            StopListening();
            //can add event instead of directly setting up NetworkManager and starting client
            NetworkManager.singleton.networkAddress = ip.Address.ToString();
            NetworkManager.singleton.networkPort = port;
            foundServer = true;
        }
        else
        {
            BeginListenerRecieve ();
        }
    }

    public void StopListening ()
    {
        if (isListening) udpListener.Close();
        isListening = false;
    }
    #endregion

    void Update()
    {
        if (foundServer && !NetworkManager.singleton.isNetworkActive)
        {
            foundServer = false;
            NetworkManager.singleton.StartClient();
        }
    }

    public void StopDiscovery()
    {
        if (isListening) StopListening();
        if (isBroadcasting) StopBroadcasting();
    }
}
