using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// Bonus money when player pass start cell
	public int _bonusPassStart = 30000;

	// List of Board cells
	public GameObject[] _places;

	public GameObject _helicopterPrefab;
	public GameObject _gameObjectInBoard;
	public GameObject _canvas;


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
	private bool _isGoPrison = false;

	public bool _isWaitCardChoice;
	public int _cardChoice = -1;

	private int _typeCurrentEvent = -1;

	private bool _shouldThrowDice = false;

	public float _totalEffectTime = 0.2f;
	public float _startGameTime = 1f;

	public Sprite[] _luckGifs; 
	public Sprite _luckyCard;
	public Sprite _unluckyCard;
	public Sprite[] _unluckyResults;

	public GameObject _ARCamera;
	public GameObject _normalCamera;
	public GameObject[] _cardChoiceTarget;
	public GameObject _fullBoard;
	private bool AR_MODE_ON = true;
	private bool DEBUG_AR = false;

	// Instance of Singleton pattern
	private static GameController gameControllerInstance = null;
	private GameController() {
	}

	public static GameController GetInstance() {
		return gameControllerInstance;
	}

	public void MyDebug(string info, object x) {
		Debug.Log (info + " " + x.ToString ());
	}


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

		StartCoroutine (WaitForInitPlayers (_totalEffectTime));
		StartCoroutine (WaitForFirstThrowDice (_startGameTime));

		if (!AR_MODE_ON) {
			_ARCamera.SetActive (false);
			for (int i = 0; i < _cardChoiceTarget.Length; i++)
				_cardChoiceTarget [i].SetActive (false);
		} else
			_normalCamera.SetActive (false);
	}


	bool isImageA = false;
	// Update is called once per frame
	void Update () {
		//if (_shouldThrowDice) {
		//	_shouldThrowDice = false;
		//	GetComponent<DiceEventHandler> ().ThrowDice ();
		//}

        if (Input.GetKeyDown(KeyCode.A)) GetComponent<DiceEventHandler>().ThrowDice();

        if (_isWaitCardChoice) {
			if (_cardChoice != -1) {
				MyDebug ("Card choice:", _cardChoice);
				if (_typeCurrentEvent == _valOpportunity) {
					RotateUIImage ();
				}
			}
		}
	}

	IEnumerator WaitForFirstThrowDice(float time) {
		yield return new WaitForSeconds(time);
		_shouldThrowDice = true;
	}

	IEnumerator WaitForInitPlayers(float time) {
		yield return new WaitForSeconds(time);
		PlayerController.GetInstance ().InitPlayers ();
	}

	public static int GetPlacesNum() {
		int len = gameControllerInstance._places.Length;
		return len;
	}

	public static void HandleSpecialEvent(int type) {

	}

	public static void HandleFinishMovePlayer() {

	}

	IEnumerator OutPrison() {
		OutPrisonFunc ();
		yield return null; 
		PlayerController.GetInstance ().SetPlayerTurnIdx (1);
	}

	private static bool _isSameDice;

	public static void HandleFinishThrowDice(int sumDiceValue, bool isSame) {
		_isSameDice = isSame;
		Player curPlayer = PlayerController.GetInstance ().GetCurrentPlayer ();
		if (_isSameDice) {
			if (curPlayer._inPrison != -1) {
				Ultility.MyDebug ("Xu ly ra tu", null);
				//-----------
				return;
			}

			if (curPlayer._sameDiceCount == 2) {
				Ultility.MyDebug ("Bi nhot vao tu vi 3 lan xuc xac giong nhau", null);
				gameControllerInstance.GoPrison (PlayerController.GetInstance ().GetCurrentPlayer ()._position);
				return;
			} else {
				curPlayer._sameDiceCount += 1;
			}
		} else  {
			if (curPlayer._inPrison == 0) {
				gameControllerInstance.GetMoneyToOutPrison ();
				return;
			} else if (curPlayer._inPrison > 1){
				curPlayer._inPrison -= 1;
				return;
			}
		}

		PlayerController.GetInstance ().MovePlayer (sumDiceValue);
	}

	public void GetMoneyToOutPrison() {

	}

	public void HandleEventAtPosition(int position, int stepMoved) {
		if (position - stepMoved < 0)
			gameControllerInstance.PassStart ();
		
		gameControllerInstance._typeCurrentEvent = gameControllerInstance._eventAtPosition [position];
		if (gameControllerInstance.AR_MODE_ON && gameControllerInstance.DEBUG_AR)
			gameControllerInstance._typeCurrentEvent = _valOpportunity;
		switch (gameControllerInstance._typeCurrentEvent) {
		case _valBuildHouse:
			{
                    // Sự kiện người chơi có thể mua nhà, hoặc đóng phí khi vào nhà xử lý tại đây
                    // Muốn lấy thông tin  của người chơi hiện tại thì dùng  PlayerController.GetInstance ()  -> Get money, position,...
                    // Sự kiện người chơi có thể mua nhà, hoặc đóng phí khi vào nhà xử lý tại đây
                    // Muốn lấy thông tin  của người chơi hiện tại thì dùng  PlayerController.GetInstance ()  -> Get money, position,...
                    //Nếu ô đất này là của người chơi sở hữu hoặc chưa có ai sở hữu
                    if (_places[position].GetComponent<CellUtil>().getOwnerIdx() == PlayerController.GetInstance().GetPlayerTurnIdx() || _places[position].GetComponent<CellUtil>().getOwnerIdx() == -1)
                    {
                        //Show dialog hỏi ý kiến người chơi
                        //Nếu chọn yes
                        int currentHouseLevel = _places[position].GetComponent<CellUtil>().getCurrentHouseLevel();
                        if (PlayerController.GetInstance().GetMoneyCurrentPlayer() >= _places[position].GetComponent<CellUtil>().getPrice(currentHouseLevel))
                        {
                            //Nếu đủ tiền
                            PlayerController.GetInstance().SetMoneyCurrentPlayer(PlayerController.GetInstance().GetMoneyCurrentPlayer() - _places[position].GetComponent<CellUtil>().getPrice(currentHouseLevel));
                            //Build house
                            if (_places[position].GetComponent<CellUtil>().upgradeHouse(PlayerController.GetInstance().GetPlayerTurnIdx()) == 1)
                            {
                                //Nếu còn có thể xây thêm nhà
                                //Hiện thông báo
                                Debug.Log("Upgrade");
                            }
                        }
                        else
                        {
                            //Nếu không đủ tiền, có thể bán nhà
                            //Bắt buộc bán nhà hoặc tuyên bố phá sản
                            int total = 0;
                            List<GameObject> _ownerHouses = new List<GameObject>();
                            for (int i = 0; i < _places.Length; i++)
                            {
                                if (_places[i].GetComponent<CellUtil>().getOwnerIdx() == PlayerController.GetInstance().GetPlayerTurnIdx())
                                {
                                    _ownerHouses.Add(_places[i]);
                                    total += _places[i].GetComponent<CellUtil>().getPrice(_places[i].GetComponent<CellUtil>().getCurrentHouseLevel());
                                }
                            }


                            if (total >= _places[position].GetComponent<CellUtil>().getPrice(currentHouseLevel))
                            {
                                //Hiện thông báo có muốn bán nhà khác để xây nhà này không
                                //Giả sử chọn có
                                for (int i = 0; i < _ownerHouses.Count; i++)
                                {
                                    _ownerHouses[i].GetComponent<CellUtil>().playHighLightColor();
                                }
                            }
                            else
                            {
                                //Thông báo không đủ tiền để mua / xây hoặc không làm gì
                            }
                        }
                    }
                    else
                    {
                        //Nếu đi vào nhà của người khác
                        //Show dialog
                        int currentHouseLevel = _places[position].GetComponent<CellUtil>().getCurrentHouseLevel();
                        if (PlayerController.GetInstance().GetMoneyCurrentPlayer() >= _places[position].GetComponent<CellUtil>().getVisitPrice(currentHouseLevel))
                        {
                            PlayerController.GetInstance().SetMoneyCurrentPlayer(PlayerController.GetInstance().GetMoneyCurrentPlayer() - _places[position].GetComponent<CellUtil>().getVisitPrice(currentHouseLevel)); //Trừ tiền
                            PlayerController.GetInstance().AddMoneyPlayer(_places[position].GetComponent<CellUtil>().getOwnerIdx(), _places[position].GetComponent<CellUtil>().getVisitPrice(currentHouseLevel)); //Cộng tiền cho đội bạn
                        }
                        else
                        {
                            //Bắt buộc bán nhà hoặc tuyên bố phá sản
                            int total = 0;
                            List<GameObject> _ownerHouses = new List<GameObject>();
                            for (int i = 0; i < _places.Length; i++)
                            {
                                if (_places[i].GetComponent<CellUtil>().getOwnerIdx() == PlayerController.GetInstance().GetPlayerTurnIdx())
                                {
                                    _ownerHouses.Add(_places[i]);
                                    total += _places[i].GetComponent<CellUtil>().getPrice(_places[i].GetComponent<CellUtil>().getCurrentHouseLevel());
                                }
                                   
                            }
                            if (total >= _places[position].GetComponent<CellUtil>().getVisitPrice(currentHouseLevel))
                            {
                                //Bán nhà
                                for (int i = 0; i < _ownerHouses.Count; i++)
                                {
                                    _ownerHouses[i].GetComponent<CellUtil>().playHighLightColor();
                                }
                            }else
                            {
                                //Phá sản
                                Debug.Log("Pha san");
                            }
                        }
                    }
                    break;
			}
		case _valGoPrison:
			gameControllerInstance.GoPrison (position);
			break;
		case _valOpportunity:
			//gameControllerInstance.Opportunity ();
			return;
		}
		int delta = 1;
		if (_isSameDice) {
			delta = 0;
		}
		PlayerController.GetInstance ().SetPlayerTurnIdx (delta);
		//gameControllerInstance._shouldThrowDice = true;
	}

	public void PassStart(){
		PlayerController.GetInstance ().IncreaseMoney (_bonusPassStart);
	}

	public void OutPrisonFunc() {
		Ultility.MyDebug ("Out of prison function", null);
	}

    public void processSaleHouse()
    {

    }

	private GameObject _helicopter;
	public void GoPrison(int curPosition) {
		if (PlayerController.GetInstance ().GetCurrentPlayerPrisonLicense () > 0) {
			MyDebug ("Dung the ra tu", null);
			PlayerController.GetInstance ().ChangeCurrentPlayerPrisonLicense (-1);
			return;
		}

		_helicopter = Instantiate (_helicopterPrefab, _gameObjectInBoard.transform);
		_isGoPrison = true;
		_helicopter.transform.localPosition = _places [curPosition].transform.localPosition + new Vector3 (0f, 0.2f, 0f);
		PlayerController.GetInstance ().HidePlayer (true);
		StartCoroutine(MoveHelicopterInCurve());

	}

	public void Unlucky() {

	}



	public void Opportunity() {
		_canvas.transform.Find ("Opportunity").gameObject.SetActive (true);
		_isWaitCardChoice = true;

		if (AR_MODE_ON) {
			_fullBoard.SetActive (false);
			for (int i = 0; i < _cardChoiceTarget.Length; i++)
				_cardChoiceTarget [i].SetActive (true);
		} else
			StartCoroutine (Wait(2f));

	}

	IEnumerator Wait(float time) {
		yield return new WaitForSeconds(time);
		_cardChoice = 0;
	}

	private string[] _UIImageNames = { "ImageA", "ImageB", "ImageC"};
	private void RotateUIImage() {
		Transform imageA = _canvas.transform.Find ("Opportunity").Find(_UIImageNames[_cardChoice]); 
		StartCoroutine(RotateUIImage(imageA, _luckGifs[_cardChoice], 5.0f));
		_isWaitCardChoice = false;

		if (AR_MODE_ON) {
			_fullBoard.SetActive (true);
			for (int i = 0; i < _cardChoiceTarget.Length; i++)
				_cardChoiceTarget [i].SetActive (false);
		}  

		_cardChoice = -1;
	}

	IEnumerator RotateUIImage(Transform img, Sprite replace, float duration)
	{
		float elapse_time = 0;
		float speed = 100 + duration/Time.deltaTime * 20;
		while (elapse_time <= duration)
		{
			img.Rotate (0, speed * Time.deltaTime, 0);
			speed -= 20;
			elapse_time += Time.deltaTime;
			yield return null;
		}
		img.rotation = Quaternion.identity;

		yield return new WaitForSeconds(0.2f);
		img.GetComponent<Image> ().sprite = replace;
		yield return new WaitForSeconds(4f);
		_shouldThrowDice = true;
		img.GetComponent<Image> ().sprite = _luckyCard;
		_canvas.transform.Find ("Opportunity").gameObject.SetActive (false);
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
		float Vy = velocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

		// Calculate flight time.
		float flightDuration = 2 * Vy / gravity;
		Vx = (_places [9].transform.localPosition.x - _helicopter.transform.localPosition.x) / flightDuration;
		Vz = (_places [9].transform.localPosition.z - _helicopter.transform.localPosition.z) / flightDuration;
		float elapse_time = 0;

		// fly curvely
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
		PlayerController.GetInstance ().HidePlayer (false);
		_isGoPrison = false;
		PlayerController.GetInstance ().SetPlayerTurnIdx (1);
		yield return new WaitForSeconds(0.2f);
		_shouldThrowDice = true;
	} 
}
