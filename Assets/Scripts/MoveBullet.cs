using UnityEngine;
using System.Collections;

public class MoveBullet : MonoBehaviour {

    public float velocity = 1f;
    public Vector3 vector;
	
	void Update () {
        transform.localPosition += vector * velocity * Time.deltaTime;
	}
}
