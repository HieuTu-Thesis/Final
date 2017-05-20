using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {
	void Start () {

	}

	// Update is called once per frame
	void LateUpdate () {
		transform.position = new Vector3 (transform.position.x, 111f, transform.position.z);
	}
}
