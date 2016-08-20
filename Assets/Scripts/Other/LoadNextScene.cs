using UnityEngine;
using System.Collections;

public class LoadNextScene : MonoBehaviour {

    [SerializeField]
    float delayBeforeStartNextScene = 0f;

	void Start () {
        StartCoroutine(LoadNextSceneWithDelay());
	}

    IEnumerator LoadNextSceneWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeStartNextScene);
        Application.LoadLevel(Application.loadedLevel + 1);
    }
}
