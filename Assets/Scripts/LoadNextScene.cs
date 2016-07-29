using UnityEngine;
using System.Collections;

public class LoadNextScene : MonoBehaviour {
    
	void Start () {
        Application.LoadLevel(Application.loadedLevel + 1);
	}
}
