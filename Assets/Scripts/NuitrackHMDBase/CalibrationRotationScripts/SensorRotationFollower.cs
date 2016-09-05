using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SensorRotationFollower : MonoBehaviour
{
    [SerializeField]
    Transform follower;
    SensorRotation sensorRotation;

    void Start()
    {
        //if (this.isLocalPlayer)
        {
            sensorRotation = GameObject.FindObjectOfType<SensorRotation>();
        }
    }

    void Update()
    {
        //if (this.isLocalPlayer)
        {
            if(sensorRotation != null)
            follower.localRotation = sensorRotation.Rotation;
        }
    }
}
