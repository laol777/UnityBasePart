using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MoveBullet : NetworkBehaviour {

    public float velocity = 1f;
    public static int numberUser;
    public Vector3 vector;

    void Start()
    {
        if (PlayerTypeDecision.type == PlayerTypeDecision.PlayerType.HOST)
            numberUser = 0;
        else
            numberUser = 1;
    }
	
	void Update () {
        transform.localPosition += vector * velocity * Time.deltaTime;
        //CmdUpdatePosition(vector, velocity);

    }


    [Command(channel = 0)]
    void CmdUpdatePosition(Vector3 vector, float velocity)
    {
        transform.localPosition += vector * velocity * Time.deltaTime;
        RpcUpdatePosition(vector, velocity);
    }


    [ClientRpc(channel = 0)]
    void RpcUpdatePosition(Vector3 vector, float velocity)
    {
        transform.localPosition += vector * velocity * Time.deltaTime;
    }

}
