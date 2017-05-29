using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class JackPot : MonoBehaviour, IVirtualButtonEventHandler {
    public GameObject[] _jackPots;
    private int _k;
    private GameObject _vButton;
    // Use this for initialization
    void Start () {
        _k = 0;
        _vButton = GameObject.Find("VirtualButton");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < 3; i++)
            {
                _jackPots[i].GetComponent<PotRotate>()._isStop = false;
                _jackPots[i].GetComponent<PotRotate>()._isStart = true;
                _jackPots[i].GetComponent<PotRotate>()._choice = Random.Range(0, 3);
                //Debug.Log(_jackPots[i].GetComponent<PotRotate>()._choice);
            }
            _k = 0;
            StartCoroutine(StopPot(8F));
        }
    }
    public void OnButtonPressed (VirtualButtonAbstractBehaviour vb)
    {
        //TO DO: Implement virtual button press handler
    }
    public void OnButtonReleased(VirtualButtonAbstractBehaviour vb)
    {

    }
    public void onClick()
    {
        for (int i = 0; i < 3; i++)
        {
            _jackPots[i].GetComponent<PotRotate>()._isStop = false;
            _jackPots[i].GetComponent<PotRotate>()._isStart = true;
            _jackPots[i].GetComponent<PotRotate>()._choice = Random.Range(0, 3);
            //Debug.Log(_jackPots[i].GetComponent<PotRotate>()._choice);
        }
        _k = 0;
        StartCoroutine(StopPot(8F));
    }
    IEnumerator StopPot(float time)
    {
        yield return StartCoroutine(Wait(time));
        _jackPots[_k++].GetComponent<PotRotate>()._isStop = true;
        if (_k < 3) StartCoroutine(StopPot(3F));
        //Stop pot and continue counting until 3 pots are stopped
    }
    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
