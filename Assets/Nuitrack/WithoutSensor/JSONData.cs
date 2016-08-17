using UnityEngine;
using System.Collections;

[System.Serializable]
public class JSONData{

    public int XRes, YRes;

    public int[] depth;

    public int[] userTracker;

    public int[] userID;

    //public Skeleton[] skeletons;

}


public class Skeleton
{
    public int ID;

    public nuitrack.JointType[] jointName =
    {
            nuitrack.JointType.Head,
            nuitrack.JointType.LeftAnkle,
            nuitrack.JointType.LeftCollar,
            nuitrack.JointType.LeftElbow,
            nuitrack.JointType.LeftFingertip,
            nuitrack.JointType.LeftFoot,
            nuitrack.JointType.LeftHand,
            nuitrack.JointType.LeftHip,
            nuitrack.JointType.LeftKnee,
            nuitrack.JointType.LeftShoulder,
            nuitrack.JointType.LeftWrist,
            nuitrack.JointType.Neck,
            nuitrack.JointType.None,
            nuitrack.JointType.RightAnkle,
            nuitrack.JointType.RightCollar,
            nuitrack.JointType.RightElbow,
            nuitrack.JointType.RightFingertip,
            nuitrack.JointType.RightFoot,
            nuitrack.JointType.RightHand,
            nuitrack.JointType.RightHip,
            nuitrack.JointType.RightKnee,
            nuitrack.JointType.RightShoulder,
            nuitrack.JointType.RightWrist,
            nuitrack.JointType.Torso,
            nuitrack.JointType.Waist
        };

    public Vector3[] joints;

}