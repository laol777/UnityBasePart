
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotControl : MonoBehaviour {

    float timeBetweenShoot = 0.4f;
    Vector3 nextPosition;

    public bool startShoot = false;

    Vector3 basePos;
    [SerializeField]GameObject bullet;

    [SerializeField]
    BulletContainer bulletContainerBot;
    [SerializeField]
    BulletContainer bulletContainerPlayer;



    int[,] depthFrame;
    int[,] userFrame;

    ChoiceStream choiceStream;
    Vector3 offset;




    MiniCalibration miniCalibration;

    void Start()
    {
        
        choiceStream = GameObject.FindObjectOfType<ChoiceStream>();
        basePos = transform.position;
        miniCalibration = GameObject.FindObjectOfType<MiniCalibration>();

    }

    float vel = 1f;

    public int iter = 0;

    void Update()
    {

        if (miniCalibration == null)
        {
            miniCalibration = GameObject.FindObjectOfType<MiniCalibration>();
        }

        if (miniCalibration != null && miniCalibration.isCalibrationComplite)
        {
            ++iter;
        }

        if (startShoot && iter == 1 )
        {
            startShoot = false;
            StartCoroutine(ShootBehaviour());
        }

        target.position = Vector3.Lerp(target.position, targetNextPose, Time.deltaTime * 5f);
    }

    [SerializeField]
    Transform target;
    Vector3 targetNextPose;
    [SerializeField]
    Transform rightWrist;

    IEnumerator ShootBehaviour()
    {

        Vector3 rndSize = new Vector3(Random.Range(-0.03f, 0.03f), Random.Range(-0.05f, 0.05f), 0f);
        targetNextPose = new Vector3(rndSize.x * 33f, rndSize.y * 33f, 0f);

        yield return new WaitForSeconds(0.5f);

        GameObject tmpBullet = (GameObject)Instantiate(bullet, rightWrist.position, Quaternion.identity);
        tmpBullet.GetComponent<AudioSource>().Play();
        bulletContainerBot.AddBullet(tmpBullet.transform);


        tmpBullet.GetComponent<MoveBullet>().velocity = 6f;
        tmpBullet.GetComponent<MoveBullet>().vector = Vector3.back + rndSize;

        Destroy(tmpBullet, 12f);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ShootBehaviour());
    }

}
