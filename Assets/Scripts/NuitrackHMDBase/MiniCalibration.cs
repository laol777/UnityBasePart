﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniCalibration : MonoBehaviour {

    ChoiceStream choiceStream;

    [SerializeField]
    float maxAngle = 30f;

    Vector3[] handDeltas;

    public bool isCalibrationComplite = false;
    public Vector3 positionCollarAfterCalibration;

    SensorRotation sensorRotation;

    [SerializeField]
    Text feedbackCalibtaion;

    PlayerBeahaviour playerBehaviuor;
    NetPlayerMotionController netPlayerMotionController;

    void Start () {
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        handDeltas = new Vector3[6];

        sensorRotation = GameObject.FindObjectOfType<SensorRotation>();
        playerBehaviuor = gameObject.GetComponent<PlayerBeahaviour>();

        netPlayerMotionController = GameObject.FindObjectOfType<NetPlayerMotionController>();
    }

    float time = 0f;
    public bool processCalibration = false;
    float delayBetween = 0f;


    Vector3 GetVector3Joint(nuitrack.Joint joint)
    {
        Vector3 returnedJoint = new Vector3();

        returnedJoint.x = joint.Real.X;
        returnedJoint.y = joint.Real.Y;
        returnedJoint.z = joint.Real.Z;

        return returnedJoint;
    }

    void Update () {

        

        handDeltas[0] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftWrist, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightWrist, 1));
        handDeltas[1] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftWrist, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftElbow, 1));
        handDeltas[2] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftElbow, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftShoulder, 1));
        handDeltas[3] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftShoulder, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightShoulder, 1));
        handDeltas[4] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightShoulder, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1));
        handDeltas[5] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightWrist, 1));

        for (int i = 1; i < 6; i++)
        {
            if ((Vector3.Angle(handDeltas[0], handDeltas[i]) > maxAngle))
            {
                time = 0f;
                feedbackCalibtaion.text = "";
                break;
            }
            else
            {
                if (delayBetween == 0f)
                {
                    time += Time.deltaTime;
                    feedbackCalibtaion.text = "Calibration " + (Mathf.Clamp(((time / 2f) * 100f), 0f, 100f)).ToString("00") + " %";
                }
                else
                {
                    feedbackCalibtaion.text = "Calibration is complete";
                }
            }
        }
       

        if (time > 2f)
        {
            time = 0f;
            StartCoroutine(DelayBetweenCalibration());
            if(sensorRotation != null)
            sensorRotation.SetBaseRotation(Quaternion.Euler(0f, 180f, 0f));
            isCalibrationComplite = true;
            playerBehaviuor.Offset = playerBehaviuor.BaseOffset + new Vector3(GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).x * 0.001f, 0f,
                                                                              GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).z * 0.001f - 2.8f);

            netPlayerMotionController.OnSuccessCalibration(Quaternion.identity);
            positionCollarAfterCalibration = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1));

            //netPlayerMotionController.OnSuccessCalibration(Quaternion.identity);
        }


    }


    IEnumerator DelayBetweenCalibration()
    {
        delayBetween = 3f;
        yield return new WaitForSeconds(3f);
        delayBetween = 0f;
    }
}
