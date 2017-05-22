using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultility : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void MyDebug(string info, object obj) {
		string more = " obj null";
		if (obj != null)
			more = obj.ToString ();
		Debug.Log (info + " " + more);
	}
}
