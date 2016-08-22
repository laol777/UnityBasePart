using UnityEngine;
using System.Collections;

public class MoveBullet : MonoBehaviour {

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
	}
}
