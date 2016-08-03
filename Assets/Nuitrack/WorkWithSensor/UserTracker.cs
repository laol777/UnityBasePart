using UnityEngine;
using System.Collections;

public class UserTracker : MonoBehaviour{

    static nuitrack.UserTracker userTracker;

    static nuitrack.UserFrame userFrame = null;
    public nuitrack.UserFrame UserFrame { get { return userFrame; } }

    UserTracker()
    {
        userTracker = nuitrack.UserTracker.Create();
        userTracker.OnUpdateEvent += UserUpdateEvent;
        Debug.Log("___UserTracker.Init() success.");
    }

    public static int frame = 0;

    static void UserUpdateEvent(nuitrack.UserFrame _userFrame)
    {
        userFrame = _userFrame;
        ++frame;
    }
}
