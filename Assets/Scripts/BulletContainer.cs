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
        bullet.Add(newObj);
    }

    public void RemoveBullet(Transform removedOdj)
    {
         bullet.Remove(removedOdj);       
    }


    void Update ()
    {
        RemoveBullet(null);
	}
}
