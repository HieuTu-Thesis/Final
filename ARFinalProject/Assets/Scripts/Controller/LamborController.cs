using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LamborController : MonoBehaviour {

	Transform _cy14, _cy15, _cy16, _cy17;
	private float rotate = 0;

	// Use this for initialization
	void Start () {
		_cy14 = transform.Find ("Cylinder014");
		_cy15 = transform.Find ("Cylinder015");
		_cy16 = transform.Find ("Cylinder016");
		_cy17 = transform.Find ("Cylinder017");
	}
	
	// Update is called once per frame
	void Update () {
		rotate -= 10;
		if (rotate <= -360)
			rotate += 360;
		_cy14.localEulerAngles = _cy15.localEulerAngles = _cy16.localEulerAngles = _cy17.localEulerAngles = new Vector3 (0f, 0f, rotate); 
	}
}
