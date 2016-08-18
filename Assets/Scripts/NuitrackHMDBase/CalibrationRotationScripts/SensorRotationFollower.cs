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
      follower.localRotation = sensorRotation.Rotation;
      //CmdUpdateHead(follower.position, follower.localRotation);
    }
  }

  //[Command(channel = 0)]
  //public void CmdUpdateHead(Vector3 newPos, Quaternion newLocalRotation)
  //{
  //  RpcUpdateHead(newPos, newLocalRotation);
  //}

  //[ClientRpc(channel = 1)]
  //public void RpcUpdateHead(Vector3 newPos, Quaternion newLocalRotation)
  //{
  //  if (!this.isLocalPlayer)
  //  {
  //    follower.position = newPos;
  //    follower.localRotation = newLocalRotation;
  //  }
  //}
}
