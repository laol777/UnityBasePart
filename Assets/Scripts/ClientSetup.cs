using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClientSetup : NetworkBehaviour 
{
    [SerializeField]GameObject 
    playerObject,
    spectatorObject;

    public override void OnStartLocalPlayer ()
    {
        Debug.Log ("OnStartLocalPlayer" + " " + netId.ToString());
        base.OnStartLocalPlayer ();
        if (isLocalPlayer)
        {
            if (PlayerTypeDecision.type == PlayerTypeDecision.PlayerType.PLAYER)
            {
                CmdSpawnPlayer();
            }
            else
            {
                CmdSpawnSpectator();
            }
        }
    }

    [Command(channel = 0)]
    void CmdSpawnPlayer()
    {
        GameObject player = (GameObject)Instantiate(playerObject, Vector3.zero, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(player, gameObject);
    }

    [Command(channel = 0)]
    void CmdSpawnSpectator()
    {
        GameObject spec = (GameObject)Instantiate(spectatorObject, Vector3.zero, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(spec, gameObject);
    }
}
