using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotRotate : MonoBehaviour {
    public bool _isStop;
    public int _choice;
    public bool _isStart;
    private float[] _degreesList;

    // Use this for initialization
    void Start () {
        _isStop = false;
        _isStart = false;
        _degreesList = new float[] { 27f, 70f, 297f, 350f };

    }
	
	// Update is called once per frame
	void Update () {
       
        if (_isStart && !(_isStop && Mathf.Abs(transform.localRotation.x - (float)(_degreesList[_choice] / 3600)) < (0.01)))
        {
            transform.Rotate(-Vector3.up, 300 * Time.deltaTime);
        }else if (_isStop && Mathf.Abs(transform.localRotation.x - (float)(_degreesList[_choice] / 3600)) < (0.01))
        {
            transform.localEulerAngles = new Vector3(_degreesList[_choice], 90f, 90f);
            _isStart = false;
            //Check result
            if (transform.parent.Find("Pot1").GetComponent<PotRotate>()._isStart == false && transform.parent.Find("Pot2").GetComponent<PotRotate>()._isStart == false && transform.parent.Find("Pot3").GetComponent<PotRotate>()._isStart == false)
            {
                if (transform.parent.Find("Pot1").GetComponent<PotRotate>()._choice == transform.parent.Find("Pot2").GetComponent<PotRotate>()._choice && transform.parent.Find("Pot1").GetComponent<PotRotate>()._choice == transform.parent.Find("Pot3").GetComponent<PotRotate>()._choice)
                {
                    Debug.Log("Congratulation!");
                }
                else Debug.Log("Failed");
            }
        }
    }
}