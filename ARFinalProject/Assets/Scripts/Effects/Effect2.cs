using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect2 : MonoBehaviour
{
    public GameObject[] _gameObjects;

    public float R, G, B;
    public int _endY;
    public float _delayTime;
    private int _endIdx, _startIdx, y, _originY;
    private int _idx;

    public float _degrees, _changeDegrees;
    private int[] _count;
    public bool _rotateEffect;
    public bool _initEffect;
    public bool _isStartRotate;
    // Use this for initialization
    void Start()
    {
        _isStartRotate = false;
        _endIdx = 1;
        _startIdx = 0;
        _originY = (int)_gameObjects[0].transform.localPosition.y;
        _idx = 0;
        y = (int)(0.7F * _originY);

        _count = new int[_gameObjects.Length];
        for (int i = 0; i < _count.Length; i++)
            _count[i] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_initEffect == true && _startIdx != _endIdx)
        {
            for (int i = _startIdx; i < _endIdx; i++)
            {
                if (_gameObjects[i].transform.localPosition.y == _originY && _gameObjects[i].GetComponent<MeshRenderer>().enabled == false)
                {
                    _gameObjects[i].GetComponent<MeshRenderer>().enabled = true;
                }
                if (_gameObjects[_endIdx - 1].transform.localPosition.y > y)
                {
                    if (_endIdx < _gameObjects.Length) _endIdx++;
                }
                if (_gameObjects[i].transform.localPosition.y < _endY) _gameObjects[i].transform.localPosition = new Vector3(_gameObjects[i].transform.localPosition.x, _gameObjects[i].transform.localPosition.y + 1, _gameObjects[i].transform.localPosition.z);
                else if (_gameObjects[i].transform.localPosition.y >= _endY)
                    _startIdx = i + 1;
            }
            if (_startIdx == _endIdx)
            {
                _initEffect = false;
                _endIdx = 1;
                _startIdx = 0;
                StartCoroutine(StartWait(_delayTime));
            }
        }
        //Effect3
        if (_rotateEffect == true && _startIdx != _endIdx)
        {
            for (int i = _startIdx; i < _endIdx; i++)
            {
                if (_gameObjects[_endIdx - 1].transform.localEulerAngles.x > _changeDegrees)
                {
                    if (_endIdx < _gameObjects.Length) _endIdx++;
                }

                if (_gameObjects[i].transform.localEulerAngles.x <= 360)
                {
                    _gameObjects[i].transform.Rotate(Vector3.right, _degrees);
                    _count[i]++;
                    if (_count[i] == 360 / (int)_degrees) _startIdx = i + 1;
                }
            }
            if (_startIdx == _endIdx)
            {
                _rotateEffect = false;
                _isStartRotate = false;
                _endIdx = 1;
                _startIdx = 0;
                StartCoroutine(WaitForShowEffect(2F));
            }
        }
    }
    public void StartRotate()
    {
        _isStartRotate = true;
        StartCoroutine(WaitForRotateEffect(2F));
    }
    IEnumerator StartWait(float time)
    {
        yield return StartCoroutine(Wait(time));
        _gameObjects[_idx].GetComponent<MeshRenderer>().material.color = new Color((float)R / 255, (float)G / 255, (float)B / 255);
        _idx++;
        if (_idx != _gameObjects.Length) StartCoroutine(StartWait(_delayTime));
        else
        {
            for (int i = 0; i < _gameObjects.Length; i++)
            {
                _gameObjects[i].GetComponent<CellUtil>().playHighLightColor(20F);
                _gameObjects[i].transform.FindChild("ETF_Landmine").GetComponent<ParticleSystem>().Play(true);
            }
            gameObject.GetComponent<Effect4>()._isShow = true;
        }
    }

    IEnumerator WaitForRotateEffect(float time)
    {
        yield return StartCoroutine(Wait(time));
        _rotateEffect = true;
    }
    IEnumerator WaitForShowEffect(float time)
    {
        yield return StartCoroutine(Wait(time));
        //Show text
        for (int i = 0; i < _gameObjects.Length; i++)
        {
            if (_gameObjects[i].transform.FindChild("Name") != null) _gameObjects[i].transform.FindChild("Name").GetComponent<MeshRenderer>().enabled = true;
            if (_gameObjects[i].transform.FindChild("Price") != null) _gameObjects[i].transform.FindChild("Price").GetComponent<MeshRenderer>().enabled = true;
            if (_gameObjects[i].transform.FindChild("Type") != null) _gameObjects[i].transform.FindChild("Type").GetComponent<MeshRenderer>().enabled = true;
            _gameObjects[i].transform.FindChild("explode").GetComponent<ParticleSystem>().Play(true);
        }
    }
    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
