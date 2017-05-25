using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIImageScript : MonoBehaviour {

	public Vector3 _begin;
	private float scale = 1f/60f;
	private Vector3 _zoom;

	private int _t = 0;
	private Vector3 _velocity;
	private bool _isMove = false;

	// Use this for initialization
	void Start () {
		_zoom = new Vector3(scale, scale, scale);
	}
	
	// Update is called once per frame
	void Update () {
		if (_isMove) {
			_t++;
			if (_t >= 30) {
				_isMove = false;
				_t = 0;
			} else {
				transform.localPosition += _velocity;
				transform.localScale += _zoom;
			}
		}
	}

	public void SetMove() {
		_isMove = true;
		_begin = transform.localPosition;
		_velocity =  _begin / -30f;
	}
}
