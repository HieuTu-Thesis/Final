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
        if (Input.GetKeyDown(KeyCode.A))
        {
            _wheel.GetComponent<WheelRotate>()._speed = Random.Range(200, 400);
            _wheel.GetComponent<WheelRotate>()._isStart = true;
            StartCoroutine(StopPot(8F));
        }
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
        string value = "";
        int valueNumber = 0;
        if (_wheel.GetComponent<WheelRotate>()._speed > 0) StartCoroutine(StopPot(0.1F));
        else
        {
            Debug.Log("xxx");
            if (_wheel.transform.localRotation.eulerAngles.y >= 265.587)
            {
                valueNumber = _values[(int)((_wheel.transform.localRotation.eulerAngles.y - 265.587) / ((float)360 / 37))];
            }
            else
            {
                valueNumber = _values[(int)((_wheel.transform.localRotation.eulerAngles.y + 360 - 265.587) / ((float)360 / 37))];
            }
            value = valueNumber.ToString();
            Debug.Log(value);
            if (!value.Contains("-"))
            {
                PlayerController.GetInstance().AddMoneyPlayer(GameController.GetInstance()._currentTurnIdx, valueNumber);
                GameController.GetInstance().showDialogInSeconds("Chúc mừng bạn nhận được " + value + "000$", 5F);
                Debug.Log("CCCCCC");
            }
            else
            {
                if (GameController.GetInstance()._currentPlayerMoney >= -valueNumber * 1000)
                {
                    PlayerController.GetInstance().AddMoneyPlayer(GameController.GetInstance()._currentTurnIdx, valueNumber);
                    GameController.GetInstance().showDialogInSeconds("Bạn bị mất " + value + "000$", 5F);
                }
                   
                else if (GameController.GetInstance().evaluateAsset(-valueNumber * 1000, 2) == 0)
                {
                    GameController.GetInstance().showDialog("Bạn không đủ tiền mặt, bạn cần " + (-valueNumber - GameController.GetInstance()._currentPlayerMoney).ToString() + "$ để chi trả cho trò chơi. Bạn có muốn bán nhà không?");
                    ConditionTrackableEventHandler._type = 1;
                    GameController.GetInstance()._isWaitCardChoiceCityProcess = true;
                    Debug.Log("BBBBBBB");
                }
                else
                {
                    //Phá sản
                    Debug.Log("AAAAAA");
                    GameController.GetInstance().showDialogInSeconds("Bạn bị phá sản", 5F);
                }
            }
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
