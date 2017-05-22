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
	public GameObject _planePrefab;
	public GameObject _gameObjectInBoard;
	public GameObject _canvas;
    public GameObject _targets;
    //-----------------------------------------
    public bool _isWaitCardChoiceSaleHouse; //Card choice cho bán nhà
    public bool _isWaitCardChoiceCityProcess; //Card choice yes no cho các vùng đất thuộc type city
    public int _totalSaleMoney = 0; //Số tiền đã bán được của người chơi hiện tại
    private int _totalPayMoney = 0;
    private int _currentPosition; //Dùng cho AR
    private int _currentTurnIdx;//Dùng cho AR
    private int _type;//Dùng cho AR
    private int _currentPlayerMoney = 0;//Dùng cho AR
    private List<GameObject> _ownerHouses;
    //-------------------------------------------
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
	public const int _valAirport = 9;
	public const int _valCoachStation = 10;

	private int[] _eventAtPosition;

	public bool _isWaitCardChoice;
	public int _cardChoice = -1;

	private int _typeCurrentEvent = 23;

	public bool _shouldThrowDice = false;

	public float _totalEffectTime = 0.2f;
	public float _startGameTime = 1f;

	public Sprite[] _luckGifs; 
	public Sprite _luckyCard;
	public Sprite _unluckyCard;
	public Sprite[] _unluckyResults;

    public bool _changeCamera = F;
    public GameObject _ARCamera;
	public GameObject _normalCamera;
	public GameObject[] _cardChoiceTarget;
	public GameObject _fullBoard;
    public bool _isWaiting = F; //Chờ xong effect
	private bool AR_MODE_ON = true;
	private bool DEBUG_AR = F;


    public bool _isWaitInit = T;
    public bool _isInited = F;
	public static bool F = false;
	public static bool T = true;
	public static bool DEBUG_EVENT = F;
	public static int DEBUG_EVENT_VAL = _valAirport;

	private GameObject _helicopter;
	private GameObject _plane;
	private GameObject _arrowTarget;

	public GameObject _arrowTargetPrefab;

	// Instance of Singleton pattern
	private static GameController gameControllerInstance = null;
	private GameController() {
	}

	public static GameController GetInstance() {
		return gameControllerInstance;
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

	private GameObject _mainCamera;
	private GameObject _moveCamera;
	// Use this for initialization
	void Start () {
		gameControllerInstance.InitEventTypeAtPosition();

		StartCoroutine (WaitForInitPlayers (_totalEffectTime));
		if (!_isWaiting) StartCoroutine (WaitForFirstThrowDice (_startGameTime));

		if (!AR_MODE_ON) {
			_ARCamera.SetActive (false);
			_mainCamera = _normalCamera;
			//for (int i = 0; i < _cardChoiceTarget.Length; i++)
			//	_cardChoiceTarget [i].SetActive (false);
		} else {
			_mainCamera = _ARCamera.transform.Find("Camera").gameObject;
			_normalCamera.SetActive (false);
		}
	}


	// Update is called once per frame
	void Update () {
        if (_isWaitInit)
            return;
        else
        {
            if (!_isInited)
            {
                _isInited = T;
                // Khoi tao effect va board
                Effect2.EFFECT = true;
                Effect3.EFFECT = true;
                Effect4.EFFECT = true;
                return;
            } 
        }
		if (_shouldThrowDice) {
			if (PlayerController.GetInstance ().GetCurrentPlayer ()._turnExtra == -1) {
				PlayerController.GetInstance ().GetCurrentPlayer ()._turnExtra = 0;
				PlayerController.GetInstance ().SetPlayerTurnIdx (1);
			}

			_shouldThrowDice = false;
			GetComponent<DiceEventHandler> ().ThrowDice ();
		}

        if (_isWaitCardChoice)
        {
            if (_cardChoice != -1)
            {
                Debug.Log("Run YES NO A B C");
                _isWaitCardChoice = false;
                if (_typeCurrentEvent == _valOpportunity || _typeCurrentEvent == _valUnlucky)
                {
                    RotateUIImage();
                }

                if (_typeCurrentEvent == _valAuction)
                {
                    Debug.Log("Run YES NO");
                    bool accept = true;
                    if (_cardChoice == 4)
                        accept = false;
                    _cardChoice = -1;
                    Auction.GetInstance().ReceiveAuctionResult(PlayerController.GetInstance().GetCurrentPlayer()._position, PlayerController.GetInstance().GetPlayerTurnIdx(), accept);
                }
            }
        }
        if (_isWaitCardChoiceSaleHouse)
        {
            if ((_totalSaleMoney + _currentPlayerMoney) >= _totalPayMoney && _type == 1)
            {
                //Stop sale house processing
                _isWaitCardChoiceSaleHouse = false;
                //Next turn
                int delta = 1;
                if (_isSameDice)
                {
                    delta = 0;
                }
                PlayerController.GetInstance().SetPlayerTurnIdx(delta);
                gameControllerInstance._shouldThrowDice = true;

                //Stop hight light color
                for (int i = 0; i < _ownerHouses.Count; i++)
                    _ownerHouses[i].GetComponent<CellUtil>().stopHighLightColor();
                //Set tiền dư cho người chơi
                PlayerController.GetInstance().SetMoneyPlayer(_currentTurnIdx, _totalSaleMoney + _currentPlayerMoney - _totalPayMoney);
                Debug.Log("Tien du: " + (_totalSaleMoney + _currentPlayerMoney - _totalPayMoney).ToString());
                //Nâng cấp nhà tại vị trí hiện tại
                _places[_currentPosition].GetComponent<CellUtil>().upgradeHouse(_currentTurnIdx);
                _totalSaleMoney = _totalPayMoney = 0;
            }
            else if (_totalSaleMoney >= _totalPayMoney && _type == 2)
            {
                //Stop sale house processing
                _isWaitCardChoiceSaleHouse = false;
                //Next turn
                int delta = 1;
                if (_isSameDice)
                {
                    delta = 0;
                }
                PlayerController.GetInstance().SetPlayerTurnIdx(delta);
                gameControllerInstance._shouldThrowDice = true;
                //Stop hight light color
                for (int i = 0; i < _ownerHouses.Count; i++)
                    _ownerHouses[i].GetComponent<CellUtil>().stopHighLightColor();
                //Set tiền dư cho người chơi
                PlayerController.GetInstance().SetMoneyPlayer(_currentTurnIdx, _totalSaleMoney + _currentPlayerMoney - _totalPayMoney);
                //Cộng tiền cho đội bạn
                if (_places[_currentPosition].GetComponent<CellUtil>().getOwnerIdx() != -1) PlayerController.GetInstance().AddMoneyPlayer(_places[_currentPosition].GetComponent<CellUtil>().getOwnerIdx(), _totalPayMoney);
                Debug.Log("Tien du: " + (_totalSaleMoney + _currentPlayerMoney - _totalPayMoney).ToString());
                _totalSaleMoney = _totalPayMoney = 0;
            }
        }
       
    }
    public void HandleAfterFinishAuction()
    {
        SetPlayerTurnIdx();
        _shouldThrowDice = true;
    }
    public void SetPlayerTurnIdx()
    {
        int delta = 1;
        if (_isSameDice)
            delta = 0;
        PlayerController.GetInstance().SetPlayerTurnIdx(delta);
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

    public void OutPrisonFunc()
    {
        StartCoroutine(OutPrison(2.0f));
    }

    IEnumerator OutPrison(float time)
    {
        yield return new WaitForSeconds(time);
        _shouldThrowDice = T;
        PlayerController.GetInstance().SetPlayerTurnIdx(1);
        PlayerController.GetInstance().ChangeMovePlayerValue(false);
    }

    public GameObject _moveCameraPrefab;
	public void EditCamera(Player obj) {
        if (!_changeCamera)
            return;

        _mainCamera.transform.GetComponent<Camera>().rect = new Rect (0f, 0f, 0f, 0f);
		if (!AR_MODE_ON)
			_mainCamera.transform.GetComponent<AudioListener> ().enabled = false;
		else
			_ARCamera.transform.GetComponent<AudioListener> ().enabled = false;
		_moveCamera = Instantiate (_moveCameraPrefab, obj._player.transform);
		_moveCamera.transform.localPosition = new Vector3 (0f, 1.6f, - 2.75f);
		_moveCamera.transform.localEulerAngles = new Vector3 (12f, 0f, 0f);
		//_moveCamera.transform.GetComponent<CameraFollowPlayer> ().target = obj;
	}

	public void SetEulerAnglesCamera() {
		_moveCamera.transform.eulerAngles = new Vector3 (12f, 0f, 0f);
	}

	public void ResetCamera() {
        if (!_changeCamera)
            return;
        if (!AR_MODE_ON)
			_mainCamera.transform.GetComponent<AudioListener> ().enabled = T;
		else
			_ARCamera.transform.GetComponent<AudioListener> ().enabled = T;
		_mainCamera.transform.GetComponent<Camera>().rect = new Rect (0f, 0f, 1f, 1f);
		Destroy (_moveCamera);
	}

	public void InitArrowTarget(int position) {
		_arrowTarget = Instantiate (_arrowTargetPrefab, _gameObjectInBoard.transform);
		_arrowTarget.GetComponent<ArrowAnimation>().begin = _places [position].transform.localPosition + new Vector3(0f, 0.2f, 0f);
	}

	public void DestroyArrowTarget() {
		Destroy (_arrowTarget);
	}

	public void AddCellAnimation(int position) {
		_places [position].AddComponent<CellAnimation> ();
	}

    IEnumerator HideDialogAfter(float time)
    {
        yield return new WaitForSeconds(time);
        MyHideDialog();
    }

    public void StartCoroutineHideDialog()
    {
        StartCoroutine(HideDialogAfter(2f));
    }

    public static bool _isSameDice;
	public static void HandleFinishThrowDice(int sumDiceValue, bool isSame) {
		_isSameDice = isSame;
		Player curPlayer = PlayerController.GetInstance ().GetCurrentPlayer ();
		if (_isSameDice) {
            if (curPlayer._inPrison != -1)
            {
                gameControllerInstance.showDialog("Bạn được ra tù");
                curPlayer._inPrison = -1;
                gameControllerInstance.OutPrisonFunc();
                gameControllerInstance.StartCoroutineHideDialog();
                return;
            }

            if (curPlayer._sameDiceCount == 2)
            {
                Ultility.MyDebug("Bi nhot vao tu vi 3 lan xuc xac giong nhau", null);
                gameControllerInstance._typeCurrentEvent = _valGoPrison;
                curPlayer._sameDiceCount = 0;
                gameControllerInstance.GoPrison(PlayerController.GetInstance().GetCurrentPlayer()._position);

                return;
            }
            else
            {
                curPlayer._sameDiceCount += 1;
            }
        }
        else
        {
            if (curPlayer._inPrison == 0)
            {
                gameControllerInstance.GetMoneyToOutPrison();
                return;
            }
            else if (curPlayer._inPrison > 1)
            {
                curPlayer._inPrison -= 1;
                gameControllerInstance.SetPlayerTurnIdx();
                gameControllerInstance._shouldThrowDice = T;
                return;
            }
        }
		gameControllerInstance.InitArrowTarget (curPlayer.GetPositionWithDelta (sumDiceValue));
		PlayerController.GetInstance ().MovePlayer (sumDiceValue);
	}

    public void GetMoneyToOutPrison()
    {
        if (PlayerController.GetInstance().GetMoneyCurrentPlayer() > 5000)
        {
            showDialog("Bạn tiêu 5000 để ra tù");
            PlayerController.GetInstance().IncreaseMoney(-5000);
            SetPlayerTurnIdx();
            _shouldThrowDice = true;
            gameControllerInstance.StartCoroutineHideDialog();
        }
        else
        {
            // GOi ban nha`
            Debug.Log("Goi ban nha de ra tu vi het tien");
            showDialog("Bán nhà để ra tù");
            processSaleHouse(5000);
        }
    }

    public void HandleEventAtPosition(int position, int stepMoved) {
		gameControllerInstance.DestroyArrowTarget ();
		if (position - stepMoved < 0)
			gameControllerInstance.PassStart ();
		
		gameControllerInstance._typeCurrentEvent = gameControllerInstance._eventAtPosition [position];
		if (gameControllerInstance.AR_MODE_ON && gameControllerInstance.DEBUG_AR)
			gameControllerInstance._typeCurrentEvent = _valOpportunity;
		if (DEBUG_EVENT)
			gameControllerInstance._typeCurrentEvent = DEBUG_EVENT_VAL;
        _currentTurnIdx = PlayerController.GetInstance().GetPlayerTurnIdx();
        _currentPosition = position;
        _currentPlayerMoney = PlayerController.GetInstance().GetMoneyPlayer(_currentTurnIdx);
        switch (gameControllerInstance._typeCurrentEvent) {
		case _valBuildHouse:
			{
                LandProcess();
                return;
			}
		case _valGoPrison:
			gameControllerInstance.GoPrison (position);
                return;
		case _valOpportunity:
                gameControllerInstance.LuckyAndUnlucky ();
                return;
		case _valUnlucky:
			gameControllerInstance.LuckyAndUnlucky ();
                return;
		case _valAirport:
			gameControllerInstance.RandomFly ();
			return;
        case _valAuction:
                gameControllerInstance.DoAuction(PlayerController.GetInstance()._playerTurnIdx, position);
                return;
        case _valPrison:
                gameControllerInstance.VisitPrison();
                return;
        }
		int delta = 1;
		if (_isSameDice) {
			delta = 0;
		}
		PlayerController.GetInstance ().SetPlayerTurnIdx (delta);
		gameControllerInstance._shouldThrowDice = true;
	}

    public void VisitPrison()
    {
        for (int i = 0; i < PlayerController.GetInstance()._playerNum; i++)
        {
            if (i != PlayerController.GetInstance()._playerTurnIdx && PlayerController.GetInstance()._players[i]._inPrison != -1)
            {
                if (PlayerController.GetInstance().GetMoneyCurrentPlayer() > 5000)
                {
                    showDialog("Bạn tiêu 5000 vì thăm tù");
                    PlayerController.GetInstance().IncreaseMoney(-5000);
                    SetPlayerTurnIdx();
                    _shouldThrowDice = true;
                    gameControllerInstance.StartCoroutineHideDialog();
                }
                else
                {
                    // GOi ban nha`
                    Debug.Log("Goi ban nha de ra tu vi het tien");
                    showDialog("Bán nhà vì thiếu tiền thăm tù");
                    processSaleHouse(5000);
                    return;
                }
                break;
            }
        }
        SetPlayerTurnIdx();
        _shouldThrowDice = T;
    }

    public void LandProcess()
    {
        // Sự kiện người chơi có thể mua nhà, hoặc đóng phí khi vào nhà xử lý tại đây
        // Muốn lấy thông tin  của người chơi hiện tại thì dùng  PlayerController.GetInstance ()  -> Get money, position,...
        // Sự kiện người chơi có thể mua nhà, hoặc đóng phí khi vào nhà xử lý tại đây
        // Muốn lấy thông tin  của người chơi hiện tại thì dùng  PlayerController.GetInstance ()  -> Get money, position,...
        //Nếu ô đất này là của người chơi sở hữu hoặc chưa có ai sở hữu
        int currentHouseLevel = _places[_currentPosition].GetComponent<CellUtil>().getCurrentHouseLevel();
       
        if (_places[_currentPosition].GetComponent<CellUtil>().getOwnerIdx() == _currentTurnIdx || _places[_currentPosition].GetComponent<CellUtil>().getOwnerIdx() == -1)
        {
            int payMoney = _places[_currentPosition].GetComponent<CellUtil>().getPrice(currentHouseLevel);
            if (_currentPlayerMoney >= payMoney)
            {
                //Show dialog hỏi ý kiến người chơi, nếu chọn yes
                //Nếu đủ tiền
                //---------------------------
                //Build house process
                if (_places[_currentPosition].GetComponent<CellUtil>().getOwnerIdx() == -1)
                    showDialog("Bạn có muốn mua " + _places[_currentPosition].GetComponent<CellUtil>().getPlaceName() + " với giá " + _places[_currentPosition].GetComponent<CellUtil>().getCurrentPrice() + "$ không?");
                else showDialog("Bạn có muốn nâng cấp nhà tại " + _places[_currentPosition].GetComponent<CellUtil>().getPlaceName() + " với giá " + _places[_currentPosition].GetComponent<CellUtil>().getCurrentPrice() + "$ không?");
                ConditionTrackableEventHandler._type = 0;
                _isWaitCardChoiceCityProcess = true;
                return;
                //-----------------------------------
            }
            else
            {
                //Nếu không đủ tiền, có thể bán nhà
                //Bắt buộc bán nhà hoặc tuyên bố phá sản
                int total = 0;
                _ownerHouses = new List<GameObject>();
                for (int i = 0; i < _places.Length; i++)
                {
                    if ((_places[i].GetComponent<CellUtil>().getOwnerIdx() == _currentTurnIdx) && i != _currentPosition)
                    {
                        _ownerHouses.Add(_places[i]);
                        total += _places[i].GetComponent<CellUtil>().getPrice(_places[i].GetComponent<CellUtil>().getCurrentHouseLevel());
                    }
                }
                if ((total + _currentPlayerMoney) >= payMoney)
                {

                    //Show dialog hỏi ý kiến người chơi, nếu chọn Yes
                    //-------------------------------------------------
                    //Sale house process 1
                    if (currentHouseLevel < 4)
                    {
                        if (_places[_currentPosition].GetComponent<CellUtil>().getOwnerIdx() == -1)
                            showDialog("Bạn không đủ tiền mặt, bạn cần " + (payMoney - _currentPlayerMoney).ToString() + "$ để mua " + _places[_currentPosition].GetComponent<CellUtil>().getPlaceName() + ". Bạn có muốn mua không?");
                        else showDialog("Bạn không đủ tiền mặt, bạn cần " + (payMoney - _currentPlayerMoney).ToString() + "$ để nâng cấp nhà tại " + _places[_currentPosition].GetComponent<CellUtil>().getPlaceName() + ". Bạn có muốn nâng cấp nhà không?");
                        ConditionTrackableEventHandler._type = 1;
                        _isWaitCardChoiceCityProcess = true;
                        return;
                    }
                    //-------------------------------------------------
                }
            }
        }
        else
        {
            //Nếu đi vào nhà của người khác
            //Show dialog

            int payMoney = _places[_currentPosition].GetComponent<CellUtil>().getVisitPrice(currentHouseLevel);
            if (_currentPlayerMoney >= payMoney)
            {
                PlayerController.GetInstance().AddMoneyPlayer(_currentTurnIdx, -payMoney); //Trừ tiền
                PlayerController.GetInstance().AddMoneyPlayer(_places[_currentPosition].GetComponent<CellUtil>().getOwnerIdx(), payMoney); //Cộng tiền cho đội bạn
                int delta = 1;
                if (_isSameDice)
                {
                    delta = 0;
                }
                PlayerController.GetInstance().SetPlayerTurnIdx(delta);
                gameControllerInstance._shouldThrowDice = true;
            }
            else
            {
                //Bắt buộc bán nhà hoặc tuyên bố phá sản
                int total = 0;
                _ownerHouses = new List<GameObject>();
                for (int i = 0; i < _places.Length; i++)
                {
                    if (_places[i].GetComponent<CellUtil>().getOwnerIdx() == _currentTurnIdx)
                    {
                        _ownerHouses.Add(_places[i]);
                        total += _places[i].GetComponent<CellUtil>().getPrice(_places[i].GetComponent<CellUtil>().getCurrentHouseLevel());
                    }
                }
                if ((total + _currentPlayerMoney) >= payMoney)
                {
                    //-----------------------------------------------
                    //Sale house process 2
                    //showDialog();
                    Debug.Log("AAAAAAAAAAAAAAAAAAAA");
                    showDialog("Bạn không đủ tiền mặt, bạn cần " + (payMoney - _currentPlayerMoney).ToString() + "$ để trả tiền ghé thăm tại " + _places[_currentPosition].GetComponent<CellUtil>().getPlaceName() + ". Bạn có muốn bán nhà để chi trả không?");
                    ConditionTrackableEventHandler._type = 2;
                    _isWaitCardChoiceCityProcess = true;
                    return;
                    //-------------------------------------
                }
                else
                {
                    //Phá sản
                    int delta = 1;
                    if (_isSameDice)
                    {
                        delta = 0;
                    }
                    PlayerController.GetInstance().SetPlayerTurnIdx(delta);
                    showDialogInSeconds("Rất tiếc, bạn đã phá sản", 5F);
                    //Next turn
                    
                    //gameControllerInstance._shouldThrowDice = true;
                }
            }
        }

        
    }

    public void MyHideDialog()
    {
        _canvas.transform.Find("Dialog").gameObject.GetComponent<DialogEffect>().hideDialog();
    }

    public void hideDialog()
    {
        //_canvas.transform.Find("Dialog").gameObject.SetActive(false);
        _shouldThrowDice = true;
        _canvas.transform.Find("Dialog").gameObject.GetComponent<DialogEffect>().hideDialog();
    }
    public void showDialog(string text)
    {
        _shouldThrowDice = false;
        //_canvas.transform.Find("Dialog").gameObject.SetActive(true);
        _canvas.transform.Find("Dialog").gameObject.transform.Find("Text").GetComponent<Text>().text = text;
        _canvas.transform.Find("Dialog").gameObject.GetComponent<DialogEffect>().showDialog();
    }

    public void showDialogInSeconds(string text, float time)
    {
        showDialog(text);
        StartCoroutine(DialogEnd(time));
    }

    IEnumerator DialogEnd(float time)
    {
        yield return new WaitForSeconds(time);
        hideDialog();
    }
    public void buildHouseProcess()
    {
        PlayerController.GetInstance().AddMoneyPlayer(_currentTurnIdx, -_places[_currentPosition].GetComponent<CellUtil>().getPrice(_places[_currentPosition].GetComponent<CellUtil>().getCurrentHouseLevel()));
        //Build house
        if (_places[_currentPosition].GetComponent<CellUtil>().upgradeHouse(_currentTurnIdx) == 1)
        {
            //Nếu còn có thể xây thêm nhà
            //Hiện thông báo
            Debug.Log("Upgrade");
        }
        int delta = 1;
        if (_isSameDice)
        {
            delta = 0;
        }
        PlayerController.GetInstance().SetPlayerTurnIdx(delta);
        gameControllerInstance._shouldThrowDice = true;
    }

    public void saleHouseProcess1()
    {
        _type = 1;
        _totalPayMoney = _places[_currentPosition].GetComponent<CellUtil>().getPrice(_places[_currentPosition].GetComponent<CellUtil>().getCurrentHouseLevel());
        //Hiện thông báo có muốn bán nhà khác để xây nhà này không
        //Giả sử chọn có
        for (int i = 0; i < _ownerHouses.Count; i++)
        {
            _ownerHouses[i].GetComponent<CellUtil>().playHighLightColor();
            _targets.transform.Find(_ownerHouses[i].name).GetComponent<CellTrackableEventHandler>()._needToProcess = true;
        }
        _isWaitCardChoiceSaleHouse = true;
        _shouldThrowDice = false;
    }

    public void saleHouseProcess2()
    {
        _type = 2;
        _totalPayMoney = _places[_currentPosition].GetComponent<CellUtil>().getVisitPrice(_places[_currentPosition].GetComponent<CellUtil>().getCurrentHouseLevel());
        //Bán nhà
        for (int i = 0; i < _ownerHouses.Count; i++)
        {
            _ownerHouses[i].GetComponent<CellUtil>().playHighLightColor();
            _targets.transform.Find(_ownerHouses[i].name).GetComponent<CellTrackableEventHandler>()._needToProcess = true;
        }
        _isWaitCardChoiceSaleHouse = true;
        _shouldThrowDice = false;
    }

    public void processSaleHouse(int fee)
    {
        _type = 2;
        _totalPayMoney = fee;
        for (int i = 0; i < _ownerHouses.Count; i++)
        {
            _ownerHouses[i].GetComponent<CellUtil>().playHighLightColor();
            _targets.transform.Find(_ownerHouses[i].name).GetComponent<CellTrackableEventHandler>()._needToProcess = true;
        }
        _isWaitCardChoiceSaleHouse = true;
        _shouldThrowDice = false;
    }

    public void FlyEnimHousePosition () {
		int cur = PlayerController.GetInstance ()._playerTurnIdx;
		int next = PlayerController.GetInstance ().GetNextPlayTurnIdx ();
		for (int i = 1; i < 36; i++) {
			if (_places [i].GetComponent<CellUtil> ().getOwnerIdx () != cur) {
				_plane = Instantiate (_planePrefab, _gameObjectInBoard.transform);
				_plane.transform.localPosition = _places [PlayerController.GetInstance().GetCurrentPlayer()._position].transform.localPosition + new Vector3 (0f, 0.2f, 0f);
				PlayerController.GetInstance ().SetActivePlayer (false);
				StartCoroutine(FlyPlayer(_plane, cur, PlayerController.GetInstance ().GetCurrentPlayer ()._position, i));
				return;
			}
		}
		for (int j = 0; j < 4; j++) {
			if (j != cur && j != next) {
				for (int i = 1; i < 36; i++) {
					if (_places [i].GetComponent<CellUtil> ().getOwnerIdx () != cur) {
						_plane = Instantiate (_planePrefab, _gameObjectInBoard.transform);
						_plane.transform.localPosition = _places [PlayerController.GetInstance().GetCurrentPlayer()._position].transform.localPosition + new Vector3 (0f, 0.2f, 0f);
						PlayerController.GetInstance ().SetActivePlayer (false);
						StartCoroutine (FlyPlayer (_plane, cur, PlayerController.GetInstance ().GetCurrentPlayer ()._position, i));
						return;
					}
				}
			}
		}

        SetPlayerTurnIdx();
		gameControllerInstance._shouldThrowDice = true;
	}

    public void DoAuction(int playeId, int position)
    {
        int fee = Auction.GetInstance().CheckFee(playeId, position);
        if (fee > 0)
        {
            Debug.Log("Fee > 0");
            bool res = PlayerController.GetInstance().IncreaseMoney(-fee);
            if (!res)
            {
                showDialog("Bạn phải bán nhà để trả phí");
                StartCoroutineHideDialog();
                processSaleHouse(fee);
            }
        }
        else if (fee < 0)
        {
            int cost = Auction.GetInstance().GetCostToAuction(playeId, position);
            if (cost > 0 && cost <= PlayerController.GetInstance().GetMoneyCurrentPlayer())
            {
                showDialog("Bạn có muốn đấu giá không?");
                _isWaitCardChoice = true;
                return;
            } else
            {
                showDialog("Bạn không đủ tiền để đấu giá @@");
                Auction.GetInstance().CheckOwner(position);
                _isWaitCardChoice = false;
            }  
        } else
        {
            showDialog("Bạn là chủ và không mất phí");
            StartCoroutineHideDialog();
        }


        SetPlayerTurnIdx();
        _shouldThrowDice = true;
    }

    public static int GetRandomIntExcecpt(int i) {
		while (true) {
			int res = Random.Range (0, 36);
			if (res != i)
				return res;
		}
	}

	public void RandomFly() {
		int randomPos = GetRandomIntExcecpt (27);
		_plane = Instantiate (_planePrefab, _gameObjectInBoard.transform);
		_plane.transform.localPosition = _places [27].transform.localPosition + new Vector3 (0f, 0.2f, 0f);
		PlayerController.GetInstance ().SetActivePlayer (false);
		StartCoroutine(FlyPlayer(_plane, PlayerController.GetInstance ()._playerTurnIdx, 27, randomPos));
	}

	public void PassStart(){
        showDialog("Bạn vượt qua ô xuất phát và được thưởng: " + _bonusPassStart.ToString());
		PlayerController.GetInstance ().IncreaseMoney (_bonusPassStart);
        MyHideDialog();
	}


		
	public void GoPrison(int curPosition) {
		if (PlayerController.GetInstance ().GetCurrentPlayerPrisonLicense () > 0) {
			PlayerController.GetInstance ().ChangeCurrentPlayerPrisonLicense (-1);
			_shouldThrowDice = true;
			return;
		}

		_helicopter = Instantiate (_helicopterPrefab, _gameObjectInBoard.transform);
		_helicopter.transform.localPosition = _places [curPosition].transform.localPosition + new Vector3 (0f, 0.2f, 0f);
		PlayerController.GetInstance ().SetActivePlayer (false);
		PlayerController.GetInstance ().GetCurrentPlayer ()._inPrison = 3;
		_shouldThrowDice = false;
		StartCoroutine(FlyPlayer(_helicopter, PlayerController.GetInstance ()._playerTurnIdx, curPosition, 9));
	}

	public void LuckyAndUnlucky() {
		_canvas.transform.Find ("LuckyAndUnlucky").gameObject.SetActive (true);
        Debug.Log("Luckyandunlucky");
		_isWaitCardChoice = true;

		if (AR_MODE_ON) {
			_fullBoard.SetActive (false);
			//for (int i = 0; i < _cardChoiceTarget.Length; i++)
			//	_cardChoiceTarget [i].SetActive (true);
		} else
			StartCoroutine (Wait(2f));
	}

	IEnumerator Wait(float time) {
		yield return new WaitForSeconds(time);
		_cardChoice = Random.Range (0, 3);;
	}

    private string[] _UIImageNames = { "ImageA", "ImageB", "ImageC" };
    private void RotateUIImage()
    {
        string name = "LuckyAndUnlucky";
        Sprite[] cards = _luckGifs;
        if (_typeCurrentEvent == _valUnlucky)
        {
            cards = _unluckyResults;
        }
        Transform image = _canvas.transform.Find(name).Find(_UIImageNames[_cardChoice]);
        int randomResult = Random.Range(0, 3);
        StartCoroutine(RotateUIImage(image, randomResult, cards[randomResult], 2.0f));
        _isWaitCardChoice = false;

        if (AR_MODE_ON)
        {
            _fullBoard.SetActive(true);
            for (int i = 0; i < _cardChoiceTarget.Length; i++)
                _cardChoiceTarget[i].SetActive(false);
        }

        _cardChoice = -1;
    }

    public void GetUnluckyAndUnluckyResult(int result)
    {
        int randomMoney = 1000 * Random.Range(1, 4);
        int delta = 1;
        if (_typeCurrentEvent == _valOpportunity)
        {
            switch (result)
            {
                case 0:
                    showDialog("Chúc mừng bạn miễn vào tù một lần");
                    PlayerController.GetInstance().GetCurrentPlayer()._prisonLicense += 1;
                    break;
                case 1:
                    // Them luot
                    showDialog("Chúc mừng bạn được thêm một lượt đi");
                    delta = 0;
                    break;
                case 2:
                    showDialog("Chúc mừng bạn được nhận số tiền: " + randomMoney.ToString());
                    PlayerController.GetInstance().GetCurrentPlayer()._money += randomMoney;
                    break;
            }
        }
        else if (_typeCurrentEvent == _valUnlucky)
        {
            switch (result)
            {
                case 0:
                    // Bay den nha doi thu
                    showDialog("Bay đến nhà của người chơi khác");
                    gameControllerInstance.StartCoroutineHideDialog();
                    FlyEnimHousePosition();
                    return;
                case 1:
                    // Mat Luot
                    showDialog("Bạn bị mất lượt lần sau");
                    PlayerController.GetInstance().GetCurrentPlayer()._turnExtra -= 1;
                    break;
                case 2:
                    showDialog("Bạn tặng quà sinh nhật cho bạn với số tiền là: " + randomMoney.ToString());
                    PlayerController.GetInstance().GetCurrentPlayer()._money -= randomMoney;
                    break;
            }
        }
        if (_isSameDice)
            delta = 0;

        PlayerController.GetInstance().SetPlayerTurnIdx(delta);
        _shouldThrowDice = true;
        gameControllerInstance.StartCoroutineHideDialog();
    }

    IEnumerator RotateUIImage(Transform img, int result, Sprite replace, float duration)
    {
        float elapse_time = 0;
        float speed = 100 + duration / Time.deltaTime * 20;
        while (elapse_time <= duration)
        {
            img.Rotate(0, speed * Time.deltaTime, 0);
            speed -= 20;
            elapse_time += Time.deltaTime;
            yield return null;
        }
        img.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.2f);
        Sprite originSpr = img.GetComponent<Image>().sprite;
        img.GetComponent<Image>().sprite = replace;
        yield return new WaitForSeconds(4f);


        img.GetComponent<Image>().sprite = originSpr;
        _canvas.transform.Find("LuckyAndUnlucky").gameObject.SetActive(false);
        GetUnluckyAndUnluckyResult(result);
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
		if (name.CompareTo ("Parking") == 0 || name.CompareTo ("TDDK") == 0
			|| name.CompareTo ("TDDL") == 0|| name.CompareTo ("TDVT") == 0)
			return _valAuction;
		if (name.CompareTo ("Tax") == 0)
			return _valTax;
		if (name.CompareTo ("Parking") == 0)
			return _valParkingFee;
		if (name.CompareTo ("Airport") == 0)
			return _valAirport;
		if (name.CompareTo ("CoachStation") == 0)
			return _valCoachStation;

		return _valBuildHouse;
	}

    IEnumerator FlyPlayer(GameObject model, int playerIdx, int startPos, int endPos)
    {
        Vector3 target = _places[endPos].transform.localPosition;
        if (startPos != endPos)
        {
            float firingAngle = 60.0f;
            float gravity = 9.8f / 4;

            Vector3 begin = _places[startPos].transform.localPosition;
            float target_Distance = Vector3.Distance(target, begin);

            // Calculate the velocity needed to throw the object to the target at specified angle.
            float velocity = Mathf.Sqrt(target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity));
            float rotateDegree = -90;
            if (target.z != begin.z)
                rotateDegree = Mathf.Rad2Deg * Mathf.Atan((target.x - begin.x) / (target.z - begin.z));
            model.transform.localEulerAngles = model.transform.localEulerAngles + new Vector3(0, rotateDegree, 0);
            float Vx = 0, Vz = 0;
            float Vy = velocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

            // Calculate flight time.
            float flightDuration = 2 * Vy / gravity;
            Vx = (target.x - begin.x) / flightDuration;
            Vz = (target.z - begin.z) / flightDuration;
            float elapse_time = 0;

            // fly curvely
            while (elapse_time <= flightDuration)
            {
                model.transform.localPosition += new Vector3(Vx * Time.deltaTime, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vz * Time.deltaTime);
                elapse_time += Time.deltaTime;
                if (elapse_time > flightDuration)
                    model.transform.localPosition = target;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
        else
            yield return new WaitForSeconds(1.6f);

        Destroy(model, 1.0f);
        PlayerController.GetInstance().SetLocalPosition(playerIdx, endPos);
        PlayerController.GetInstance().SetPosition(playerIdx, endPos);
        float faceAngle = 0f;
        if (endPos >= 9 && endPos < 18)
            faceAngle = 90f;
        else if (endPos >= 18 && endPos < 27)
            faceAngle = 180f;
        else if (endPos >= 27 && endPos < 36)
            faceAngle = 270f;
        PlayerController.GetInstance().GetCurrentPlayer()._player.transform.localEulerAngles = new Vector3(0f, faceAngle, 0f);
        PlayerController.GetInstance().SetActivePlayer(playerIdx, true);

        yield return new WaitForSeconds(1f);

        PlayerController.GetInstance().ChangeMovePlayerValue(false);
        if (_typeCurrentEvent == _valGoPrison)
        {
            PlayerController.GetInstance().SetPlayerTurnIdx(1);
            _shouldThrowDice = true;
        }
        else
        {
            HandleEventAtPosition(endPos, 0);
        }
    }
}
