using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handling event throw dices
public class DiceEventHandler : MonoBehaviour {
	public GameObject _particle;
	public GameObject _innerBackground;
	public GameObject[] _dicePrefabs;
	public GameObject _diceBoard;
    public GameObject _board;
	public float _forceAmount = 15.0f; // Force
	public float _torqueAmount = 15.0f; // Momen
	public ForceMode _forceMode;

	private GameObject[] _dices; // dices
	private Rigidbody[] _rgBodys; // Rigid bodys of dices
	private bool _isThrowDice = true; // To check 
	private int _sumDiceValue = 0; // Total value of dices

	private bool _isMovePlayer; // state: if player is moving or not
	private int DEBUG_CELL = 13;
	private bool DICESAME = false;

	void Start() {
		_diceBoard.SetActive (false);
		_rgBodys = new Rigidbody[_dicePrefabs.Length];
		_dices = new GameObject[_dicePrefabs.Length];
		_isMovePlayer = false;
	}

	public void SetIsMovePlayer(bool val) {
		_isMovePlayer = val;
	}

	public void ThrowDice() {
		_innerBackground.SetActive (false);
		_diceBoard.SetActive (true);
		_isThrowDice = true;
		for (int i = 0; i < _dicePrefabs.Length; i++) {
			_dices [i] = Instantiate (_dicePrefabs [i], _board.transform);
            _dices[i].transform.localPosition = new Vector3(_dices[i].transform.localPosition.x, 15f, _dices[i].transform.localPosition.z);
			_rgBodys [i] = _dices [i].GetComponent<Rigidbody> ();
			if (_rgBodys == null)
				Debug.Log ("null body");
			_rgBodys [i].velocity = new Vector3 (0.1f, 0.1f, 0.1f);
			_rgBodys [i].transform.Rotate (Random.Range (-myScale, myScale), Random.Range (-myScale, myScale), Random.Range (-myScale, myScale));
			_rgBodys [i].AddForce (new Vector3 (3.0f, 0.0f, 0.0f) * _forceAmount, _forceMode);
			_rgBodys [i].AddTorque (GetRandomVector3 (_torqueAmount), _forceMode);
		}
	}

	void Update () {
		if (_isMovePlayer) {

		}
		else {
			if (_isThrowDice) {
				_sumDiceValue = 0;
				bool isFail = false;
				bool isAllDicesStop = true;
				for (int i = 0; i < _dicePrefabs.Length; i++) {
					if (_rgBodys [i] == null) {
						isFail = true;
						break;
					}
				
					if (_rgBodys [i].velocity.magnitude >= 0.00001f) {
						isAllDicesStop = false;
						break;
					}
				}

				// Process to get dice value
				if (!isFail && isAllDicesStop) {
					int[] valueDices = { 0, 0 };
					for (int i = 0; i < _dicePrefabs.Length; i++) {
						//Debug.Log ("Before normal DiceRotation[" + i.ToString () + "]" + _dices [i].transform.rotation.eulerAngles.ToString ());
						int xRotation = ((int)_dices [i].transform.rotation.eulerAngles.x) % 360;
						int zRotation = ((int)_dices [i].transform.rotation.eulerAngles.z) % 360;
						xRotation = NormalizeRotationAngle (xRotation);
						zRotation = NormalizeRotationAngle (zRotation);

						//Debug.Log ("AfternomalizeDiceRotation[" + i.ToString () + "]: " + xRotation.ToString () + " " + zRotation.ToString ());
						_isThrowDice = false;

						switch (xRotation) {
						case 90:
							valueDices[i] = 6;
							break;	
						case 0:
						case 180:
							if (zRotation == 0)
								valueDices[i] = 5;
							else if (zRotation == 90)
								valueDices[i] = 4;
							else if (zRotation == 180)
								valueDices[i] = 2;
							else if (zRotation == 270)
								valueDices[i] = 3;
							else
								valueDices[i] = 0;
							break;	
						case 270:
							valueDices[i] = 1;
							break;
						}

						Debug.Log ("Value Dice[" + i.ToString () + "] = " + valueDices[i].ToString ());
						_sumDiceValue += valueDices[i];
					}
					_isMovePlayer = true;

					bool same = valueDices [0] == valueDices [1];
					if (DICESAME)
						same = DICESAME;
					if (DEBUG_CELL != -1) {
						GameController.HandleFinishThrowDice (DEBUG_CELL, same);
					}
					else
						GameController.HandleFinishThrowDice (_sumDiceValue, same);

					StartCoroutine (DestroyDice (1.5f));
				}
			}
		}
	}

	// Distroy dice after throw
	IEnumerator DestroyDice(float time) {
		yield return new WaitForSeconds (time);
		for (int i = 0; i < _dicePrefabs.Length; i++) 
			Destroy (_dices[i]);

		_innerBackground.SetActive (true);
		_diceBoard.SetActive (false);
	}

	// Get Random Vector 3 with scale and myScale
	private float myScale = 100f;
	Vector3 GetRandomVector3(float scale) {
		return new Vector3 (scale * Random.Range(-myScale, myScale), scale * Random.Range(-myScale, myScale), scale * Random.Range(-myScale, myScale));
	}

	// Normalize euler angle of dice
	int NormalizeRotationAngle(int value) {
		int[] a = {0, 90, 180, 270};
		int mn = 370;
		int res = 0;
		for (int i = 0; i < 4; i++) {
			int dis = Mathf.Abs (a [i] - value);
			if (dis < mn) {
				mn = dis;
				res = i;
			}
		}
		return a[res];
	}
}
