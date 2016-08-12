using UnityEngine;
using System.Collections;

public class TestNuitrack : MonoBehaviour {


    void Update () {
        if (UserTracker.UserFrame != null)
            Debug.Log(UserTracker.UserFrame.NumUsers);

    }
}
