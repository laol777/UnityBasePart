using UnityEngine;
using System.Collections;

public class TestSound : MonoBehaviour {

    AudioSource soundFailing;
    public bool isStart;

    void Start () {
        soundFailing = gameObject.GetComponent<AudioSource>();
	}
	
	void Update () {
        if (isStart)
        {
            isStart = false;
            soundFailing.Play();
        }
	}
}
