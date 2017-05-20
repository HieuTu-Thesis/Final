using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auction  {
	private static Auction _instance = null;
	private int curMoney;
	private Dictionary<string, AuctionPlace> _auctions = new Dictionary<string, AuctionPlace>();

	private Auction() {
	}

	public static Auction GetInstance() {
		if (_instance == null) {
			_instance = new Auction ();
			_instance.Init ();
		}
		return _instance;
	}

	private void Init() {
		AuctionPlace tdvt = new AuctionPlace ("TDVT", 20000 , 2000);
		AuctionPlace tddl = new AuctionPlace ("TDDL", 15000, 1500);
		AuctionPlace tddk = new AuctionPlace ("TDDK", 20000, 2000);

		_auctions.Add ("16", tdvt);
		_auctions.Add ("25", tddl);
		_auctions.Add ("29", tddk);
	}

	private class AuctionPlace {
		public string _name;
		public bool[] _canBuy = { true, true, true, true };
		public int[] _auctedMoney = { 0, 0, 0,0 };
		public int _startMoney;
		public int _rate;
		public int _curMoney;
		public int _owner = -1;
		public int _fee = -1;

		public AuctionPlace(string name, int money, int rate) {
			_curMoney = _startMoney = money;
			_rate = rate;
			_name = name;
			_fee = _startMoney / 2;
		}
	}

	// -1 thi` ko mất phí, chuyển qua hàm đấu giá
	// 0: chủ -> ko mất phí
	// > 0: đóng phí là _fee 
	public int CheckFee(int position, int playerId) {
		AuctionPlace ap = _instance._auctions [position.ToString()];
		if (ap._owner != -1) {
			if (ap._owner == playerId)
				return 0;
			else
				return ap._fee;
		}

		return -1;
	}

	// 0: ko thể đấu giá dc nữa
	// >0: chi phi dau gia
	public int GetCostToAuction(int position, int playerId) {
		AuctionPlace ap = _instance._auctions [position.ToString()];

		if (!ap._canBuy [playerId])
			return 0;

		return ap._curMoney - ap._auctedMoney [playerId] + ap._rate;
	}

	public void ReceiveAuctionResult(int position, int playerId, bool accept) {
		AuctionPlace ap = _instance._auctions [position.ToString()];

		if (!accept) {
			int cnt = 0;
			ap._canBuy [playerId] = false;
			for (int i = 0; i < 4; i++)
				if (ap._canBuy [i])
					cnt++;
			if (cnt == 1) {
				for (int i = 0; i < 4; i++)
					if (ap._canBuy [i])
						ap._owner = i;
			}
		} else {
			ap._curMoney += ap._rate;
			ap._auctedMoney [playerId] = ap._curMoney;
		}
	}
}
