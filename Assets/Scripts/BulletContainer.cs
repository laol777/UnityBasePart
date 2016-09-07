using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletContainer : MonoBehaviour {

    [SerializeField]List<Transform> bullet;

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

    public List<Transform> GetBullet()
    {
        return bullet;
    }


    void Update()
    {
        RemoveBullet(null);
	}
}
