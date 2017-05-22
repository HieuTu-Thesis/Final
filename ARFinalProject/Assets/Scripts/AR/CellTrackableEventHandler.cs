using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CellTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public GameObject _place;
    private TrackableBehaviour mTrackableBehaviour;
    public bool _needToProcess;
    // Use this for initialization
    void Start () {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
        _needToProcess = false;
        //transform.gameObject.SetActive(false);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && GameController.GetInstance()._isWaitCardChoiceSaleHouse && _needToProcess)
        {
            //GameController.GetInstance()._cardChoiceName = mTrackableBehaviour.TrackableName;
            GameController.GetInstance()._totalSaleMoney += _place.GetComponent<CellUtil>().getCurrentPrice();
            //Destroy house
            _place.GetComponent<CellUtil>().destroyHouse();
            _place.GetComponent<CellUtil>().stopHighLightColor();
            _needToProcess = false;
        }
    }
}
