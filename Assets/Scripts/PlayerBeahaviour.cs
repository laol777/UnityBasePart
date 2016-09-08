using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerBeahaviour : MonoBehaviour
{
    Vector3 baseOffset;
    public Vector3 BaseOffset { get { return baseOffset; } }
    Vector3 offset;
    public Vector3 Offset { set { offset = value; } get { return offset; } }

    ChoiceStream choiceStream;
    SensorRotation sensorRotation;
    //[Range(1, 2)]public int numberUser = 1;

    [SerializeField]Transform head;


    MiniCalibration miniCalibraion;

    [SerializeField]AudioSource soundFailing;

    void Start()
    {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame

        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        sensorRotation = GameObject.FindObjectOfType<SensorRotation>();
        miniCalibraion = gameObject.GetComponent<MiniCalibration>();
    }





    IEnumerator RedactPlayerPrefabName()
    {
        yield return new WaitForSeconds(0.1f);
        baseOffset = new Vector3(0f, 0f, -5f);
        offset = baseOffset;

        transform.position += offset;
        if (sensorRotation != null)
            sensorRotation.SetBaseRotation(Quaternion.Euler(0f, 180f, 0f));
    }

    public bool isStartDrop;
    float timeDrop;

    Vector3 GetVector3Joint(nuitrack.Joint joint)
    {
        Vector3 returnedJoint = new Vector3();

        returnedJoint.x = joint.Real.X;
        returnedJoint.y = joint.Real.Y;
        returnedJoint.z = joint.Real.Z;

        return returnedJoint;
    }

    bool isProcessDrop;

    void Update()
    {
        if (choiceStream != null)
        {
            head.localPosition = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.Head, 1)) * 0.001f;
        }

        if (miniCalibraion.isCalibrationComplite
           && (Mathf.Abs(miniCalibraion.positionCollarAfterCalibration.x - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).x) > 500f
           || Mathf.Abs(miniCalibraion.positionCollarAfterCalibration.z - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).z) > 500f)
           )
        {
            isStartDrop = true;

        }

        if (isStartDrop)
        {
            if (!isProcessDrop)
            {
                StartCoroutine(ProcessDrop());
            }
        }

        if (isProcessDrop)
        {
            offset.y -= 9.8f * Time.deltaTime;
        }

        transform.position = offset; //change after calibration
    }

    IEnumerator ProcessDrop()
    {
        isProcessDrop = true;
        soundFailing.Play();
        yield return new WaitForSeconds(3);
        Application.LoadLevel(Application.loadedLevel - 1);
    }

}
