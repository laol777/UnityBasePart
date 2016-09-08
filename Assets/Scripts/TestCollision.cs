using UnityEngine;
using System.Collections;

public class TestCollision : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
       
        Debug.Log("collision");
     
    }
}
