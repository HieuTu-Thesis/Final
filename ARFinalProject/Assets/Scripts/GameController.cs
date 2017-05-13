using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	// Bonus money when player pass start cell
	public int _bonusPassStart = 30000;

	// List of Board cells
	public GameObject[] _places;

	public GameObject _helicopterPrefab;
	public GameObject _gameObjectInBoard;

	// type of event
	public const int _valBuildHouse = 0;
	public const int _valOpportunity = 1;
	public const int _valGame = 2;
	public const int _valUnlucky = 3;
	public const int _valGoPrison = 4;
	public const int _valPrison = 5;
	public const int _valAuction = 6;
	public const int _valParkingFee = 7;
	public const int _valTax = 8;

	private int[] _eventAtPosition;
	private bool _isGoPrision = false;

	// Instance of Singleton pattern
	private static GameController gameControllerInstance = null;
	private GameController() {
	}

	//public static GameController 


	void Awake()
	{
		if (gameControllerInstance != null && gameControllerInstance != this)
		{
			Destroy(this.gameObject);
		} else {
			gameControllerInstance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		gameControllerInstance.InitEventTypeAtPosition();
	}

	// Update is called once per frame
	void Update () {

	}

	public static int GetPlacesNum() {
		int len = gameControllerInstance._places.Length;
		return len;
	}

	public static void HandleSpecialEvent(int type) {

	}

	public static void HandleEventAtPosition(int position, int stepMoved) {
		if (position - stepMoved < 0)
			gameControllerInstance.PassStart ();
		
		int type = gameControllerInstance._eventAtPosition [position];
		switch (type) {
		case _valBuildHouse:
			{
				// Sự kiện người chơi có thể mua nhà, hoặc đóng phí khi vào nhà xử lý tại đây
				// Muốn lấy thông tin  của người chơi hiện tại thì dùng  PlayerController.GetInstance ()  -> Get money, position,...
				break;
			}
		case _valGoPrison:
			gameControllerInstance.GoPrison ();
			break;
		}
		PlayerController.GetInstance ().SetPlayerTurnIdx (1);
	}

	public void PassStart(){
		PlayerController.GetInstance ().IncreaseMoney (_bonusPassStart);
	}

	private GameObject _helicopter;
	public void GoPrison() {
		_helicopter = Instantiate (_helicopterPrefab, _gameObjectInBoard.transform);
		int goPrisonPosition = 35;
		_isGoPrision = true;
		_helicopter.transform.localPosition = _places [goPrisonPosition].transform.localPosition + new Vector3 (0f, 0.2f, 0f);
		PlayerController.GetInstance ().HidePlayer (true);
		StartCoroutine(MoveHelicopterInCurve());
		PlayerController.GetInstance ().HidePlayer (false);
	}

	public void Unlucky() {

	}

	public void Opportunity() {

	}

	public static Transform GetTransformPlaceAtIndex(int idx) {
		return gameControllerInstance._places [idx].transform;
	}

	private void InitEventTypeAtPosition() {
		_eventAtPosition = new int[_places.Length];

		for (int i = 0; i < _places.Length; i++) {
			_eventAtPosition [i] = GetTypeOfEvent (_places [i].name);
		}
	}

	private int GetTypeOfEvent(string name) {
		if (name.CompareTo ("Opportunity1") == 0 || name.CompareTo ("Opportunity2") == 0)
			return _valOpportunity;
		if (name.CompareTo ("Unlucky1") == 0 || name.CompareTo ("Unlucky2") == 0)
			return _valUnlucky;
		if (name.CompareTo ("Game") == 0)
			return _valGame;
		if (name.CompareTo ("GoPrison") == 0)
			return _valGoPrison;
		if (name.CompareTo ("Prison") == 0)
			return _valPrison;
		if (name.CompareTo ("CoachStation") == 0 || name.CompareTo ("Parking") == 0 || name.CompareTo ("Airport") == 0 || name.CompareTo ("TDDK") == 0
		    || name.CompareTo ("TDDL") == 0|| name.CompareTo ("TDVT") == 0)
			return _valAuction;
		if (name.CompareTo ("Tax") == 0)
			return _valTax;
		if (name.CompareTo ("Parking") == 0)
			return _valParkingFee;
		
		return _valBuildHouse;
	}



	IEnumerator MoveHelicopterInCurve()
	{
		float firingAngle = 60.0f;
		float gravity = 9.8f/4; 
		float target_Distance = Vector3.Distance(_places [9].transform.localPosition, _helicopter.transform.localPosition);

		// Calculate the velocity needed to throw the object to the target at specified angle.
		float velocity = Mathf.Sqrt(target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity));
		float Vx = 0, Vz = 0;
		Vz = velocity * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = velocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

		// Calculate flight time.
		float flightDuration = target_Distance / Vz;
		Vx = (_places [9].transform.localPosition.x - _helicopter.transform.localPosition.x) / flightDuration;
		float elapse_time = 0;

		// jumping curvely
		while (elapse_time <= flightDuration)
		{
			_helicopter.transform.localPosition += new Vector3(Vx * Time.deltaTime, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vz * Time.deltaTime);
			elapse_time += Time.deltaTime;
			if (elapse_time > flightDuration)
				_helicopter.transform.localPosition = _places [9].transform.localPosition;
			yield return null;
		}

		yield return new WaitForSeconds(0.1f);
		Destroy (_helicopter, 1.0f);

		_isGoPrision = false;
	} 
}
