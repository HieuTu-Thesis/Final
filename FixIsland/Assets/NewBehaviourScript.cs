using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
	private Rigidbody rbody;

	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		rbody.velocity = new Vector3 (2, 0, 2);
	}
}
