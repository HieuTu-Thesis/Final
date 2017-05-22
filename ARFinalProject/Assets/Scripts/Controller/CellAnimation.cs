using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAnimation : MonoBehaviour {

	public float t = 0;
	public float x = 1;
	public bool f = true;
	public bool s = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (f || s) {
			t = t + x;

			if (f && t > 5) {
				f = false;
				x = -1f;
			}
			if (s && t <= 0)
				s = false;
			
			transform.localPosition += new Vector3 (0f, -0.1f * x / 5.0f, 0f);
		} else {
			transform.localPosition = new Vector3 (transform.localPosition.x, 0f, transform.localPosition.z);
			Destroy (this);
		}
	}
}
