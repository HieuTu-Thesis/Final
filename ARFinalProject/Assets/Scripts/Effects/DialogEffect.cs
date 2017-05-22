using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEffect : MonoBehaviour {

    private bool _isShow;
    private bool _isStart;
    public float _startY;
    public float _endY;
	// Use this for initialization
	void Start () {
        _isShow = false;
        _isStart = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (_isShow && _isStart)
        {
            if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.y >= _endY)
                this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(this.gameObject.GetComponent<RectTransform>().localPosition.x, this.gameObject.GetComponent<RectTransform>().localPosition.y - 5, this.gameObject.GetComponent<RectTransform>().localPosition.z);
        }else if (_isStart)
        {
            if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.y <= _startY)
                this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(this.gameObject.GetComponent<RectTransform>().localPosition.x, this.gameObject.GetComponent<RectTransform>().localPosition.y + 5, this.gameObject.GetComponent<RectTransform>().localPosition.z);
            else
            {
                _isStart = false;
                //this.gameObject.GetComponent<RectTransform>().anchoredPosition.y = _startY;
            }
        }
        
	}
    public void showDialog()
    {
        _isShow = true;
        _isStart = true;
    }

    public void hideDialog()
    {
        _isShow = false;
        _isStart = true;
    }
}
