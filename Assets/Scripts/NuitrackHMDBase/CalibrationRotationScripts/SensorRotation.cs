using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SensorRotation : MonoBehaviour 
{
	Vector3 magneticHeading = Vector3.zero;
	Vector3 gyroGravity = Vector3.down;
	Vector3 gyroRateUnbiased = Vector3.zero;
	
	Vector3 crossProd = Vector3.zero;
	
	Vector3 
		smoothedMagneticHeading = Vector3.zero, 
		smoothedGravity = Vector3.zero;
	
	[SerializeField]float dampCoeffVectors = 0.1f;
	[SerializeField]float dampCoeffMag = 1f;
	
	Quaternion baseRotation = Quaternion.identity;
	Quaternion rotation = Quaternion.identity;
	Quaternion finalRotation = Quaternion.identity;
	public Quaternion Rotation {get {return finalRotation;}}
	
	bool correctionOn = false;
	[SerializeField]float angleCorrectionOn = 15f;
	[SerializeField]float angleCorrectionOff = 3f;
	
	static string ROOM_BASE_ROTATION = "RoomBaseRotation";
	
	IEnumerator Start () 
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Input.compass.enabled = true;
		Input.gyro.enabled = true;
		yield return null;
		LoadBaseRotation();
		InitRotation();
	}
	
	void InitRotation()
	{
		magneticHeading = Input.compass.rawVector;
		magneticHeading = new Vector3(-magneticHeading.y, magneticHeading.x, -magneticHeading.z); // for landscape left
		
		gyroGravity = Input.gyro.gravity;
		gyroGravity = new Vector3(gyroGravity.x, gyroGravity.y, -gyroGravity.z);
		
		smoothedMagneticHeading = magneticHeading;
		smoothedGravity = gyroGravity;
		
		crossProd = Vector3.Cross(smoothedMagneticHeading, smoothedGravity).normalized;
		
		rotation = Quaternion.Inverse(Quaternion.LookRotation(crossProd, -gyroGravity));
	}
	
	void LoadBaseRotation()
	{
		Debug.Log("Loading baseRotation (nuitrack.Nuitrack.GetConfigValue(ROOM_BASE_ROTATION))");
		string configValue = nuitrack.Nuitrack.GetConfigValue(ROOM_BASE_ROTATION);
		Debug.Log("Config value: " + configValue);
		if (configValue == "") return;
		
		byte[] calibrationInfo = Convert.FromBase64String(configValue);
		int index = 0;
		float x, y, z, w;
		x = BitConverter.ToSingle(calibrationInfo, index);
		index += sizeof(float);
		y = BitConverter.ToSingle(calibrationInfo, index);
		index += sizeof(float);
		z = BitConverter.ToSingle(calibrationInfo, index);
		index += sizeof(float);
		w = BitConverter.ToSingle(calibrationInfo, index);
		Quaternion newBaseRotation = new Quaternion(x, y, z, w);
		baseRotation = newBaseRotation;
		Debug.Log("baseRotation: " + baseRotation.ToString());
	}
	
	void SaveBaseRotation()
	{
		Debug.Log("Saving baseRotation (nuitrack.Nuitrack.Configure(ROOM_BASE_ROTATION, val))");
		List<byte> calibratedBaseRotation = new List<byte>();
		calibratedBaseRotation.AddRange(BitConverter.GetBytes(baseRotation.x));
		calibratedBaseRotation.AddRange(BitConverter.GetBytes(baseRotation.y));
		calibratedBaseRotation.AddRange(BitConverter.GetBytes(baseRotation.z));
		calibratedBaseRotation.AddRange(BitConverter.GetBytes(baseRotation.w));
		string val = Convert.ToBase64String(calibratedBaseRotation.ToArray());
		nuitrack.Nuitrack.SetConfigValue(ROOM_BASE_ROTATION, val);
	}
	
	public void SetBaseRotation(Quaternion additionalRotation)
	{
		baseRotation = additionalRotation * Quaternion.Inverse(rotation);
		SaveBaseRotation();
	}
	
	void Update ()
	{
		//if (Input.touchCount > 1) SetBaseRotation(Quaternion.identity);
	}
	
	void FixedUpdate () 
	{
		//RotateSmooth();
		RotateMethod2();
	}
	
	void RotateSmooth()
	{
		magneticHeading = Input.compass.rawVector;
		magneticHeading = new Vector3(-magneticHeading.y, magneticHeading.x, -magneticHeading.z); // for landscape left
		
		gyroGravity = Input.gyro.gravity;
		gyroGravity = new Vector3(gyroGravity.x, gyroGravity.y, -gyroGravity.z);
		gyroRateUnbiased = Vector3.Scale(Input.gyro.rotationRateUnbiased, new Vector3(-1f, -1f, 1f));
		
		smoothedMagneticHeading = Vector3.Slerp(smoothedMagneticHeading, magneticHeading, dampCoeffVectors);
		smoothedGravity = Vector3.Slerp(smoothedGravity, gyroGravity, dampCoeffVectors);
		
		crossProd = Vector3.Cross (smoothedMagneticHeading, smoothedGravity).normalized;
		
		rotation = rotation * Quaternion.Euler(gyroRateUnbiased * Time.deltaTime / Time.timeScale * Mathf.Rad2Deg);
		//angle between current rotation and magnetic:
		float deltaAngle = Quaternion.Angle(rotation, Quaternion.Inverse(Quaternion.LookRotation(crossProd, -gyroGravity)));
		rotation = Quaternion.RotateTowards(rotation, Quaternion.Inverse(Quaternion.LookRotation(crossProd, -gyroGravity)), Time.deltaTime / Time.timeScale * dampCoeffMag * deltaAngle);
		
		finalRotation = baseRotation * rotation;
	}
	
	void RotateMethod2()
	{
		magneticHeading = Input.compass.rawVector;
		magneticHeading = new Vector3(-magneticHeading.y, magneticHeading.x, -magneticHeading.z); // for landscape left
		
		gyroGravity = Input.gyro.gravity;
		gyroGravity = new Vector3(gyroGravity.x, gyroGravity.y, -gyroGravity.z);
		gyroRateUnbiased = Vector3.Scale(Input.gyro.rotationRateUnbiased, new Vector3(-1f, -1f, 1f));
		
		smoothedMagneticHeading = Vector3.Slerp(smoothedMagneticHeading, magneticHeading, dampCoeffVectors);
		smoothedGravity = Vector3.Slerp(smoothedGravity, gyroGravity, dampCoeffVectors);
		
		crossProd = Vector3.Cross (smoothedMagneticHeading, smoothedGravity).normalized;
		
		rotation = rotation * Quaternion.Euler(gyroRateUnbiased * Time.deltaTime / Time.timeScale * Mathf.Rad2Deg);
		//angle between current rotation and magnetic:
		float deltaAngle = Quaternion.Angle(rotation, Quaternion.Inverse(Quaternion.LookRotation(crossProd, -gyroGravity)));
		if (deltaAngle > angleCorrectionOn)
		{
			correctionOn = true;
		}
		if (deltaAngle < angleCorrectionOff)
		{
			correctionOn = false;
		}
		if (correctionOn)
		{
			rotation = Quaternion.RotateTowards(rotation, Quaternion.Inverse(Quaternion.LookRotation(crossProd, -gyroGravity)), Time.deltaTime / Time.timeScale * dampCoeffMag * deltaAngle);
		}
		finalRotation = baseRotation * rotation;
	}
}
