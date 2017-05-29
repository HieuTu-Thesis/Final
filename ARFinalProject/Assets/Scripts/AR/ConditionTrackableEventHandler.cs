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


	//IEnumerator Wait(float time) {
	//	yield return new WaitForSeconds(time);
	//	GameController.GetInstance().hideDialog();

	//	if (_type == 0) GameController.GetInstance().buildHouseProcess();
	//	else if (_type == 1) GameController.GetInstance().saleHouseProcess1();
	//	else if (_type == 2) GameController.GetInstance().saleHouseProcess2();
	//}

	//void Update() {
	//	/*
	//	if (GameController.GetInstance ()._isWaitCardChoiceCityProcess && !GameController.GetInstance().AR_MODE_ON) {
	//		GameController.GetInstance()._isWaitCardChoiceCityProcess = false;
	//		StartCoroutine (Wait (2.0f));
	//	}*/
	//}

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) )
        {
            Debug.Log("Found " + mTrackableBehaviour.TrackableName);
            if (GameController.GetInstance()._isWaitCardChoice)
            {
                if (mTrackableBehaviour.TrackableName.CompareTo("Yes") == 0)
                {
                    Debug.Log("Run YES");
                    GameController.GetInstance()._cardChoice = 3;
                }
                else if (mTrackableBehaviour.TrackableName.CompareTo("No") == 0)
                {
                    Debug.Log("Run NO");
                    GameController.GetInstance()._cardChoice = 4;
                }
                return;
            }

            if (!GameController.GetInstance()._isWaitCardChoiceCityProcess)
                return;

            GameController.GetInstance()._isWaitCardChoiceCityProcess = false;
            if (mTrackableBehaviour.TrackableName.Equals("Yes"))
            {
                GameController.GetInstance().hideDialog();
                if (_type == 0) GameController.GetInstance().buildHouseProcess();
                else if (_type == 1) GameController.GetInstance().saleHouseProcess();
                
            }else if (mTrackableBehaviour.TrackableName.Equals("No"))
            {
                GameController.GetInstance().hideDialog();
                int delta = 1;
                if (GameController._isSameDice)
                    delta = 0;
                PlayerController.GetInstance().SetPlayerTurnIdx(delta); 
                //Temporary code
                //TO DO: Xử lý phá sản
                if (_type == 2)
                {
                    GameController.GetInstance().showDialogInSeconds("Rất tiếc, bạn đã phá sản", 3F);
                }
                else GameController.GetInstance()._shouldThrowDice = true;
            }
        }
    }
}
