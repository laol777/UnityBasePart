using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TestNetwork : NetworkBehaviour
{

	// Use this for initialization
	void Start () {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
     
    }

    IEnumerator RedactPlayerPrefabName() 
    {
        yield return new WaitForSeconds(0.1f);
        if ((hasAuthority && isServer) || (!hasAuthority && !isServer))
            gameObject.name = "hostPlayer";
        else
            gameObject.name = "clientPlayer";
    }
}
