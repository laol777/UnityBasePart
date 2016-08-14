﻿using UnityEngine;
using System.Collections;

public class UserTracker : MonoBehaviour{

    static nuitrack.UserTracker userTracker;

    public static nuitrack.UserTracker GetUserTracker { get { return userTracker; } }

    static nuitrack.UserFrame userFrame = null;
    public static nuitrack.UserFrame UserFrame { get { return userFrame; } }

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
        //try
        //{
        //    Debug.Log(_userFrame.Users[0].ID.ToString() + " " + _userFrame.Users[1].ID.ToString());
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.Log(_userFrame.Users[0].ID);
        //}
         
        ++frame;
    }
}
