using UnityEngine;
using System.Collections;

public class TestNuitrack : MonoBehaviour {

    DepthSensor depthSensor;
    [SerializeField]TextMesh text;
	void Start () {
        depthSensor = GameObject.FindObjectOfType<DepthSensor>();
	}

 //   void Update () {
 //       if (depthSensor != null && depthSensor.DepthFrame != null)
 //           text.text = depthSensor.DepthFrame[30, 40].ToString();
 //           //text.text = depthSensor.DepthFrame.Rows.ToString() + " " + depthSensor.DepthFrame.Cols.ToString();
	//}
}
