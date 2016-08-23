using UnityEngine;
using System.Collections;

public class BotControl : MonoBehaviour {

    float timeBetweenShoot = 0.4f;
    Vector3 nextPosition;

    public bool startShoot = false;

    Vector3 basePos;
    [SerializeField]GameObject bullet;

    Transform target;
    [SerializeField]
    BulletContainer bulletContainer;

    void Start()
    {
        basePos = transform.position;
        bulletContainer = GameObject.FindObjectOfType<BulletContainer>();
    }

    float vel = 1f;


    void Update()
    {

        if(target == null && GameObject.Find("head") != null)
        {
            target = GameObject.Find("head").transform;
        }

        if (startShoot && target != null)
        {
            startShoot = false;
            StartCoroutine(ShootBehaviour());
        }
    }

    public GameObject[] ball;



    int countShoot = 1;

    IEnumerator ShootBehaviour()
    {
        countShoot++;

        nextPosition.x = basePos.x + Random.Range(-1f, 1f);
        nextPosition.y = basePos.y + Random.Range(-1f, 1f);
        nextPosition.z = transform.position.z;

        GameObject tmpBullet = (GameObject)Instantiate(bullet, transform.position, Quaternion.identity);
        tmpBullet.GetComponent<MoveBullet>().velocity = 3f;
        //Vector3 rndSize = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.3f, 0.8f), 0f);
        //tmpBullet.GetComponent<MoveBullet>().vector = Vector3.Normalize(target.position + rndSize - transform.position);
        Vector3 rndSize = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
        tmpBullet.GetComponent<MoveBullet>().IsLocal = false;
        bulletContainer.AddBullet(tmpBullet.transform);
        tmpBullet.GetComponent<MoveBullet>().vector = Vector3.forward + rndSize;
        Destroy(tmpBullet, 12f);

        yield return new WaitForSeconds(0.6f);
        StartCoroutine(ShootBehaviour());
        //if (countShoot % 4 != 0)
        //{
        //    StartCoroutine(ShootBehaviour());
        //}
        //else
        //{
        //    countShoot++;
        //}
    }

}
