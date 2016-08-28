using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BotFailEffect : MonoBehaviour {

    [SerializeField]
    Text text;

    int countCollision;

    void OnTriggerEnter(Collider other)
    {
        countCollision++;
        text.text = countCollision.ToString();
    }


}
