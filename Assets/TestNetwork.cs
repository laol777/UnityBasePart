using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TestNetwork : NetworkBehaviour
{

    Vector3 tmpPos;

    Vector3 offset;
    Quaternion startRotation;

    [SerializeField]
    Transform camera;
    // Use this for initialization
    void Start () {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
        tmpPos = new Vector3();
        startRotation = new Quaternion();
    }

    IEnumerator RedactPlayerPrefabName() 
    {
        yield return new WaitForSeconds(0.1f);
        if ((hasAuthority && isServer) || (!hasAuthority && !isServer))
        {
            gameObject.name = "hostPlayer";
            offset = new Vector3(0f, 0f, -3f);
            startRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            gameObject.name = "clientPlayer";
            offset = new Vector3(0f, 0f, 3f);
            startRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        transform.position += offset;
        transform.rotation *= startRotation;
    }



    void Update()
    {
        
        if (SkeletonTracker.CurrentSkeleton != null)
        {
            tmpPos.x = SkeletonTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Head).Real.X * 0.001f;
            tmpPos.y = SkeletonTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Head).Real.Y * 0.001f;
            tmpPos.z = SkeletonTracker.CurrentSkeleton.GetJoint(nuitrack.JointType.Head).Real.Z * 0.001f;

            
            camera.localPosition = -tmpPos;
        }
    }
}
