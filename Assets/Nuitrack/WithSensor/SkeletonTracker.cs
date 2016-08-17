using UnityEngine;
using System.Collections;

public class SkeletonTracker : MonoBehaviour{

    nuitrack.SkeletonTracker skeletonTracker;

    nuitrack.Skeleton currentSkeleton;
    public nuitrack.Skeleton CurrentSkeleton { get { return currentSkeleton; } }

    nuitrack.Skeleton[] skeletons;
    public nuitrack.Skeleton[] Skeletons { get { return skeletons; } }

    nuitrack.SkeletonData skeletonData;
    public nuitrack.SkeletonData SkeletonData { get { return skeletonData; } }

    SkeletonTracker()
    {
        skeletonTracker = nuitrack.SkeletonTracker.Create();
        skeletonTracker.OnSkeletonUpdateEvent += HandleOnSkeletonUpdateEvent;
        Debug.Log("___SkeletonTracker.Init() success.");
    }
    void HandleOnSkeletonUpdateEvent(nuitrack.SkeletonData _skeletonData)
    {
        skeletonData = _skeletonData;
        if (_skeletonData == null) return; //just in case

        skeletons = _skeletonData.Skeletons;

        if (NuitrackManager.currentUser != 0)
        {
            NuitrackManager.currentUser = (_skeletonData.GetSkeletonByID(NuitrackManager.currentUser) == null) ? 0 : NuitrackManager.currentUser;
        }

        if (_skeletonData.NumUsers == 0)
        {
            currentSkeleton = null;
            return;
        }

        currentSkeleton = _skeletonData.GetSkeletonByID(NuitrackManager.currentUser);
        currentSkeleton = _skeletonData.Skeletons[0];
    }
}
