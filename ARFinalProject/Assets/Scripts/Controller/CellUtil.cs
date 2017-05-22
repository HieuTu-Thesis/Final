using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellUtil : MonoBehaviour
{
    public int[] _price;
    public int[] _visitPrice;
    //Build house info
    public GameObject[] _houseModels;
    public GameObject[] _flags;
    public GameObject[] _tempateModels;

    public Material _material;
    public GameObject _upgradeEffect;
    public GameObject _destroyEffect;
    public string _placeName;

    private GameObject _effect;
    private GameObject _dEffect;
    private GameObject _currentHouse;
    private GameObject _currentFlag;
    private int _currentHouseLevel;
    private int _ownerIdx;
    private float _showTime;
    private Color _color, _currentColor;
    private float _scaleG, _scaleB, _scale, _current, _origin;
    private bool _isIncrese, _isPlay;
    private int _type;
    private bool _isShow;

    // Use this for initialization
    void Start()
    {
        _ownerIdx = -1;
        _currentHouseLevel = 0;
        //Calculate();
        //StartCoroutine(StartWait(_showTime));
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlay == false && _isShow == true) Calculate();

        //if (_type == 0 && _isChanged == true)
        //{
        //    _color = gameObject.GetComponent<MeshRenderer>().material.color;
        //    _scaleB = _color.r / _color.b;
        //    _scaleG = _color.r / _color.g;
        //    _current = _color.r;
        //    _origin = _color.r;
        //    _isChanged = false;
        //}
        //else if (_type == 1 && _isChanged == true)
        //{
        //    _color = gameObject.GetComponent<MeshRenderer>().material.color;
        //    _current = _color.g;
        //    _origin = _color.g;
        //    _scale = _color.g / _color.b;
        //    _isChanged = false;
        //}

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

    //Public function
    public int getPrice(int idx)
    {
        Debug.Log(idx);
        if (_price.Length > 0)
            return _price[idx];
        return 0;
    }
    public int getVisitPrice(int idx)
    {
        return _visitPrice[idx];
    }
    public void setCurrentHouseLevel(int houseLevel)
    {
        _currentHouseLevel = houseLevel;
    }
    public int getCurrentHouseLevel()
    {
        return _currentHouseLevel;
    }
    public int getCurrentPrice()
    {
        if (_price.Length > 0)
        return _price[_currentHouseLevel];
        return 0;
    }
    public int getCurrentVisitPrice()
    {
        return _visitPrice[_currentHouseLevel];
    }
    public void setOwnerIdx(int idx)
    {
        _ownerIdx = idx;
    }
    public int getOwnerIdx()
    {
        return _ownerIdx;
    }
    public void playHighLightColor(float showTime)
    {
        Calculate();
        _isShow = true;
        StartCoroutine(StartWait(showTime));
    }
    public void stopHighLightColor()
    {
        _isShow = false;
    }

    public void playHighLightColor()
    {
        Calculate();
        _isShow = true;
    }

    public string getPlaceName()
    {
        return _placeName;
    }

    public int upgradeHouse(int ownerIdx)
    {
        Debug.Log("Upgrade house");
        if (_ownerIdx == -1)
        {
            _ownerIdx = ownerIdx;
            Debug.Log(_ownerIdx);
            //Gắn cờ _flags cho vùng đất mới mua
            _currentFlag = Instantiate(_flags[_ownerIdx], _tempateModels[4].transform.position, _tempateModels[4].transform.rotation, this.gameObject.transform);
        }
        else
        {
            //Nếu còn xây được
            if (_currentHouseLevel < 4)
            {
                //Chưa có ai xây
                if (_currentHouseLevel == 0)
                {
                    //_ownerIdx = ownerIdx;
                    //Gỡ cờ
                    Destroy(_currentFlag);
                    _currentHouse = Instantiate(_houseModels[4 * ownerIdx + _currentHouseLevel], _tempateModels[_currentHouseLevel].transform.position, _tempateModels[_currentHouseLevel].transform.rotation, this.gameObject.transform);
                    //if (_effect == null)
                    //    _effect = Instantiate(_upgradeEffect, _tempateModels[1].transform.position, _tempateModels[1].transform.rotation, this.gameObject.transform);
                    //else _effect.GetComponent<ParticleSystem>().Play(true);
                    //_upgradeEffect.GetComponent<ParticleSystem>().Play(true);
                    _currentHouseLevel++;
                }
                else
                {
                    //Đã có nhà sẵn
                    //Gỡ bỏ nhà cũ
                    Destroy(_currentHouse);
                    //Xây nhà mới
                    _currentHouse = Instantiate(_houseModels[4 * ownerIdx + _currentHouseLevel], _tempateModels[_currentHouseLevel].transform.position, _tempateModels[_currentHouseLevel].transform.rotation, this.gameObject.transform);
                    //_effect = Instantiate(_upgradeEffect, _tempateModels[1].transform.position, _tempateModels[1].transform.rotation, this.gameObject.transform);
                    //_upgradeEffect.GetComponent<ParticleSystem>().Play(true);
                    _currentHouseLevel++;
                }
                if (_effect != null) Destroy(_effect);
                _effect = Instantiate(_upgradeEffect, _tempateModels[3].transform.position, _tempateModels[3].transform.rotation, this.gameObject.transform);
                return 1;
            }
            else return 0;
        }

        if (_effect != null) Destroy(_effect);
        _effect = Instantiate(_upgradeEffect, _tempateModels[3].transform.position, _tempateModels[3].transform.rotation, this.gameObject.transform);
        return 1;
    }
    public void destroyHouse()
    {
        if (_currentFlag != null) Destroy(_currentFlag);
        if (_currentHouse != null) Destroy(_currentHouse);
        if (_effect != null) Destroy(_effect);
        if (_dEffect != null) Destroy(_dEffect);
        _dEffect = Instantiate(_destroyEffect, _tempateModels[3].transform.position, _tempateModels[3].transform.rotation, this.gameObject.transform);
        //else Instantiate(_destroyEffect, _tempateModels[3].transform.position, _tempateModels[3].transform.rotation, this.gameObject.transform);
        _currentHouseLevel = 0;
        _ownerIdx = -1;
    }
}
