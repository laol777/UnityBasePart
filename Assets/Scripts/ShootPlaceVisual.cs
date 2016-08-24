using UnityEngine;
using System.Collections;

public class ShootPlaceVisual : MonoBehaviour {

    Vector3 scaleVisual;

    Vector3 delta;

    void Start()
    {
        scaleVisual = new Vector3(0.5f, 0.5f, 0.5f);
        delta = new Vector3(0.4f, 0.4f, 0.4f);
    }

    void Update () {
        scaleVisual -= delta * Time.deltaTime;
        gameObject.transform.localScale = scaleVisual;
	}
}
