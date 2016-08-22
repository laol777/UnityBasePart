using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletContainer : MonoBehaviour {

    List<Transform> bullet;

    void Start()
    {
        bullet = new List<Transform>();
    }

    public void AddBullet(Transform newObj)
    {
        if(!newObj.GetComponent<MoveBullet>().IsLocal)
            bullet.Add(newObj);
    }

    public void RemoveBullet(Transform removedOdj)
    {
         bullet.Remove(removedOdj);       
    }

    public List<Transform> GetBullet()
    {
        return bullet;
    }


    void Update ()
    {
        RemoveBullet(null);
	}
}
