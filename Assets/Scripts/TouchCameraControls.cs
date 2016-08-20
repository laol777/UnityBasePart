using UnityEngine;
using System.Collections;

public class TouchCameraControls : MonoBehaviour 
{
	[SerializeField]Transform camera;
  [SerializeField]float minCameraDistance = 1f;
  [SerializeField]float maxCameraDistance = 10f;
  float xAngle = 0f, yAngle = 0f;

	float cameraDistance;
	bool had2touches = false;

	void Start () 
	{
		cameraDistance = camera.localPosition.z;
	}

	float prev2touchesDistance;

	void Update () 
	{
		if (Input.touchCount == 1)
		{
			had2touches = false;
			xAngle -= Input.touches[0].deltaPosition.y;
			yAngle += Input.touches[0].deltaPosition.x;

			xAngle = Mathf.Clamp (xAngle, -90f, 90f);
			while (yAngle > 360f)
			{
				yAngle -= 360f;
			}
			while (yAngle < -360f)
			{
				yAngle += 360f;
			}
			transform.rotation = Quaternion.Euler(xAngle, yAngle, 0f);
		}
		else if (Input.touchCount == 2)
		{
			if (!had2touches)
			{
				prev2touchesDistance = (Input.touches[0].position - Input.touches[1].position).magnitude;
				had2touches = true;
			}
			else
			{
				float current2touchesDistance = (Input.touches[0].position - Input.touches[1].position).magnitude;
				cameraDistance *= prev2touchesDistance / current2touchesDistance;
				cameraDistance = Mathf.Clamp(cameraDistance, -maxCameraDistance, -minCameraDistance);
				camera.localPosition = new Vector3(0f, 0f, cameraDistance);
				prev2touchesDistance = current2touchesDistance;
			}
		}
		else
		{
			had2touches = false;
		}
	}
}
