using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BotFailEffect : MonoBehaviour {

    //[SerializeField]
    //TextMesh text;

    [SerializeField]
    GameObject[] visualLifeBot;

    int countCollision = 0;

    void OnTriggerEnter(Collider other)
    {
        countCollision++;
        //text.text = countCollision.ToString();
        //Debug.Log(countCollision);
        if (countCollision > 11)
        {
            Application.LoadLevel(Application.loadedLevel - 1);
        }
        else
        {
            visualLifeBot[11 - countCollision].SetActive(false);
        }

    }


}
