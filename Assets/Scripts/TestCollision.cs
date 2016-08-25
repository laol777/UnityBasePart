using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCollision : MonoBehaviour {
    Vector3 offset;
    Quaternion startRotation;

    SkeletonTracker skeletonTracker;
    ChoiceStream choiceStream;


    public Vector3 tmpPos;
    public int numberUser = 1;


    int[,] depthFrame;
    int[,] userFrame;

    [SerializeField]GameObject shootEffectPlaceVisualisation;
    void Start()
    {
        StartCoroutine(RedactPlayerPrefabName()); // hasAuthorithy change value after 1 frame
        tmpPos = new Vector3();
        startRotation = new Quaternion();

        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
    }

    IEnumerator RedactPlayerPrefabName()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.name = "hostPlayer";
        offset = new Vector3(0f, 0f, 5f);
        startRotation = Quaternion.Euler(0f, 0f, 0f);
        numberUser = 1;
        {
            transform.position += offset;
        }
       
    }

    bool isEffectFailProcess = false;


    [SerializeField]Transform bullet;

    void Update()
    {
        depthFrame = choiceStream.GetDepthFrame();
        userFrame = choiceStream.GetUserFrame();
        int userID = choiceStream.GetUserID(numberUser);
        Vector3 depthWithOffset = Vector3.zero;
        Vector3 tmpDepth;

        Vector3 bulletCoord = Vector3.zero;
        Vector3 depthCoord = Vector3.zero;


        bool isBreak = false;

        if (depthFrame != null && userFrame != null)
        {
            for (int i = 0; i < choiceStream.YRes; ++i)
            {
                if (isBreak) break;
                for (int j = 0; j < choiceStream.XRes; ++j)
                {
                    if (isBreak) break;
                    if (userFrame[i, j] != 0) //check it
                    {
                        tmpDepth.x = (float)((j - 40f) / (80f)) * 3f;
                        tmpDepth.y = -(float)((i - 30f) / (60f)) * 2f;
                        tmpDepth.z = depthFrame[i, j] * 0.001f + offset.z;

                        depthWithOffset = tmpDepth;

                        if (Vector3.Distance(depthWithOffset, bullet.position) < 0.05f)
                        {
                            if (!isEffectFailProcess)
                            {
                                isBreak = true;
                                StartCoroutine(EffectFail());
                                GameObject tmpShootEffectPlaceVisualisation = (GameObject)Instantiate(shootEffectPlaceVisualisation, depthWithOffset, Quaternion.identity);
                                Destroy(tmpShootEffectPlaceVisualisation, 1f);
                            }
                        }

                            
                        }

                    }

            }
        }

    }

    IEnumerator EffectFail()
    {
        isEffectFailProcess = true;
        yield return new WaitForSeconds(0.2f);
        isEffectFailProcess = false;
    }
}
