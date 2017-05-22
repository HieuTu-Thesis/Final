using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Roulette : MonoBehaviour, IVirtualButtonEventHandler
{
    public GameObject _wheel;
    public Material _lightOn;
    public Material _lightOff;
    private GameObject[] _bulbs;
    private GameObject _vButton;
    private int[] _values;

    // Use this for initialization
    void Start()
    {
        _vButton = GameObject.Find("VirtualButton");
        _values = new int[] { 0, -26, 3, -35, 12, -28, 7, -29, 18, -22, 9, -31, 14, -20, 1, -33, 16, -24, 5, -10, 23, -8, 30, -11, 36, -13, 27, -6, 34, -17, 25, -2, 21, -4, 19, -15, 32 };
        _bulbs = GameObject.FindGameObjectsWithTag("Bulb");
        StartCoroutine(LightUp(1F));
    }

    // Update is called once per frame
    void Update()
    {
        //Bulb Show

    }
    public void OnButtonPressed(VirtualButtonAbstractBehaviour vb)
    {
        //TO DO: Implement virtual button press handler
    }
    public void OnButtonReleased(VirtualButtonAbstractBehaviour vb)
    {

    }
    public void onClick()
    {
        _wheel.GetComponent<WheelRotate>()._speed = Random.Range(200, 400);
        _wheel.GetComponent<WheelRotate>()._isStart = true;
        StartCoroutine(StopPot(8F));
    }
    IEnumerator StopPot(float time)
    {
        yield return StartCoroutine(Wait(time));

        _wheel.GetComponent<WheelRotate>()._speed -= 1;
        if (_wheel.GetComponent<WheelRotate>()._speed > 0) StartCoroutine(StopPot(0.1F));
        else
        {
            if (_wheel.transform.rotation.eulerAngles.y >= 265.587) Debug.Log(_values[(int)((_wheel.transform.rotation.eulerAngles.y - 265.587) / ((float)360 / 37))]);
            else Debug.Log(_values[(int)((_wheel.transform.rotation.eulerAngles.y + 360 - 265.587) / ((float)360 / 37))]);
        }
        //Stop pot and continue counting until 3 pots are stopped
    }
    IEnumerator LightUp(float time)
    {
        yield return StartCoroutine(Wait(time));
        for (int i = 0; i < 13; i++)
        {
            if (_bulbs[i].GetComponent<MeshRenderer>().material.color == _lightOn.color) _bulbs[i].GetComponent<MeshRenderer>().material.color = _lightOff.color;
            else _bulbs[i].GetComponent<MeshRenderer>().material.color = _lightOn.color;
        }
        StartCoroutine(LightUp(1F));
    }
        IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
