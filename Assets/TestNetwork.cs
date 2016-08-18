﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TestNetwork : NetworkBehaviour
{



    Vector3 offset;
    Quaternion startRotation;

    [SerializeField]
    Transform camera;


    SkeletonTracker skeletonTracker;
    ChoiceStream choiceStream;
    // Use this for initialization
    void Start () {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
        tmpPos = new Vector3();
        startRotation = new Quaternion();

        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
    }

    IEnumerator RedactPlayerPrefabName() 
    {
        yield return new WaitForSeconds(0.1f);
        if ((hasAuthority && isServer) || (!hasAuthority && !isServer))
        {
            gameObject.name = "hostPlayer";
            offset = new Vector3(0f, 0f, 3f);
            startRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            gameObject.name = "clientPlayer";
            offset = new Vector3(0f, 0f, -3f);
            startRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        transform.position += offset;
        transform.rotation *= startRotation;
    }


    public Vector3 tmpPos;
    public int numberUser = 1;

    [SerializeField]
    Transform head;
    [SerializeField]
    Transform rightWrist;
    [SerializeField]
    Transform rightElbow;

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
