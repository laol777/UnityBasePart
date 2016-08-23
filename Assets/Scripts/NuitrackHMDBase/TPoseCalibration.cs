using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TPoseCalibration : MonoBehaviour 
{
    ChoiceStream choiceStream;
    SkeletonTracker skeletonTracker;
    PlayerBeahaviour playerBehaviour;

	public delegate void OnStartHandler();
	public delegate void OnProgressHandler(float progress);
	public delegate void OnFailHandler();
	public delegate void OnSuccessHandler(Quaternion rotation);
	public delegate void OnCalibrationAllowed(bool allowedStatus);

	public event OnStartHandler onStart;
	public event OnProgressHandler onProgress;
	public event OnFailHandler onFail;
	public event OnSuccessHandler onSuccess;
	public event OnCalibrationAllowed onCalibrationAllowed;

    public bool isCalibrationComplite = false;

	[SerializeField]float calibrationTime = 2f;
	public float CalibrationTime {get {return this.calibrationTime;}}

	[SerializeField]float maxAngle = 30f;
	[SerializeField]float maxSqrDifference = 10000f;

	float timer;
	float cooldown;

	Vector3[] initPositions;
	Vector3[] currentPositions;

    private static Quaternion sensorOffset = Quaternion.identity;
    public static Quaternion GetSensorOffset { get { return sensorOffset; } }

    nuitrack.JointType[] checkedJoints = new nuitrack.JointType[]
	{
		nuitrack.JointType.Head, nuitrack.JointType.Torso, 
		nuitrack.JointType.LeftShoulder, nuitrack.JointType.LeftElbow, nuitrack.JointType.LeftWrist,
		nuitrack.JointType.RightShoulder, nuitrack.JointType.RightElbow, nuitrack.JointType.RightWrist
	};
	
	bool calibrationStarted;
	bool calibrationAllowed = false;

    Quaternion SensorOffset()
    {

        Vector3 torso = choiceStream.GetJoint(nuitrack.JointType.Torso, currentNumberPlayer);
        Vector3 collar = choiceStream.GetJoint(nuitrack.JointType.LeftCollar, currentNumberPlayer);
        float angle = Mathf.Atan2((collar - torso).z, (collar - torso).y) * Mathf.Rad2Deg;
        return Quaternion.Euler(angle, 0f, 0f);
    }
    Quaternion GetSensorOffsetLerpOnProcess()
    {
        return Quaternion.Lerp(sensorOffset, SensorOffset(), 0.1f);
    }


    public bool CalibrationAllowed 
	{
		get {return this.calibrationAllowed;} 
		set {calibrationAllowed = value; if (onCalibrationAllowed != null) onCalibrationAllowed(calibrationAllowed);}
	}

    int currentNumberPlayer;

	void Start () 
	{
        if (Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.WindowsPlayer)
            Destroy(this);


        playerBehaviour = gameObject.GetComponent<PlayerBeahaviour>();
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        currentNumberPlayer = playerBehaviour.numberUser;

        DontDestroyOnLoad(gameObject);
		if (GameObject.FindObjectsOfType<TPoseCalibration>().Length > 1) //just in case
		{
			Destroy(gameObject);
		}
		timer = 0f;
		cooldown = 0f;
		calibrationStarted = false;
		initPositions = new Vector3[checkedJoints.Length];
		currentPositions = new Vector3[checkedJoints.Length];

	}
	
	void Update () 
	{
        calibrationAllowed = true;

        if (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
		}
		else if (calibrationAllowed)
		{
			if (choiceStream.GetSegmentationID() != null)
			{
				if (!calibrationStarted)
				{
					StartCalibration();

				}
				else
				{
					if (timer > calibrationTime)
					{
						calibrationStarted = false;
						timer = 0f;
						cooldown = calibrationTime;

						if (onSuccess != null) onSuccess(GetHeadAngles());
                        isCalibrationComplite = true;



                    }
					else
					{
						ProcessCalibration();
						if (!calibrationStarted)
						{
                            timer = 0f;
							if (onFail != null) onFail();
                            sensorOffset = Quaternion.Euler(0f, 0f, 0f);
                        }
						else
						{
							if (onProgress != null) onProgress(timer / calibrationTime);
							timer += Time.deltaTime;
						}
					}
				}
			}
		}
	}

	void StartCalibration()
	{
		bool checkFailed = false;
		//if (NuitrackManager.Skeletons.Length < 1) return;

        foreach (nuitrack.Skeleton skelly in choiceStream.GetSkeletons())
		{
			checkFailed = false;
			Dictionary<nuitrack.JointType, nuitrack.Joint> joints = new Dictionary<nuitrack.JointType, nuitrack.Joint>();

			{
				int i = 0;
				foreach (nuitrack.JointType joint in checkedJoints)
				{
					joints.Add(joint, skelly.GetJoint(joint));
					if (joints[joint].Confidence < 0.5f) 
					{
						checkFailed = true;
						break;
					}
                    initPositions[i] = choiceStream.GetJoint(joint, currentNumberPlayer) * 0.001f;// joints[joint].ToVector3();
					i++;
				}
			}

			if (checkFailed) continue;

			Vector3[] handDeltas = new Vector3[6];
            
            handDeltas[0] = choiceStream.GetJoint(nuitrack.JointType.LeftWrist, currentNumberPlayer) - choiceStream.GetJoint(nuitrack.JointType.RightWrist, currentNumberPlayer);
            handDeltas[1] = choiceStream.GetJoint(nuitrack.JointType.LeftWrist, currentNumberPlayer) - choiceStream.GetJoint(nuitrack.JointType.LeftElbow, currentNumberPlayer);
            handDeltas[2] = choiceStream.GetJoint(nuitrack.JointType.LeftElbow, currentNumberPlayer) - choiceStream.GetJoint(nuitrack.JointType.LeftShoulder, currentNumberPlayer);
            handDeltas[3] = choiceStream.GetJoint(nuitrack.JointType.LeftShoulder, currentNumberPlayer) - choiceStream.GetJoint(nuitrack.JointType.RightShoulder, currentNumberPlayer);
            handDeltas[4] = choiceStream.GetJoint(nuitrack.JointType.RightShoulder, currentNumberPlayer) - choiceStream.GetJoint(nuitrack.JointType.RightElbow, currentNumberPlayer);
            handDeltas[5] = choiceStream.GetJoint(nuitrack.JointType.RightElbow, currentNumberPlayer) - choiceStream.GetJoint(nuitrack.JointType.RightWrist, currentNumberPlayer);

			for (int i = 1; i < 6; i++)
			{
				if ( Vector3.Angle (handDeltas[0], handDeltas[i]) > maxAngle) 
				{
					checkFailed = true;
					break;
				}
			}

			if (!checkFailed)
			{
				//NuitrackManager.Instance.OverrideCurrentUser(skelly.ID);
				break;
			}
		}

		if (checkFailed) return; // no user in t-pose

		calibrationStarted = true;
		if (onStart != null) onStart();
        sensorOffset = SensorOffset();
    }
	
	void ProcessCalibration()
	{
		Dictionary<nuitrack.JointType, Vector3> joints = new Dictionary<nuitrack.JointType, Vector3>();
		{
			int i = 0;
			foreach (nuitrack.JointType joint in checkedJoints)
			{
				joints.Add(joint, choiceStream.GetJoint(joint, currentNumberPlayer));

                //choiceStream.GetJoint(joint, currentNumberPlayer)
                currentPositions[i] = choiceStream.GetJoint(joint, currentNumberPlayer) * 0.001f;
				i++;
			}
		}
		for (int i = 0; i < initPositions.Length; i++)
		{
			if ((initPositions[i] - currentPositions[i]).sqrMagnitude > maxSqrDifference)
			{
				calibrationStarted = false;
				return;
			}
		}
        sensorOffset = GetSensorOffsetLerpOnProcess();
    }

	Quaternion GetHeadAngles()
	{
		float angleY = -Mathf.Rad2Deg * Mathf.Atan2 ((currentPositions[4] - currentPositions[7]).z, (currentPositions[4] - currentPositions[7]).x);
		float angleX = Vector3.Angle(Vector3.Cross(Vector3.right, Input.gyro.gravity), new Vector3(0f, 0f, -1f));

		Debug.Log ("Gravity vector: " + Input.gyro.gravity.ToString("0.000") + "; AngleX: " + angleX.ToString("0") + "; AngleY: " + angleY.ToString("0"));

		return Quaternion.Euler(angleX, angleY, 0f);
	}
}
