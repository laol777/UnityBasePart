using UnityEngine;
using System.Collections;

public class NuitrackManager : MonoBehaviour {

    [SerializeField]
    bool isHaveSkeletonTracker, 
         isHaveHandTracker,
         isHaveUserTracker,
         isHaveDepthSensor;

	void Awake()
    {
        if (isHaveSkeletonTracker) gameObject.AddComponent<SkeletonTracker>();
        if (isHaveHandTracker) gameObject.AddComponent<HandTracker>();
        if (isHaveUserTracker) gameObject.AddComponent<UserTracker>();
        if (isHaveDepthSensor) gameObject.AddComponent<DepthSensor>();
    }
	

	void Update ()
    {
	
	}
}
