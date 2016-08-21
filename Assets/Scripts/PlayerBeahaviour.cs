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

        if(!hasAuthority) //disable, when is starting as remote player on current device
        {
            camera.gameObject.SetActive(false);
            cursor.SetActive(false);
        }
    }

    public GameObject bullet;
    public GameObject cameraAim;
    public GameObject cursor;
    public GameObject quadCamera;
    public GameObject quadCursor;

    bool scalePositionCursor = false;
    bool prevScalePositionCursor = false;

    public Vector3 offs;


    bool isStartFail;
    Vector3 tmpF;
    float timeFail = 5f;

    bool isHaveAbilityFail;

    float time = 0f;

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

        Vector3 vectorShoot = Vector3.Normalize(rightWrist.position - rightElbow.position);
        cursor.transform.localPosition = new Vector3(vectorShoot.x * 2f, vectorShoot.y * 2f, -10f);

        time += Time.deltaTime;
        if (time > 1f)
        {
            time = 0f;
            GameObject tmp = (GameObject)Instantiate(bullet, rightWrist.position, Quaternion.identity);
            tmp.GetComponent<MoveBullet>().vector = vectorShoot;
            tmp.GetComponent<MoveBullet>().velocity = 3f;
            Destroy(tmp, 10f);
        }

        if (hasAuthority)
        {
            #region myPart
            if (cursor != null)
            {
                float angle = Vector3.Angle(vectorShoot, new Vector3(0f, 0f, -1f));

                if (angle < 30f)
                {
                    scalePositionCursor = true;
                }
                else if ((angle < 60f) && (prevScalePositionCursor))
                {
                    scalePositionCursor = true;
                }
                else
                {
                    scalePositionCursor = false;
                }

                if (quadCamera != null)
                {
                    quadCamera.SetActive(scalePositionCursor);
                }

                prevScalePositionCursor = scalePositionCursor;

            }
            #endregion
        }

        /*
        if (hasAuthority)
        {
            #region myPart




            if (cursor != null)
            {
                Vector3 vectorShoot = Vector3.Normalize(cameraAim.transform.position - rightWrist.position);
                float angle = Vector3.Angle(vectorShoot, new Vector3(0f, 0f, 1f));

                if (angle < 30f)
                {
                    scalePositionCursor = true;
                }
                else if ((angle < 60f) && (prevScalePositionCursor))
                {
                    scalePositionCursor = true;
                }
                else
                {
                    scalePositionCursor = false;
                }

                if (scalePositionCursor)
                {
                    cursor.transform.position = new Vector3(vectorShoot.x * 2f, vectorShoot.y * 2f, 11.5f);
                }

                cursor.SetActive(scalePositionCursor);

                if (quadCamera != null)
                {
                    quadCamera.SetActive(scalePositionCursor);
                }

                Vector3 tmp = new Vector3(vectorShoot.x * 0.075f, vectorShoot.y * 0.075f, 0f);

                if (quadCursor != null)
                {
                    quadCursor.transform.localPosition = tmp;
                }
                prevScalePositionCursor = scalePositionCursor;

                float xOffset = (vectorShoot.x + 1f) / 2f;
                float yOffset = (vectorShoot.y + 1f) / 2f;

                tmp = Vector3.Normalize(cursor.transform.position - rightWrist.position) * 90f;
                rightWrist.rotation = Quaternion.Euler(tmp.x, tmp.y, tmp.z);

                offs.y = -30f;

                rightWrist.localRotation *= Quaternion.Euler(offs);

            }
                      #endregion
                       }
            */



    }
}
