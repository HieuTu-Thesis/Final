using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect3 : MonoBehaviour {
    public GameObject[] _gameObjects;
    public float _degrees, _changeDegrees;
    private int _startIdx, _endIdx;
    private int[] _count;
	public static bool EFFECT = false;
	// Use this for initialization
	void Start () {
        _startIdx = 0;
        _endIdx = 1;
        _count = new int[_gameObjects.Length];
        for (int i = 0; i < _count.Length; i++)
            _count[i] = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (!EFFECT)
			return;
        if (_startIdx != _endIdx)
        {
            for (int i = _startIdx; i < _endIdx; i++)
            {
                if (_gameObjects[_endIdx - 1].transform.localEulerAngles.x > _changeDegrees)
                {
                    if (_endIdx < _gameObjects.Length) _endIdx++;
                }
                
                if (_gameObjects[i].transform.localEulerAngles.x <= 360)
                {
                    //Vector3 rotationVector = _gameObjects[i].transform.localEulerAngles;
                    //Vector3 v = new Vector3(_gameObjects[i].transform.localEulerAngles.x + 1, _gameObjects[i].transform.localEulerAngles.y, _gameObjects[i].transform.localEulerAngles.z);
                    //rotationVector = v;
                    //_gameObjects[i].transform.localRotation = Quaternion.Euler(rotationVector);
                    _gameObjects[i].transform.Rotate(Vector3.right, _degrees);
                    _count[i]++;
                    if (_count[i] == 360 / (int)_degrees) _startIdx = i + 1;
                    //Debug.Log(_gameObjects[0].transform.localEulerAngles.x);
                    //if (_gameObjects[i].transform.localEulerAngles.x >= -1 && _gameObjects[i].transform.localEulerAngles.x <= 1) _startIdx = i + 1;
                }
                //else if (_gameObjects[i].transform.localEulerAngles.x ? )
                //    _startIdx = i + 1;
            }
        }
	}
}
