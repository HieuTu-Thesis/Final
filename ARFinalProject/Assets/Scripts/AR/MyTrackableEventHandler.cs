using UnityEngine;
using System.Collections;
using Vuforia;

public class MyTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
	
	private TrackableBehaviour mTrackableBehaviour;
	static public string[] _cardChoiceNames = {"ImageTargetChoiceA","acid","ImageTargetChoiceB", "ImageTargetChoiceC" };

	void Start()
	{
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
		transform.gameObject.SetActive (false);
	}


	public void OnTrackableStateChanged (TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
		
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			if (!GameController.GetInstance ()._isWaitCardChoice)
				return;
			
			Debug.Log (mTrackableBehaviour.TrackableName + " found");
			for (int i = 0; i < _cardChoiceNames.Length; i++) {
				if (mTrackableBehaviour.TrackableName.CompareTo (_cardChoiceNames [i]) == 0) {
					if (GameController.GetInstance ()._isWaitCardChoice)
						GameController.GetInstance ()._cardChoice = i;
				}
			}
		}
		else
		{
			
		}
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}

