using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour {
	private static Singleton instance;

	private void Awake() {
		if (instance != null) {
			Destroy(gameObject);
		}else{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public static Singleton GetInstance() {
		return instance;
	}

	private Singleton() {
	}

	// Use this for initialization
	void Start () {
		if (Singleton.GetInstance ())
			Debug.Log ("Successss");
		else
			Debug.Log ("Faillll");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
