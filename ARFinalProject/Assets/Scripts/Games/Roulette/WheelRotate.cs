using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotate : MonoBehaviour {
    public bool _isStart;
    public float _speed;
    // Use this for initialization
    void Start () {
        _isStart = false;
    }

    // Update is called once per frame
    void Update() {
        if (_isStart)
        {
            transform.RotateAround(new Vector3(transform.parent.transform.position.x, transform.parent.transform.position.y, transform.parent.transform.position.z), Vector3.up, _speed * Time.deltaTime);
        }
	}
}
