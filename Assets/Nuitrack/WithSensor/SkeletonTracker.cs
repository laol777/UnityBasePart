using UnityEngine;
using System.Collections;

public class SkeletonTracker : MonoBehaviour{

    static nuitrack.SkeletonTracker skeletonTracker;

    static nuitrack.Skeleton currentSkeleton;
    public static nuitrack.Skeleton CurrentSkeleton { get { return currentSkeleton; } }

    static nuitrack.Skeleton[] skeletons;
    public static nuitrack.Skeleton[] Skeletons { get { return skeletons; } }

    SkeletonTracker()
    {
        skeletonTracker = nuitrack.SkeletonTracker.Create();
        skeletonTracker.OnSkeletonUpdateEvent += HandleOnSkeletonUpdateEvent;
        Debug.Log("___SkeletonTracker.Init() success.");
    }
    static void HandleOnSkeletonUpdateEvent(nuitrack.SkeletonData skeletonData)
    {
        if (skeletonData == null) return; //just in case

        skeletons = skeletonData.Skeletons;

        if (NuitrackManager.currentUser != 0)
        {
            NuitrackManager.currentUser = (skeletonData.GetSkeletonByID(NuitrackManager.currentUser) == null) ? 0 : NuitrackManager.currentUser;
        }

        if (skeletonData.NumUsers == 0)
        {
            currentSkeleton = null;
            return;
        }

        currentSkeleton = skeletonData.GetSkeletonByID(NuitrackManager.currentUser);
        currentSkeleton = skeletonData.Skeletons[0];
    }
}
