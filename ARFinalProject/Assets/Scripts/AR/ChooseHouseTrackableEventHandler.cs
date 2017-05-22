using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ChooseHouseTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;
    public float _waitingTime;
    private int _seconds;
    // Use this for initialization
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
        _seconds = 0;
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && GameController.GetInstance()._isWaitCardChoiceSaleHouse)
        {
            //Counting time
            StartCoroutine(StartWait(1F));
        }
    }

    IEnumerator StartWait(float time)
    {
        yield return new WaitForSeconds(time);
        _seconds++;
        //Show second in 3D text
        if (_seconds < _waitingTime) StartCoroutine(StartWait(1F));
    }
}
