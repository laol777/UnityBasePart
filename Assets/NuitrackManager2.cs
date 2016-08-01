using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NuitrackManager2 : MonoBehaviour 
{

    static nuitrack.HandTracker handTracker;
	static nuitrack.SkeletonTracker skeletonTracker;

    static int currentUser = 0;
	public static int CurrentUser{get {return currentUser;}}

	static nuitrack.Skeleton currentSkeleton;
	public static nuitrack.Skeleton CurrentSkeleton {get {return currentSkeleton;}}

	static nuitrack.Skeleton[] skeletons;
	public static nuitrack.Skeleton[] Skeletons {get {return skeletons;}}

	static nuitrack.UserHands currentHands;
	public static nuitrack.UserHands СurrentHands {get {return currentHands;}}

	static nuitrack.UserHands[] hands;
	public static nuitrack.UserHands[] Hands {get {return hands;}}

	public delegate void UserLost();
    
	public static event UserLost onUserLoss;

	static NuitrackManager2 instance;


    public static NuitrackManager2 Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = FindObjectOfType<NuitrackManager2>();
                if (instance == null)
                {
                    GameObject container = new GameObject();
                    container.name = "NuitrackManager";
                    instance = container.AddComponent<NuitrackManager2>();
                }
			
                DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	public void OverrideCurrentUser(int newUserId)
	{
		currentSkeleton = null;
		skeletons = null;
		hands = null;
		currentHands = null;
		currentUser = newUserId;
	}

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		NuitrackLoader.InitNuitrackLibraries();
        
	}

  public static void StartModules()
  {

        Debug.Log ("NuitrackManager.Start()");
        //try
        {
            nuitrack.Nuitrack.Init();

            skeletonTracker = nuitrack.SkeletonTracker.Create();
            skeletonTracker.OnSkeletonUpdateEvent += HandleOnSkeletonUpdateEvent;
            //skeletonTracker.SetAutoTracking(true);


            handTracker = nuitrack.HandTracker.Create();
            handTracker.OnUpdateEvent += HandleOnHandsUpdateEvent;


            nuitrack.Nuitrack.Run();
        }
        //catch{ }
        
    }
   
    static void HandleOnHandsUpdateEvent (nuitrack.HandTrackerData handTrackerData)
    {
        if (handTrackerData == null) return;
    
        hands = handTrackerData.UsersHands;
    
        if (currentUser != 0)
        {
            currentHands = handTrackerData.GetUserHandsByID(currentUser);
        }
        else
        {
            currentHands = null;
        }
    }

	static void HandleOnSkeletonUpdateEvent (nuitrack.SkeletonData skeletonData)
	{
		if (skeletonData == null) return; //just in case

		skeletons = skeletonData.Skeletons;

		if (currentUser != 0)
		{
			currentUser = (skeletonData.GetSkeletonByID(currentUser) == null) ? 0 : currentUser;
			//need to tell server that we lost user
			//events should work fine here
			if ((currentUser == 0) && (onUserLoss != null)) onUserLoss();
		}

		if (skeletonData.NumUsers == 0) 
		{
			currentSkeleton = null;
			return;
		}

		if (currentUser == 0)
		{
			//how do we get id in the case of networking?
			// we'll let TPoseCalibration script handle it
			//currentUser = skeletonData.Skeletons[0].ID;
		}
        //currentUser = skeletonData.Skeletons[0].ID; //del it!!!! after debug
        currentSkeleton = skeletonData.GetSkeletonByID(currentUser);
	}

	void Update()
	{

        try
        {
		      nuitrack.Nuitrack.Update();
        }
        catch
        {
        }
	}

	static public void CloseUserGen()
	{
        if (skeletonTracker != null)
        {
  		    skeletonTracker.OnSkeletonUpdateEvent -= HandleOnSkeletonUpdateEvent;
  		    handTracker.OnUpdateEvent -= HandleOnHandsUpdateEvent;
  		    nuitrack.Nuitrack.Release();
        }
	}

	void OnDestroy()
	{
		CloseUserGen();
	}

}