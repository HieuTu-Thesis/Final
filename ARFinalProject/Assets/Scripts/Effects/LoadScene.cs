using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class LoadScene : MonoBehaviour, ITrackableEventHandler
{
    public GameObject start;
    private TrackableBehaviour mTrackableBehaviour; // trackers
    // Use this for initialization
    void Start()
    {
        start.SetActive(false);
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTrackableStateChanged(
       TrackableBehaviour.Status previousStatus,
       TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
           newStatus == TrackableBehaviour.Status.TRACKED ||
           newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            start.SetActive(true);
            StartCoroutine(StartWait(16F));
        }
        else
        {

        }
    }
    IEnumerator StartWait(float time)
    {
        yield return StartCoroutine(Wait(time));
        start.SetActive(false);
    }

    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
