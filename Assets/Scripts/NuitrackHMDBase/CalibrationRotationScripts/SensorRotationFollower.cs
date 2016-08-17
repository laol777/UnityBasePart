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
        sensorRotation = GameObject.FindObjectOfType<SensorRotation>();
    }

    void Update()
    {
        follower.localRotation = sensorRotation.Rotation;
    }
}
