using UnityEngine;
using System.Collections;

public class MiniCalibration : MonoBehaviour {

    ChoiceStream choiceStream;

    [SerializeField]
    float maxAngle = 30f;

    Vector3[] handDeltas;

    public bool isCalibrationComplite = false;

    SensorRotation sensorRotation;
    void Start () {
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        handDeltas = new Vector3[6];

        sensorRotation = GameObject.FindObjectOfType<SensorRotation>();

    }

    float time = 0f;

	void Update () {

        time += Time.deltaTime;

        handDeltas[0] = choiceStream.GetJoint(nuitrack.JointType.LeftWrist, 1) - choiceStream.GetJoint(nuitrack.JointType.RightWrist, 1);
        handDeltas[1] = choiceStream.GetJoint(nuitrack.JointType.LeftWrist, 1) - choiceStream.GetJoint(nuitrack.JointType.LeftElbow, 1);
        handDeltas[2] = choiceStream.GetJoint(nuitrack.JointType.LeftElbow, 1) - choiceStream.GetJoint(nuitrack.JointType.LeftShoulder, 1);
        handDeltas[3] = choiceStream.GetJoint(nuitrack.JointType.LeftShoulder, 1) - choiceStream.GetJoint(nuitrack.JointType.RightShoulder, 1);
        handDeltas[4] = choiceStream.GetJoint(nuitrack.JointType.RightShoulder, 1) - choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1);
        handDeltas[5] = choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1) - choiceStream.GetJoint(nuitrack.JointType.RightWrist, 1);

        for (int i = 1; i < 6; i++)
        {
            if (Vector3.Angle(handDeltas[0], handDeltas[i]) > maxAngle)
            {
                time = 0f;
                break;
            }
        }



        if (time > 2f)
        {
            time = 0f;
            sensorRotation.SetBaseRotation(Quaternion.Euler(0f, 180f, 0f));
            isCalibrationComplite = true;
        }


    }
}
