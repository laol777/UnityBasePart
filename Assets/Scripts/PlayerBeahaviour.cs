using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerBeahaviour : NetworkBehaviour
{
    Vector3 offset;
    Quaternion startRotation;

    [SerializeField]
    Transform camera;


    SkeletonTracker skeletonTracker;
    ChoiceStream choiceStream;

    SensorRotation sensorRotation;


    public Vector3 tmpPos;
    public int numberUser = 1;

    [SerializeField]
    Transform head;
    [SerializeField]
    Transform rightWrist;
    [SerializeField]
    Transform rightElbow;
    [SerializeField]
    Transform rotationPivot;

    void Start()
    {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
        tmpPos = new Vector3();
        startRotation = new Quaternion();

        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        sensorRotation = GameObject.FindObjectOfType<SensorRotation>();
    }

    IEnumerator RedactPlayerPrefabName()
    {
        yield return new WaitForSeconds(0.1f);
        //if ((hasAuthority && isServer) || (!hasAuthority && !isServer))
        //{
        //    gameObject.name = "hostPlayer";
        //    offset = new Vector3(0f, 0f, 3f);
        //    startRotation = Quaternion.Euler(0f, 0f, 0f);
        //}
        //else
        //{
        //    gameObject.name = "clientPlayer";
        //    offset = new Vector3(0f, 0f, -3f);
        //    startRotation = Quaternion.Euler(0f, 0f, 0f);
        //}
        if ((hasAuthority && isServer) || (!hasAuthority && !isServer))
        {
            gameObject.name = "hostPlayer";
            offset = new Vector3(0f, 0f, 5f);
            startRotation = Quaternion.Euler(0f, 0f, 0f);
            sensorRotation.SetBaseRotation(startRotation);
            numberUser = 1;
            if (isServer)
            {
                rotationPivot.rotation *= startRotation;
                transform.position += offset;
                sensorRotation.SetBaseRotation(Quaternion.Euler(0f, 180f, 0f));
            }
        }
        else
        {
            gameObject.name = "clientPlayer";
            offset = new Vector3(0f, 0f, -5f);
            startRotation = Quaternion.Euler(0f, 180f, 0f);
            sensorRotation.SetBaseRotation(startRotation);
            numberUser = 2;
            rotationPivot.rotation *= startRotation;
            transform.position += offset;
        }

        if(!hasAuthority)
        {
            camera.gameObject.SetActive(false);
        }
    }




    void Update()
    {

        if (choiceStream != null)
        {

            tmpPos = choiceStream.GetJoint(nuitrack.JointType.Head, numberUser) * 0.001f;
            head.localPosition = tmpPos;

            tmpPos = choiceStream.GetJoint(nuitrack.JointType.RightWrist, numberUser) * 0.001f;
            rightWrist.localPosition = tmpPos;

            tmpPos = choiceStream.GetJoint(nuitrack.JointType.RightElbow, numberUser) * 0.001f;
            rightElbow.localPosition = tmpPos;

        }
    }
}
