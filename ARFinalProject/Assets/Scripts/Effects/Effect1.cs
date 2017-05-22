using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect1 : MonoBehaviour {
    public float _showTime;
    private Color _color, _currentColor;
    private float _scaleG, _scaleB, _scale, _current, _origin;
    private bool _isIncrese, _isPlay;
    public int _type;
    public bool _isShow, _isChanged;
    public Material _material;
	public static bool EFFECT = false;
    // Use this for initialization
    void Start() {
        Calculate();
        StartCoroutine(StartWait(_showTime));
    }

    // Update is called once per frame
    void Update()
    {
		if (!EFFECT)
			return;
		
        if (_isPlay == false && _isShow == true) Calculate();

        if (_type == 0 && _isChanged == true)
        {
            _color = gameObject.GetComponent<MeshRenderer>().material.color;
            _scaleB = _color.r / _color.b;
            _scaleG = _color.r / _color.g;
            _current = _color.r;
            _origin = _color.r;
            _isChanged = false;
        }
        else if (_type == 1 && _isChanged == true)
        {
            _color = gameObject.GetComponent<MeshRenderer>().material.color;
            _current = _color.g;
            _origin = _color.g;
            _scale = _color.g / _color.b;
            _isChanged = false;
        }

        if (!((Mathf.Abs(_current - _origin) <= (((float)(_origin / 2) / 0.5F) * (float)2 / 255)) && _isShow == false))
        {
            _isPlay = true;
            if (_current <= _origin / 2 || _isShow == false)
            {
                _isIncrese = true;
            }
            else if (_current >= _origin && _isShow != false)
            {
                _isIncrese = false;
            }
            if (_isIncrese == true)
                _current = _current + (((float)(_origin / 2) / 0.5F) * (float)2 / 255);
            else _current = _current - (((float)(_origin / 2) / 0.5F) * (float)2 / 255);
            if (_type == 0)
            {
                _currentColor.g = _current / _scaleG;
                _currentColor.b = _current / _scaleB;
                _currentColor.r = _current;
            }
            else if (_type == 1)
            {
                _currentColor.g = _current;
                _currentColor.b = _current / _scale;
                _currentColor.r = 0;
            }
            else if (_type == 2)
            {
                _currentColor.r = _current;
                _currentColor.b = _current / _scale;
                _currentColor.g = 0;
            }
            else if (_type == 3)
            {
                _currentColor.r = _current;
                _currentColor.g = _current / _scale;
                _currentColor.b = 0;
            }
            else if (_type == 4)
            {
                _currentColor.r = _current;
                _currentColor.g = 0;
                _currentColor.b = 0;
            }
            else if (_type == 5)
            {
                _currentColor.g = _current;
                _currentColor.r = 0;
                _currentColor.b = 0;
            }
            else if (_type == 6)
            {
                _currentColor.b = _current;
                _currentColor.r = 0;
                _currentColor.g = 0;
            }
            gameObject.GetComponent<MeshRenderer>().materials[0].color = _currentColor;
        }
        else if (_isPlay == true)
        {
            gameObject.GetComponent<MeshRenderer>().material = _material;
            _current = _origin;
            _isPlay = false;
        }
    }
    IEnumerator StartWait(float time)
    {
        yield return StartCoroutine(Wait(time));
        _isShow = false;
        //Show true color
        if (transform.parent.gameObject.GetComponent<Effect2>()._isStartRotate == false) transform.parent.gameObject.GetComponent<Effect2>().StartRotate();
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
    void Calculate()
    {
        _color = gameObject.GetComponent<MeshRenderer>().material.color;
        _currentColor = _color;
        _type = 0;
        if (_color.r == 0F && _color.g != 0F && _color.b != 0F)
        {
            _scale = _color.g / _color.b;
            _type = 1;
            _current = _color.g;
            _origin = _color.g;
        }
        else if (_color.g == 0F && _color.r != 0F && _color.b != 0F)
        {
            _scale = _color.r / _color.b;
            _type = 2;
            _current = _color.r;
            _origin = _color.r;
        }
        else if (_color.b == 0F && _color.g != 0F && _color.r != 0F)
        {
            _scale = _color.r / _color.g;
            _type = 3;
            _current = _color.r;
            _origin = _color.r;
        }
        else if (_color.r != 0F && _color.g == 0F && _color.b == 0F)
        {
            _type = 4;
            _current = _color.r;
            _origin = _color.r;
        }
        else if (_color.g != 0F && _color.r == 0F && _color.b == 0F)
        {
            _type = 5;
            _current = _color.g;
            _origin = _color.g;
        }
        else if (_color.b != 0F && _color.g == 0F && _color.r == 0F)
        {
            _type = 6;
            _current = _color.b;
            _origin = _color.b;
        }
        else
        {
            _scaleB = _color.r / _color.b;
            _scaleG = _color.r / _color.g;
            _current = _color.r;
            _origin = _color.r;
        }
        _isIncrese = false;
        //_isShow = false;
        _isPlay = false;
    }
}
