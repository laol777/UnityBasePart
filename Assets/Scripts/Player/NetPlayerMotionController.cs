using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NetPlayerMotionController : MonoBehaviour
{

    ChoiceStream choiceStream;

    [SerializeField]
    Transform
        baseTransform,

        torso,
        head,
        leftShoulder,
        rightShoulder,
        leftElbow,
        rightElbow,
        leftWrist,
        rightWrist,
        leftHip,
        leftKnee,
        rightHip,
        rightKnee;

    Dictionary<nuitrack.JointType, Transform> joints;
    Dictionary<nuitrack.JointType, Quaternion> targetOrientations;
    nuitrack.JointType[] jointsOrder;
    SensorRotation sensorRotation;

    Vector3 offsetPosition = Vector3.zero;
    Quaternion offsetOrientation = Quaternion.Euler(0f, 0f, 0f);

    [SerializeField]
    float linePosition = 0f;
    [SerializeField]
    float maxOffset = 5f;

    [SerializeField]
    float angularLerpCoeff;

    [SerializeField]
    float linearLerpCoeff;

    [SerializeField]
    AnimationCurve angleToVelocity;

    Vector3 targetBasePosition;
    Quaternion targetBaseOrientation;

    public Vector3 GetJointPosition(nuitrack.JointType jointType)
    {
        if (joints.ContainsKey(jointType)) return joints[jointType].position;
        else return Vector3.zero;
    }

    public Transform GetJointTransform(nuitrack.JointType jointType)
    {
        if (joints.ContainsKey(jointType)) return joints[jointType];
        else return null;
    }

    Quaternion baseRotationHead;
    void Start()
    {
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        //offsetPosition = new Vector3(0f, 0f, 5f);

        sensorRotation = FindObjectOfType<SensorRotation>();
        baseRotationHead = Quaternion.Euler(0f, 0f, 0f);
        offsetPosition = new Vector3(0f, 0f, -5f);

        //////////////////
        #region joint define
        jointsOrder = new nuitrack.JointType[12]; //needed for ordered rotation change so children will rotate after parents (can't rely on foreach over Dictionary)
        jointsOrder[0] = nuitrack.JointType.Torso;
        jointsOrder[1] = nuitrack.JointType.Head;
        jointsOrder[2] = nuitrack.JointType.LeftShoulder;
        jointsOrder[3] = nuitrack.JointType.RightShoulder; 
        jointsOrder[4] = nuitrack.JointType.LeftElbow;
        jointsOrder[5] = nuitrack.JointType.RightElbow;
        jointsOrder[6] = nuitrack.JointType.LeftWrist;
        jointsOrder[7] = nuitrack.JointType.RightWrist;
        jointsOrder[8] = nuitrack.JointType.LeftHip;
        jointsOrder[9] = nuitrack.JointType.LeftKnee;
        jointsOrder[10] = nuitrack.JointType.RightHip;
        jointsOrder[11] = nuitrack.JointType.RightKnee;



        joints = new Dictionary<nuitrack.JointType, Transform>();

        joints.Add(nuitrack.JointType.Torso, torso);
        joints.Add(nuitrack.JointType.Head, head);
        joints.Add(nuitrack.JointType.LeftShoulder, leftShoulder);
        joints.Add(nuitrack.JointType.RightShoulder, rightShoulder);
        joints.Add(nuitrack.JointType.LeftElbow, leftElbow);
        joints.Add(nuitrack.JointType.RightElbow, rightElbow);
        joints.Add(nuitrack.JointType.LeftWrist, leftWrist);
        joints.Add(nuitrack.JointType.RightWrist, rightWrist);
        joints.Add(nuitrack.JointType.LeftHip, leftHip);
        joints.Add(nuitrack.JointType.LeftKnee, leftKnee);
        joints.Add(nuitrack.JointType.RightHip, rightHip);
        joints.Add(nuitrack.JointType.RightKnee, rightKnee);

        targetOrientations = new Dictionary<nuitrack.JointType, Quaternion>();
        //leftHip,
        //leftKnee,
        //rightHip,
        //rightKnee;

        targetOrientations.Add(nuitrack.JointType.Torso, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.Head, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.LeftShoulder, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.RightShoulder, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.LeftElbow, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.RightElbow, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.LeftWrist, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.RightWrist, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.LeftHip, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.LeftKnee, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.RightHip, Quaternion.identity);
        targetOrientations.Add(nuitrack.JointType.RightKnee, Quaternion.identity);
        #endregion
        //////////////////

    }


    void Update()
    {
        UpdateTargetPositions();
        Quaternion[] jointRotations = GetJointRotationsArray();

        UpdatePlayerPositions();

    }

    Quaternion[] GetJointRotationsArray()
    {
        Quaternion[] jointRotations = new Quaternion[jointsOrder.Length];
        for (int i = 0; i < jointsOrder.Length; i++)
        {
            jointRotations[i] = targetOrientations[jointsOrder[i]];
        }
        return jointRotations;
    }
    public Vector3 offs;


    Vector3 tmpF;

    Vector3 GetVector3Joint(nuitrack.Joint joint)
    {
        Vector3 returnedJoint = new Vector3();

        returnedJoint.x = joint.Real.X;
        returnedJoint.y = joint.Real.Y;
        returnedJoint.z = joint.Real.Z;

        return returnedJoint;
    }

    public void OnSuccessCalibration(Quaternion a)
    {
        offsetPosition = new Vector3(0f, 0f, -5f) + new Vector3(GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).x * 0.001f, 0f,
                                                                              GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).z * 0.001f - 2.8f);
    }

    void UpdatePlayerPositions()
    {



        for (int i = 0; i < jointsOrder.Length; i++)
        {
            if ((jointsOrder[i] == nuitrack.JointType.LeftWrist) || (jointsOrder[i] == nuitrack.JointType.RightWrist)) continue; //those should not rotate
            joints[jointsOrder[i]].rotation = Quaternion.Slerp(joints[jointsOrder[i]].rotation, targetOrientations[jointsOrder[i]], 1f);

        }

    }
    Vector3 offset;
    void UpdateTargetPositions()
    {

        baseTransform.position = new Vector3(-GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).x * 0.001f,
                                                -0f,
                                                -5f -GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.LeftCollar, 1)).z * 0.001f);
  


        if (choiceStream != null && choiceStream.GetArrayIDSegmentation() != null && choiceStream.GetArrayIDSegmentation().Length != 0)
        {
            offset = Vector3.left * GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.Torso, 1)).x * 0.001f
                                + Vector3.back * GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.Torso, 1)).z * 0.001f;
            targetBasePosition = offsetPosition + offset;
           
        }
        targetBaseOrientation = offsetOrientation;// * TPoseCalibration.GetSensorOffset;
        baseTransform.position = targetBasePosition;

        if (choiceStream != null && choiceStream.GetArrayIDSegmentation() != null && choiceStream.GetArrayIDSegmentation().Length != 0)
        {
            for (int i = 0; i < jointsOrder.Length; i++)
            {
                nuitrack.JointType jointType = jointsOrder[i];
                if ((jointType == nuitrack.JointType.Head) || (jointType == nuitrack.JointType.LeftWrist) || (jointType == nuitrack.JointType.RightWrist))
                    continue;
                nuitrack.Joint joint = choiceStream.GetJoint(jointType, 1);
                if (joint.Confidence > 0.5f)
                {
                    //signs changed below == 180 degrees rotation around Y axis (we need joint rotations from human perspective, not from sensor's)
                    Vector3 jointUp = new Vector3(-joint.Orient.Matrix[1], joint.Orient.Matrix[4], -joint.Orient.Matrix[7]);
                    Vector3 jointForward = new Vector3(joint.Orient.Matrix[2], -joint.Orient.Matrix[5], joint.Orient.Matrix[8]);
                    targetOrientations[jointType] = offsetOrientation * /* CalibratedInfo.SensorOrientation **/ Quaternion.LookRotation(jointForward, jointUp);
                }
            }
        }
        else
        {
            for (int i = 0; i < jointsOrder.Length; i++)
            {
                nuitrack.JointType jointType = jointsOrder[i];
                if ((jointType == nuitrack.JointType.Head) || (jointType == nuitrack.JointType.LeftWrist) || (jointType == nuitrack.JointType.RightWrist))
                    continue;
                targetOrientations[jointType] = offsetOrientation * /* CalibratedInfo.SensorOrientation **/ Quaternion.identity;
            }
        }
        if (sensorRotation != null)
        {
            targetOrientations[nuitrack.JointType.Head] = offsetOrientation * sensorRotation.Rotation * baseRotationHead;//head is special :)
        }
        else
        {
            targetOrientations[nuitrack.JointType.Head] = offsetOrientation * baseRotationHead;//head is special :)
        }

    }


}
