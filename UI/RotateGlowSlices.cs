using UnityEngine;
using System.Collections;

public class RotateGlowSlices : MonoBehaviour {

	void OnEnable()
	{
		//iTween.RotateTo(gameObject, iTween.Hash("rotation", new Vector3(0,0,90), "time",1f, "loopType","loop","easeType", iTween.EaseType.linear, "islocal",true));
	}

	void OnDisable()
	{

	}

	void Rotate()
	{
		//transform.localRotation = Quaternion.Euler(new Vector3(0,0, transform.localRotation.eulerAngles.z + 60*Time.deltaTime));
		transform.localEulerAngles += (Vector3.forward * Time.deltaTime * 35);
	}

	void Update()
	{
		Rotate();
	}

}
