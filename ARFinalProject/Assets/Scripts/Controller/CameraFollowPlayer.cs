using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

	public Player target;
	Vector3[] deltaPosition = { new Vector3 (-2f, 0f, 0f), new Vector3 (0f, 0f, +2f), new Vector3 (2f, 0f, 0f), new Vector3 (0, 0, -2f) };
	//Vector3[] Rotation = { new Vector3 (0f, 90f, 0f), new Vector3 (0f, 180f, 0f), new Vector3 (0f, -90f, 0f), new Vector3 (0f, 0f, 0f) };
	public int idxDelta = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (target._position < 9)
			idxDelta = 0;
		else if (target._position < 18)
			idxDelta = 1;
		else if (target._position < 27)
			idxDelta = 2;
		else
			idxDelta = 3;
		transform.localEulerAngles = new Vector3 (10f, 40f, 0f);
		//transform.localEulerAngles = Rotation [idxDelta];
		//transform.localPosition = new Vector3 (target._player.transform.localPosition.x + deltaPosition [idxDelta].x, 1f, target._player.transform.localPosition.z + deltaPosition [idxDelta].z);
		transform.localPosition = new Vector3 (target._player.transform.localPosition.x - 1, 1, target._player.transform.localPosition.z - 2);
	}
}
