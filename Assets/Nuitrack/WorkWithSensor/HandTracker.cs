using UnityEngine;
using System.Collections;

public class HandTracker : MonoBehaviour{

    static nuitrack.HandTracker handTracker;

    public void Init()
    {
        handTracker = nuitrack.HandTracker.Create();
        handTracker.OnUpdateEvent += HandleOnHandsUpdateEvent;
    }

    static void HandleOnHandsUpdateEvent(nuitrack.HandTrackerData handTrackerData)
    {

    }
}
