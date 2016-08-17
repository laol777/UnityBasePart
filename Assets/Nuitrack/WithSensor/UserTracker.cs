using UnityEngine;
using System.Collections;

public class UserTracker : MonoBehaviour{

    nuitrack.UserTracker userTracker;

    public nuitrack.UserTracker GetUserTracker { get { return userTracker; } }

    nuitrack.UserFrame userFrame = null;
    public nuitrack.UserFrame UserFrame { get { return userFrame; } }

    UserTracker()
    {
        userTracker = nuitrack.UserTracker.Create();
        userTracker.OnUpdateEvent += UserUpdateEvent;
        Debug.Log("___UserTracker.Init() success.");
    }

    public int frame = 0;

    void UserUpdateEvent(nuitrack.UserFrame _userFrame)
    {
        userFrame = _userFrame;
         
        ++frame;
    }
}
