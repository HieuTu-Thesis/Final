using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public int _defaultMoney = 100000;
	public int _defaultPosition = 0;
	public int _playerNum = 4;
	public GameObject _gameObjectInBoard;
	public GameObject[] _playerPrefab;

	public Player[] _players = new Player[4];
	public float _totalEffectTime = 0.2f;

	public int _placesNum;
	public static PlayerController playerControllerInstance = null;


	private bool _isMovePlayer;
	private int _playerTurnIdx;
	private float _moveSpeed;
	private float _rotationSpeed;
	private int _stepNum;
	private Transform _currentPlayerTransform;
	private float deltaXZ = 0.02f;
	private float deltaY = 0.085f;
	private Vector3 deltaPosition = new Vector3 (9f -16f, 109.155f, 5.75f-10f);
	private Vector3[] deltaPlayerPosition = new Vector3[4];

	// Use this for initialization
	void Start () {
		_isMovePlayer = false;
		_playerTurnIdx = 0;
		_stepNum = 0;

		_moveSpeed = 5;
		_rotationSpeed = 5;

		_placesNum = GameController.GetPlacesNum ();

		deltaPlayerPosition [0] = new Vector3 (deltaXZ, deltaY, deltaXZ);
		deltaPlayerPosition [1] = new Vector3 (-deltaXZ, deltaY, deltaXZ);
		deltaPlayerPosition [2] = new Vector3 (deltaXZ, deltaY, -deltaXZ);
		deltaPlayerPosition [3] = new Vector3 (-deltaXZ, deltaY, -deltaXZ);
		StartCoroutine (WaitForInitPlayers (_totalEffectTime));
	}

	IEnumerator WaitForInitPlayers(float time)
	{
		yield return new WaitForSeconds(time);
		PlayerController pc = PlayerController.GetInstance ();
		if (!pc)
			Debug.Log("Singleton fails");
		else
			pc.InitPlayers ();
	}

	// Init singleton pattern at awake
	void Awake()
	{
		if (playerControllerInstance != null && playerControllerInstance != this)
		{
			Destroy(this.gameObject);
		} else {
			playerControllerInstance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
		
	public Vector3 GetLocalPositionPlace(int idx) {
		return GameController.GetTransformPlaceAtIndex (idx).localPosition;
	}

	public void SetPlayerTurnIdx(int delta) {
		_playerTurnIdx = (_playerTurnIdx + delta) % _playerNum;
	}

	public void IncreaseMoney(int delta) {
		_players [_playerTurnIdx]._money += delta;
	}

	public int GetMoneyCurrentPlayer() {
		return _players [_playerTurnIdx]._money;
	}

	public void HidePlayer(bool val) {
		_players [_playerTurnIdx]._player.SetActive (val);
	}
		
	// Move player by jumping
	private float firingAngle = 45.0f;
	private float gravity = 9.8f; 
	IEnumerator MovePlayerInCurve()
	{
		// Calcute number player have the same position with current player to set transparent
		int cnt = 1;
		for (int i = 0; i < _playerNum; i++) { 
			if (_players[i]._position == GetCurrentPlayerPosition())
				cnt++;
		}
		if (cnt > 1) {
			SetPlayerTransparent(_playerTurnIdx, 1f);
			if (cnt == 2) {
				for (int i = 0; i < _playerNum; i++) {
					if (i != _playerTurnIdx && _players[i]._position == GetCurrentPlayerPosition())  {
						SetPlayerTransparent(i, 1f);
						break;
					}
				}
			}
		}

		// Move player with _stepNum steps
		for (int i = 0; i < _stepNum; i++)
		{
			int idxPositionInBoard = GetCurrentPlayerPosition();
			//Debug.Log ("position in board: " + idxPositionInBoard.ToString ());
			float target_Distance = Vector3.Distance(GetLocalPositionPlace(idxPositionInBoard), GetLocalPositionPlace(GetCurrentPlayerNextPosition()));

			// Calculate the velocity needed to throw the object to the target at specified angle.
			float velocity = Mathf.Sqrt(target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity));
			float Vx, Vz;
			Vx = Vz = velocity * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
			float Vy = velocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

			// Calculate flight time.
			float flightDuration = target_Distance / Vx;

			float elapse_time = 0;
			if (idxPositionInBoard >= 0 && idxPositionInBoard <= 8) 
				Vx = 0;
			else if (idxPositionInBoard >= 18 && idxPositionInBoard <= 26)
			{
				Vx = 0;
				Vz = -Vz;
			}
			else if (idxPositionInBoard >= 9 && idxPositionInBoard <= 17)
				Vz = 0;
			else
			{
				Vz = 0;
				Vx = -Vx;
			}

			// jumping curvely
			while (elapse_time <= flightDuration)
			{
				_players[_playerTurnIdx]._player.transform.localPosition += new Vector3(Vx * Time.deltaTime, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vz * Time.deltaTime);
				elapse_time += Time.deltaTime;
				if (elapse_time > flightDuration)
					_players [_playerTurnIdx]._player.transform.localPosition = GetLocalPositionPlace(GetCurrentPlayerNextPosition()) + deltaPlayerPosition[_playerTurnIdx];
				yield return null;
			}

			yield return new WaitForSeconds(0.1f);

			_players[_playerTurnIdx].IncreasePosition();
			idxPositionInBoard = GetCurrentPlayerPosition();
			// Rotate player to look at camera
			if (idxPositionInBoard == 18)
				_players[_playerTurnIdx]._player.transform.localEulerAngles = new Vector3(0f, -120f, 0f);
			else if (GetCurrentPlayerPosition() == 0)
				_players[_playerTurnIdx]._player.transform.localEulerAngles = new Vector3(0f, -150f, 0f);
		}


		// Calcute number player have the same position with current player to set transparent after finished moving
		for (int i = 0; i < _playerNum; i++)
		{
			if (i != _playerTurnIdx && _players[i]._position == GetCurrentPlayerPosition())
			{
				SetPlayerTransparent (_playerTurnIdx, .2f);
				break;
			}
		}


		GetComponent<DiceEventHandler>().SetIsMovePlayer(_isMovePlayer);

		// Moving player is finished.
		// Do anything else....
		//....
		GameController.HandleEventAtPosition(GetCurrentPlayerPosition(), _stepNum);
		//_playerTurnIdx = GetNextPlayTurnIdx();
		_stepNum = 0;
	} 



	// Set transparent for Player
	void SetPlayerTransparent(int playerId, float val)
	{
	//	_players[playerId]._player.transform.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, val);
	//	_players[playerId]._player.transform.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, val);
	}

	// Dummy code. Haven't handle order of players yet.
	int GetNextPlayTurnIdx ()
	{
		return (_playerTurnIdx + 1) % _playerNum;
	}

	// Get position of current player in board. Value [0..35]
	int GetCurrentPlayerPosition() {
		return _players[_playerTurnIdx]._position;
	}

	// Get next position of current player in board. Value [0..35]
	int GetCurrentPlayerNextPosition() {
		return _players[_playerTurnIdx].GetNextPosition ();
	}

	// Update is called once per frame
	void Update () {
		if (_isMovePlayer) {
			Debug.Log ("Step num: " + _stepNum.ToString ());
			StartCoroutine(MovePlayerInCurve());
			_isMovePlayer = false;
		}
	}

	// Singleton pattern for PlayerController
	private PlayerController(){}
	public static PlayerController GetInstance() {			
		return playerControllerInstance;
	}

	// Init Player when start
	public void InitPlayers() {
		for (int i = 0; i < _playerNum; i++) {
			GameObject playerObj = Instantiate(_playerPrefab[i], _gameObjectInBoard.transform);
			playerObj.transform.localPosition = GetLocalPositionPlace(0) + deltaPlayerPosition[i];
			Player player = new Player (playerObj, _defaultPosition, _defaultMoney);
			_players[i] = player;
		}
	}

	// Get Player with index in players
	public Player GetPlayer(int index) {
		if (index >= _playerNum)
			return null;
		return _players [index];
	}

	// Method will be called when dices stop
	public void MovePlayer(int stepNum) {
		Debug.Log ("Move player");
		_stepNum = stepNum;
		_isMovePlayer = true;
	}
}
