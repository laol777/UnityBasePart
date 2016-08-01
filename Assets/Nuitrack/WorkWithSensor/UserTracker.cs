using UnityEngine;
using System.Collections;

public class UserTracker : MonoBehaviour{

    static nuitrack.UserTracker userTracker;

    static nuitrack.UserFrame userFrame = null;

    public void Init()
    {
        userTracker = nuitrack.UserTracker.Create();
        userTracker.OnUpdateEvent += UserUpdateEvent;
        Debug.Log("UserTracker.Init() success.");
    }

    static void UserUpdateEvent(nuitrack.UserFrame _userFrame)
    {
        userFrame = _userFrame;
    }
}
