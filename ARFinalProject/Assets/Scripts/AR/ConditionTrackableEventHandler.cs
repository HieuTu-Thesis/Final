using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ConditionTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;
    public static int _type; //0: Build house 1, 1: Sale house 1, 2: Sale house 2
    // Use this for initialization
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
        //transform.gameObject.SetActive(false);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && GameController.GetInstance()._isWaitCardChoiceCityProcess)
        {
            GameController.GetInstance()._isWaitCardChoiceCityProcess = false;
            if (mTrackableBehaviour.TrackableName.Equals("Yes"))
            {
                GameController.GetInstance().hideDialog();
                if (_type == 0) GameController.GetInstance().buildHouseProcess();
                else if (_type == 1) GameController.GetInstance().saleHouseProcess1();
                else if (_type == 2) GameController.GetInstance().saleHouseProcess2();
                
            }else if (mTrackableBehaviour.TrackableName.Equals("No"))
            {
                GameController.GetInstance().hideDialog();
                int delta = 1;
                if (GameController._isSameDice)
                    delta = 0;
                PlayerController.GetInstance().SetPlayerTurnIdx(delta);
                GameController.GetInstance()._shouldThrowDice = true;
                if (_type == 2) GameController.GetInstance().showDialog("Phá sản");
            }
        }
    }
}
