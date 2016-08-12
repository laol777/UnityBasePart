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
            //need to tell server that we lost user
            //events should work fine here
            //if ((NuitrackManager.CurrentUser == 0) && (onUserLoss != null)) onUserLoss();
        }

        if (skeletonData.NumUsers == 0)
        {
            currentSkeleton = null;
            return;
        }

        if (NuitrackManager.currentUser == 0)
        {
            //how do we get id in the case of networking?
            // we'll let TPoseCalibration script handle it
            //currentUser = skeletonData.Skeletons[0].ID;
        }
        //NuitrackManager.currentUser = skeletonData.Skeletons[0].ID; //del it!!!! after debug
        if(skeletonData.NumUsers != 0)
            Debug.Log(skeletonData.NumUsers.ToString() + " " + skeletonData.Skeletons[0].ID);
        currentSkeleton = skeletonData.GetSkeletonByID(NuitrackManager.currentUser);
    }
}
