using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TestNetwork : NetworkBehaviour
{

    Vector3 tmpPos;
    // Use this for initialization
    void Start () {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
        tmpPos = new Vector3();
    }

    IEnumerator RedactPlayerPrefabName() 
    {
        yield return new WaitForSeconds(0.1f);
        if ((hasAuthority && isServer) || (!hasAuthority && !isServer))
            gameObject.name = "hostPlayer";
        else
            gameObject.name = "clientPlayer";
    }



    void Update()
    {
        if (SkeletonTracker.CurrentSkeleton != null)
        {
            tmpPos.x = SkeletonTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Head).Real.X * 0.001f;
            tmpPos.y = SkeletonTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Head).Real.Y * 0.001f;
            tmpPos.z = SkeletonTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Head).Real.Z * 0.001f + 2.5f;
            Debug.Log(tmpPos);
            transform.position = tmpPos;
        }
    }
}
