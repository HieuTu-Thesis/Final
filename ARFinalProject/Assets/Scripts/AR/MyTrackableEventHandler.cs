using UnityEngine;
using System.Collections;
using Vuforia;

public class MyTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
	
	private TrackableBehaviour mTrackableBehaviour;
	static public string[] _cardChoiceNames = {"A","B", "C" };

	void Start()
	{
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
	}


	public void OnTrackableStateChanged (TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
		
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
            Debug.Log(mTrackableBehaviour.TrackableName);
            if (GameController.GetInstance()._isWaitInit)
            {
                if (mTrackableBehaviour.TrackableName.CompareTo("Board") == 0)
                {
                    GameController.GetInstance()._isWaitInit = false;
                    OnTrackingFound();
                    return;
                }
            }

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

    private void OnTrackingFound()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = true;
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
    }

    // Update is called once per frame
    void Update ()
	{
	
	}
}

