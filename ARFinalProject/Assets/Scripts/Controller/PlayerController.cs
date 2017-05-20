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

	public int _placesNum;
	public static PlayerController playerControllerInstance = null;


	private bool _isMovePlayer;
	public int _playerTurnIdx;
	private int _stepNum;
	private Transform _currentPlayerTransform;
	private float deltaXZ = 0f;//0.15f;
	private float deltaY = 0.09f;

	private Vector3[] deltaPlayerPosition = new Vector3[4];
	private Vector3[] playerFaceDirection = new Vector3[2];

	private const int MOVE_FORWARD = 1;
	private const int MOVE_BACKWARD = -1;



	// Use this for initialization
	void Start () {
		_isMovePlayer = false;
		_playerTurnIdx = 0;
		_stepNum = 0;
		_placesNum = GameController.GetPlacesNum ();

		deltaPlayerPosition [0] = new Vector3 (deltaXZ, deltaY, deltaXZ);
		deltaPlayerPosition [1] = new Vector3 (-deltaXZ, deltaY, deltaXZ);
		deltaPlayerPosition [2] = new Vector3 (deltaXZ, deltaY, -deltaXZ);
		deltaPlayerPosition [3] = new Vector3 (-deltaXZ, deltaY, -deltaXZ);

		//playerFaceDirection[0] = new Vector3(0f, -120f, 0f);
		//playerFaceDirection[1] = new Vector3(0f, -150f, 0f);
		playerFaceDirection[0] = playerFaceDirection[1] = new Vector3(0f, -0f, 0f);
	}

	private float deltaCorner = 0.6f;
	private float deltaEdge = 0.6f;
	public Vector3 GetDeltaPosition(int idxPlayer, int position) {
		Vector3 res = deltaPlayerPosition [idxPlayer];
		if (position == 0)
			res += new Vector3 (deltaCorner, 0f, deltaCorner);
		else if (position == 9)
			res += new Vector3 (deltaCorner, 0f, -deltaCorner);
		else if (position == 18)
			res += new Vector3 (-deltaCorner, 0f, -deltaCorner);
		else if (position == 27)
			res += new Vector3 (-deltaCorner, 0f, deltaCorner);
		else if (1 <= position && position <= 8)
			res += new Vector3 (deltaEdge, 0f, 0f);
		else if (10 <= position && position <= 17)
			res += new Vector3 (0f, 0f, -deltaEdge);
		else if (19 <= position && position <= 26)
			res += new Vector3 (-deltaEdge, 0f, 0f);
		else if (28 <= position && position <= 35)
			res += new Vector3 (0f, 0f, deltaEdge);
		
		return res;
	}

	// Update is called once per frame
	void Update () {
		if (_isMovePlayer) {
			Debug.Log ("Step num: " + _stepNum.ToString ());
			StartCoroutine (MovePlayerInCurve (MOVE_FORWARD));
			_isMovePlayer = false;
		}
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

	public void SetLocalPosition(int idxPlayer, int position) {
		_players [idxPlayer]._player.transform.localPosition = GetDeltaPosition (idxPlayer, position) + GameController.GetInstance ()._places [position].transform.localPosition;
	}
		
	public Vector3 GetLocalPositionPlace(int idx) {
		return GameController.GetTransformPlaceAtIndex (idx).localPosition;
	}

	public void SetPlayerTurnIdx(int delta) {
		_playerTurnIdx = (_playerTurnIdx + delta) % _playerNum;
	}

	public bool IncreaseMoney(int delta) {
		_players [_playerTurnIdx]._money += delta;
		if (_players [_playerTurnIdx]._money < 0)
			return false;
		return true;
	}

	public int GetMoneyCurrentPlayer() {
		return _players [_playerTurnIdx]._money;
	}
    public int GetMoneyPlayer(int playerIdx)
    {
        return _players[playerIdx]._money;
    }
    public int SetMoneyPlayer(int playerIdx, int money)
    {
        return _players[_playerTurnIdx]._money = money;
    }
    public void SetActivePlayer(bool val) {
		Ultility.MyDebug ("Hide player", val.ToString ());
		_players [_playerTurnIdx]._player.SetActive (val);
	}

	public void SetActivePlayer(int idxPlayer, bool val) {
		Ultility.MyDebug ("Hide player", val.ToString ());
		_players [idxPlayer]._player.SetActive (val);
	}

	public void SetPosition (int val) {
		_players [_playerTurnIdx]._position = val;
	}

	public void SetPosition (int idxPlayer, int val) {
		_players [idxPlayer]._position = val;
	}

	public int GetCurrentPlayerPrisonLicense() {
		return _players [_playerTurnIdx]._prisonLicense;
	}

	public void ChangeCurrentPlayerPrisonLicense(int delta) {
		_players [_playerTurnIdx]._prisonLicense += delta;
	}

	public void ChangeMovePlayerValue(bool value) {
		_isMovePlayer = value;
		GetComponent<DiceEventHandler>().SetIsMovePlayer(_isMovePlayer);
	}
		
	// Move player by jumping
	// parameters of formula
	private float firingAngle = 60.0f;
	private float gravity = 2 * 9.8f; 
	IEnumerator MovePlayerInCurve(int direction) // direction 1: forward, -1: backward
	{
//		GameController.GetInstance ().EditCamera (_players[_playerTurnIdx]);
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
			int nextPosition = GetPositionOfPlayerWithDelta (_playerTurnIdx, direction);

			if (nextPosition == 0 || nextPosition == 9 || nextPosition == 18 || nextPosition == 27) {
				float rotateAngle = direction * 90f;
				_players [_playerTurnIdx]._player.transform.Rotate (new Vector3 (0f, rotateAngle, 0f));// = new Vector3 (0f, rotateAngle + _players [_playerTurnIdx]._player.transform.localEulerAngles.y, 0f);
			}

			Vector3 targetLocalPosition = GetLocalPositionPlace(GetPositionOfPlayerWithDelta(_playerTurnIdx, direction)) + GetDeltaPosition(_playerTurnIdx, nextPosition);
			Vector3 startLocalPosition = GetLocalPositionPlace (idxPositionInBoard) + GetDeltaPosition(_playerTurnIdx, idxPositionInBoard);
			float target_Distance = Vector3.Distance(startLocalPosition, targetLocalPosition);

			// Calculate the velocity needed to throw the object to the target at specified angle.
			float velocity = Mathf.Sqrt(target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity));

			float Vx, Vz;

			float Vy = velocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
			float elapse_time = 0;
			float flightDuration = 2 * Vy / gravity;
			Vx = (targetLocalPosition.x - startLocalPosition.x) / flightDuration;
			Vz = (targetLocalPosition.z - startLocalPosition.z) / flightDuration;

			// jumping curvely
			while (elapse_time <= flightDuration)
			{
				_players[_playerTurnIdx]._player.transform.localPosition += new Vector3(Vx * Time.deltaTime, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vz * Time.deltaTime);
				elapse_time += Time.deltaTime;
				if (elapse_time > flightDuration)
					_players [_playerTurnIdx]._player.transform.localPosition = targetLocalPosition;
				yield return null;
			}
			GameController.GetInstance ().AddCellAnimation (GetPositionOfPlayerWithDelta(_playerTurnIdx, direction));
			yield return new WaitForSeconds(0.1f);

			_players[_playerTurnIdx].IncreasePosition(direction);
			idxPositionInBoard = GetCurrentPlayerPosition();

			/*
			// Rotate player to look at camera
			if (idxPositionInBoard == 18)
				_players [_playerTurnIdx]._player.transform.localEulerAngles = playerFaceDirection [0];
			else if (idxPositionInBoard == 0)
				_players [_playerTurnIdx]._player.transform.localEulerAngles = playerFaceDirection [1];

			if (direction == -1) {
				if (idxPositionInBoard == 35)
					_players [_playerTurnIdx]._player.transform.localEulerAngles = playerFaceDirection [0];
				else if (idxPositionInBoard == 17)
					_players [_playerTurnIdx]._player.transform.localEulerAngles = playerFaceDirection [1];
			} */
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

		if (_stepNum < 4)
			yield return new WaitForSeconds(1f);

//		GameController.GetInstance ().ResetCamera ();
		GetComponent<DiceEventHandler>().SetIsMovePlayer(_isMovePlayer);

        // Moving player is finished.
        // Do anything else....
        //....
        GameController.GetInstance().HandleEventAtPosition(GetCurrentPlayerPosition(), _stepNum);
		//_playerTurnIdx = GetNextPlayTurnIdx();
		_stepNum = 0;
	} 

	public Player GetCurrentPlayer() {
		return _players [_playerTurnIdx];
	}

	public void MakePlayerGoPrison() {
		_players [_playerTurnIdx]._sameDiceCount = 0;
		_players [_playerTurnIdx]._inPrison = 3;
		_players [_playerTurnIdx]._position = 9; // 9 la position cua nha tu`
		_players [_playerTurnIdx]._player.transform.localPosition = GetLocalPositionPlace (9) + GetDeltaPosition(_playerTurnIdx, 9);
	}

	// Set transparent for Player
	void SetPlayerTransparent(int playerId, float val)
	{
	//	_players[playerId]._player.transform.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, val);
	//	_players[playerId]._player.transform.GetComponentInChildren<SkinnedMeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, val);
	}

	// Dummy code. Haven't handle order of players yet.
	public int GetNextPlayTurnIdx ()
	{
		return (_playerTurnIdx + 1) % _playerNum;
	}

	// Get Position of player with index of player
	int GetPlayerPositionAtIndex(int idx) {
		return _players[idx]._position;
	}

	// Get position of current player in board. Value [0..35]
	int GetCurrentPlayerPosition() {
		return _players[_playerTurnIdx]._position;
	}

	int GetPositionOfPlayerWithDelta(int idxPlayer, int delta) {
		return _players [idxPlayer].GetPositionWithDelta (delta);
	}

	// Get next position of current player in board. Value [0..35]
	int GetCurrentPlayerNextPosition() {
		return _players[_playerTurnIdx].GetNextPosition ();
	}

    public int GetPlayerTurnIdx()
    {
        return _playerTurnIdx;
    }
    public int SetMoneyCurrentPlayer(int money)
    {
        return _players[_playerTurnIdx]._money = money;
    }
    public int AddMoneyPlayer(int playerIdx, int money)
    {
        return _players[playerIdx]._money += money;
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
			playerObj.transform.localPosition = GetLocalPositionPlace(0) + deltaPlayerPosition[i] + GetDeltaPosition(i, 0);
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
		_stepNum = stepNum;
		_isMovePlayer = true;
	}
}
