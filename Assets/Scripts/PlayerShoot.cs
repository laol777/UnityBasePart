using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {

    [SerializeField]
    Animator shootMeshEffect;

    [SerializeField]
    Transform cameraAim;

    public BulletContainer bulletContainerPlayer;
    public BulletContainer bulletContainerBot;


    [SerializeField]
    AudioSource soundShoot;

    [SerializeField]
    Transform rightWrist;

    Vector3 tmpF;

    Vector3[] handDeltas;

    ChoiceStream choiceStream;

    float time;

    bool scalePositionCursor;
    bool prevScalePositionCursor;
    bool isShootPlayer = true;


    public GameObject bulletPrefab;
    public GameObject cursor;
    public GameObject quadCamera;
    public GameObject quadCursor;

        [SerializeField]
    GameObject paticleEffect;

    Vector3 GetVector3Joint(nuitrack.Joint joint)
    {
        Vector3 returnedJoint = new Vector3();

        returnedJoint.x = joint.Real.X;
        returnedJoint.y = joint.Real.Y;
        returnedJoint.z = joint.Real.Z;

        return returnedJoint;
    }



    void Start () {
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        handDeltas = new Vector3[2];
    }
	
	void Update ()
    {
        rightWrist.localPosition = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightWrist, 1)) * 0.001f;

        handDeltas[0] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightWrist, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1));
        handDeltas[1] = GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightElbow, 1)) - GetVector3Joint(choiceStream.GetJoint(nuitrack.JointType.RightShoulder, 1));

        Vector3 vectorShoot = Vector3.Normalize(handDeltas[0]);

        tmpF.x = vectorShoot.x * 2f;
        tmpF.y = vectorShoot.y * 2f;
        tmpF.z = -10f;
        cursor.transform.localPosition = tmpF;
        time += Time.deltaTime;


        float angle = Vector3.Angle(vectorShoot, Vector3.back);

        float angle2 = Vector3.Angle(handDeltas[0], handDeltas[1]);

        if (angle < 30f && angle2 < 30f)
        {
            scalePositionCursor = true;
        }
        else if ((angle < 40f) && (prevScalePositionCursor))
        {
            scalePositionCursor = true;
        }
        else
        {
            scalePositionCursor = false;
        }



        if (scalePositionCursor)
        {
            if (time > 1f && isShootPlayer)
            {
                time = 0f;
                GameObject tmp = (GameObject)Instantiate(bulletPrefab, rightWrist.position, Quaternion.identity);
                tmp.transform.parent = transform;
                Destroy(tmp.GetComponent<AudioSource>());
                //tmp.GetComponent<AudioSource>().enabled = false;
                tmp.GetComponent<MoveBullet>().vector = Vector3.Normalize(cursor.transform.localPosition - rightWrist.localPosition);
                tmp.GetComponent<MoveBullet>().velocity = 3f;
                //if (!hasAuthority)
                {
                    bulletContainerPlayer.AddBullet(tmp.transform);
                }
                GameObject tmpPaticle = (GameObject)Instantiate(paticleEffect, rightWrist.position, Quaternion.identity);
                tmpPaticle.transform.parent = rightWrist.transform;
                Destroy(tmpPaticle, 1.3f);
                soundShoot.Play();
                //shootMeshEffect.Play("shoot");
                shootMeshEffect.SetTime(0);
                Destroy(tmp, 10f);

            }
        }

        if (cursor != null)
        {

            vectorShoot.z = 0f;
            vectorShoot.x = -vectorShoot.x;

            quadCursor.transform.localPosition = vectorShoot / 2f;

            prevScalePositionCursor = scalePositionCursor;

        }

        if (quadCamera != null)
        {
            quadCamera.SetActive(scalePositionCursor);
        }
    }
}
