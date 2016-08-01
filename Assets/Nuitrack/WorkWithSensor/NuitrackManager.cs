using UnityEngine;
using System.Collections;

public class NuitrackManager : MonoBehaviour {

    [SerializeField]
    bool isHaveSkeletonTracker, 
         isHaveHandTracker,
         isHaveUserTracker,
         isHaveDepthSensor;

    SkeletonTracker skeletonTracker;
    HandTracker handTracker;
    UserTracker userTracker;
    DepthSensor depthSensor;

    static int currentUser = 0;
    public static int CurrentUser { get { return currentUser; } }

    T CreateAndAddToObjNuitrackPart<T>() where T : UnityEngine.Component
    {
        gameObject.AddComponent<T>();
        return this.gameObject.GetComponent<T>();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        NuitrackLoader.InitNuitrackLibraries();

        try
        {
            Debug.Log("NuitrackManager.Init() starts.");
            Init();
            Debug.Log("NuitrackManager.Init() success.");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception: InitNuitrackModulesFail. " + ex);
        }


        
    }


    void Init()
    {
        nuitrack.Nuitrack.Init();

        skeletonTracker = isHaveSkeletonTracker ? CreateAndAddToObjNuitrackPart<SkeletonTracker>() : null;
        handTracker = isHaveHandTracker ? CreateAndAddToObjNuitrackPart<HandTracker>() : null;
        userTracker = isHaveUserTracker ? CreateAndAddToObjNuitrackPart<UserTracker>() : null;
        depthSensor = isHaveDepthSensor ? CreateAndAddToObjNuitrackPart<DepthSensor>() : null;

        skeletonTracker.Init();
        handTracker.Init();
        userTracker.Init();
        depthSensor.Init();

        nuitrack.Nuitrack.Run();
    }

	void Update ()
    {
	
	}

}
