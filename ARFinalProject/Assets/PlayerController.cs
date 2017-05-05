using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public int _defaultMoney = 20000;
	public int _defaultPosition = 0;
	public int _playerNum = 4;
	public GameObject _gameObjectInBoard;
	public GameObject[] _playerPrefab;
	public Transform[] _places;
	public Player[] _players = new Player[4];
	public float _totalEffectTime = 0.2f;

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

	// Move player by jumping
	private float firingAngle = 45.0f;
	private float gravity = 9.8f; 
	IEnumerator MovePlayerInCurve()
	{
		// Calcute number player have the same position with current player to set transparent
		int cnt = 1;
		for (int i = 0; i < _playerNum; i++) { 
			if (_players[i]._position == _players[_playerTurnIdx]._position)
				cnt++;
		}
		if (cnt > 1) {
			SetPlayerTransparent(_playerTurnIdx, 1f);
			if (cnt == 2) {
				for (int i = 0; i < _playerNum; i++) {
					if (i != _playerTurnIdx && _players[i]._position == _players[_playerTurnIdx]._position)  {
						SetPlayerTransparent(i, 1f);
						break;
					}
				}
			}
		}

		// Move player with _stepNum steps
		for (int i = 0; i < _stepNum; i++)
		{
			int idxPositionInBoard = _players[_playerTurnIdx]._position;
			//Debug.Log ("position in board: " + idxPositionInBoard.ToString ());
			float target_Distance = Vector3.Distance(_places[idxPositionInBoard].localPosition, _places[_players[_playerTurnIdx].GetNextPosition()].localPosition);

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
					_players [_playerTurnIdx]._player.transform.localPosition = _places [_players [_playerTurnIdx].GetNextPosition ()].localPosition + deltaPlayerPosition[_playerTurnIdx];
				yield return null;
			}

			yield return new WaitForSeconds(0.1f);

			_players[_playerTurnIdx].IncreasePosition();
			idxPositionInBoard = _players[_playerTurnIdx]._position;
			// Rotate player to look at camera
			if (idxPositionInBoard == 18)
				_players[_playerTurnIdx]._player.transform.localEulerAngles = new Vector3(0f, -120f, 0f);
			else if (_players[_playerTurnIdx]._position == 0)
				_players[_playerTurnIdx]._player.transform.localEulerAngles = new Vector3(0f, -150f, 0f);
		}


		// Calcute number player have the same position with current player to set transparent after finished moving
		for (int i = 0; i < _playerNum; i++)
		{
			if (i != _playerTurnIdx && _players[i]._position == _players[_playerTurnIdx]._position)
			{
				SetPlayerTransparent (_playerTurnIdx, .2f);
				break;
			}
		}

		_stepNum = 0;
		_playerTurnIdx = GetNextPlayTurnIdx();
		GetComponent<DiceEventHandler>().SetIsMovePlayer(_isMovePlayer);

		// Moving player is finished.
		// Do anything else....
		//....
	} 

	// Move Player Linearly. Replaced by MovePlayerInCurve
	IEnumerator MovePlayer (float time)
	{
		Debug.Log ("Move player in update");
		int cnt = 1;

		for (int i = 0; i < _playerNum; i++) {
			if (_players [i]._position == _players [_playerTurnIdx]._position)
				cnt++;
		}

		if (cnt > 1) {
			_players [_playerTurnIdx]._player.transform.GetComponent<MeshRenderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

			if (cnt == 2) {
				for (int i = 0; i < _playerNum; i++) {
					if (i != _playerTurnIdx && _players [i]._position == _players [_playerTurnIdx]._position) {
						_players [i]._player.transform.GetComponent<MeshRenderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
						break;
					}
				}
			}
		}

		for (int i = 0; i < _stepNum; i++) {
			Vector3 diff = _places[_players [_playerTurnIdx].GetNextPosition()].localPosition - _places[_players [_playerTurnIdx]._position].localPosition;
			_players [_playerTurnIdx]._player.transform.localPosition += diff;
			_players [_playerTurnIdx].IncreasePosition ();
			if (_players [_playerTurnIdx]._position == 18) {
				_players [_playerTurnIdx]._player.transform.localEulerAngles = new Vector3 (0f, -120f, 0f);
			} else if (_players [_playerTurnIdx]._position == 0) {
				_players [_playerTurnIdx]._player.transform.localEulerAngles = new Vector3 (0f, -150f, 0f);
			}
			yield return new WaitForSeconds(.2f); 
		}

		for (int i = 0; i < _playerNum; i++) {
			if (i != _playerTurnIdx && _players [i]._position == _players [_playerTurnIdx]._position) {
				SetPlayerTransparent(_playerTurnIdx, .2f);
				break;
			}
		}


		_stepNum = 0;
		_playerTurnIdx = GetNextPlayTurnIdx();
		GetComponent<DiceEventHandler>().SetIsMovePlayer(_isMovePlayer);
	}

	// Set transparent for Player
	void SetPlayerTransparent(int playerId, float val)
	{
		_players[playerId]._player.transform.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, val);
	}

	// Dummy code. Haven't handle order of players yet.
	int GetNextPlayTurnIdx ()
	{
		return (_playerTurnIdx + 1) % _playerNum;
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
			playerObj.transform.localPosition = _places [0].transform.localPosition + deltaPlayerPosition[i];
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

	// Player class stores information of Player
	public class Player {
		public GameObject _player;
		public int _position;
		public int _money;

		public Player(GameObject playerObject, int position, int money) {
			_player = playerObject;
			_position = position;
			_money = money;
		}

		public void IncreasePosition() {
			_position = (_position + 1) % 36;
		}

		public int GetNextPosition() {
			return (_position + 1) % 36;
		}
	}
}
