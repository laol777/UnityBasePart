using UnityEngine;
using System.Collections;

public class MoveBullet : MonoBehaviour {

    public float velocity = 1f;
    public Vector3 vector;
	
	void Update () {
        transform.position += vector * velocity * Time.deltaTime;
	}
}
