using UnityEngine;
using System.Collections;

public class SkeletonTracker : MonoBehaviour{

    static nuitrack.SkeletonTracker skeletonTracker;

    public void Init()
    {
        skeletonTracker = nuitrack.SkeletonTracker.Create();
        skeletonTracker.OnSkeletonUpdateEvent += HandleOnSkeletonUpdateEvent;
    }

    static void HandleOnSkeletonUpdateEvent(nuitrack.SkeletonData skeletonData)
    {

    }
}
