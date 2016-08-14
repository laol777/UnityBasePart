using UnityEngine;
using System.Collections;

public class TestNuitrack : MonoBehaviour {

    ChoiceStream cs;

    void Start()
    {
        cs = GameObject.FindObjectOfType<ChoiceStream>();
    }

    public int[] test;

    void Update () {
        test = cs.GetUserID();

    }
}
