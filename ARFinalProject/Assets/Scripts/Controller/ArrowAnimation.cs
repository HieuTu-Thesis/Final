using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	public float t = 0;
	public float x = 1;
	public Vector3 begin;

	// Update is called once per frame
	void Update () {
		t = t + x;
		if (t >= 20f)
			x = -1f;
		else if (t <= 0f)
			x = 1f;
		transform.localPosition = begin + new Vector3(0f, 0.5f * t/ 20.0f, 0f);
	}
}
