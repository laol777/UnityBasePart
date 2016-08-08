using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameNetworkManager : NetworkManager 
{
    public delegate void OnPlayerRemovedHandle(NetworkInstanceId _netId);
    public delegate void OnServerStopHandle();
    public delegate void OnServerStartHandle();

    public event OnPlayerRemovedHandle onPlayerDisconnect;
    public event OnServerStopHandle onServerStop;
    public event OnServerStartHandle onServerStart;

    public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer (conn, playerControllerId);
    }

    public override void OnServerRemovePlayer (NetworkConnection conn, PlayerController player)
    {
        if (onPlayerDisconnect != null)
        {
            try
            {
                Debug.Log("OnPlayerRemoved event");
                onPlayerDisconnect(player.gameObject.GetComponent<NetworkIdentity>().netId);
            }
            catch
            {
            }
        }
        base.OnServerRemovePlayer (conn, player);
    }

    public override void OnServerDisconnect (NetworkConnection conn)
    {
        if (onPlayerDisconnect != null)
        {
            foreach (PlayerController player in conn.playerControllers)
            {
                Debug.Log ("OnServerDisconnect event");
                onPlayerDisconnect(player.gameObject.GetComponent<NetworkIdentity>().netId);
            }
        }
        base.OnServerDisconnect (conn);
    }

    public override void OnStopServer ()
    {

        //TODO: debug info
        /*
        System.Delegate[] invokeList = onServerStop.GetInvocationList();
        string methodNames = "";
        for (int i = 0; i < invokeList.Length; i++)
        {
        methodNames += invokeList[i].Method.Name + System.Environment.NewLine;
        }
        Debug.Log ("onStopServer subscribed metods (" + invokeList.Length.ToString() + "): " + methodNames);
        */
        ///

        if (onServerStop != null) onServerStop();
        base.OnStopServer ();
    }

    public override void OnStartServer ()
    {
        if (onServerStart != null) onServerStart();
        base.OnStartServer ();
        //NetworkServer.RegisterHandler(NetworkRigidbody.msgId, new NetworkMessageDelegate(NetworkRigidbody.HandleRigidbody));
    }

    public override void OnClientSceneChanged (NetworkConnection conn)
    {
        //TODO:
        base.OnClientSceneChanged (conn);
    }
}
