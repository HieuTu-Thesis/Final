using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotRotate : MonoBehaviour
{
    public bool _isStop;
    public int _choice;
    public bool _isStart;
    private float[] _degreesList;

    // Use this for initialization
    void Start()
    {
        _isStop = false;
        _isStart = false;
        _degreesList = new float[] { 27f, 70f, 297f, 350f };

    }

    // Update is called once per frame
    void Update()
    {

        if (_isStart && !(_isStop && Mathf.Abs(transform.localRotation.x - (float)(_degreesList[_choice] / 3600)) < (0.01)))
        {
            transform.Rotate(-Vector3.up, 300 * Time.deltaTime);
        }
        else if (_isStop && Mathf.Abs(transform.localRotation.x - (float)(_degreesList[_choice] / 3600)) < (0.01))
        {
            transform.localEulerAngles = new Vector3(_degreesList[_choice], 90f, 90f);
            _isStart = false;
            //Check result
            if (transform.parent.Find("Pot1").GetComponent<PotRotate>()._isStart == false && transform.parent.Find("Pot2").GetComponent<PotRotate>()._isStart == false && transform.parent.Find("Pot3").GetComponent<PotRotate>()._isStart == false)
            {
                if (transform.parent.Find("Pot1").GetComponent<PotRotate>()._choice == transform.parent.Find("Pot2").GetComponent<PotRotate>()._choice && transform.parent.Find("Pot1").GetComponent<PotRotate>()._choice == transform.parent.Find("Pot3").GetComponent<PotRotate>()._choice)
                {
                    Debug.Log("Congratulation!");
                    PlayerController.GetInstance().AddMoneyPlayer(GameController.GetInstance()._currentTurnIdx, 100000);
                    GameController.GetInstance().showDialogInSeconds("Chúc mừng bạn nhận được " + 100 + "000$", 5F);
                }
                else
                {
                    Debug.Log("Failed");
                    PlayerController.GetInstance().AddMoneyPlayer(GameController.GetInstance()._currentTurnIdx, -5000);
                    //GameController.GetInstance().showDialogInSeconds("Bạn bị mất " + "5000$", 5F);
                    //GameController.GetInstance().evaluateAsset(5000, 2);
                    if (GameController.GetInstance()._currentPlayerMoney >= 5000)
                    {
                        PlayerController.GetInstance().AddMoneyPlayer(GameController.GetInstance()._currentTurnIdx, -5000);
                        GameController.GetInstance().showDialogInSeconds("Bạn bị mất 5000$", 5F);
                    }
                      
                    else if (GameController.GetInstance().evaluateAsset(5000, 2) == 0)
                    {
                        GameController.GetInstance().showDialog("Bạn không đủ tiền mặt, bạn cần " + (5000 - GameController.GetInstance()._currentPlayerMoney).ToString() + " để chi trả cho trò chơi. Bạn có muốn bán nhà không?");
                        ConditionTrackableEventHandler._type = 1;
                        GameController.GetInstance()._isWaitCardChoiceCityProcess = true;
                    }
                    else
                    {
                        //Phá sản
                        GameController.GetInstance().showDialogInSeconds("Bạn bị phá sản", 5F);
                    }
                }
            }
        }
    }
}