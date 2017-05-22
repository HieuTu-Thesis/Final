using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auction
{
    private static Auction _instance = null;
    private int curMoney;
    private Dictionary<string, AuctionPlace> _auctions = new Dictionary<string, AuctionPlace>();

    private Auction()
    {
    }

    public static Auction GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Auction();
            _instance.Init();
        }
        return _instance;
    }

    private void Init()
    {
        AuctionPlace tdvt = new AuctionPlace("TDVT", 1800, 200);
        AuctionPlace tddl = new AuctionPlace("TDDL", 1350, 150);
        AuctionPlace tddk = new AuctionPlace("TDDK", 1800, 200);

        _auctions.Add("tdvt", tdvt);
        _auctions.Add("tddl", tddl);
        _auctions.Add("tddk", tddk);
    }

    private class AuctionPlace
    {
        public string _name;
        public bool[] _canBuy = { true, true, true, true };
        public int[] _auctedMoney = { 0, 0, 0, 0 };
        public int _startMoney;
        public int _rate;
        public int _curMoney;
        public int _owner = -1;
        public int _fee = -1;

        public AuctionPlace(string name, int money, int rate)
        {
            _curMoney = _startMoney = money;
            _rate = rate;
            _name = name;
            _fee = _startMoney / 2;
        }
    }

    AuctionPlace GetAuctionPlace(int pos)
    {
        if (pos == 17)
            return _instance._auctions["tdvt"];
        if (pos == 26)
            return _instance._auctions["tddl"];
        return _instance._auctions["tddk"];
    }

    // -1 thi` ko mất phí, chuyển qua hàm đấu giá
    // 0: chủ -> ko mất phí
    // > 0: đóng phí là _fee 
    public int CheckFee(int position, int playerId)
    {
        AuctionPlace ap = GetAuctionPlace(position);
        if (ap._owner != -1)
        {
            if (ap._owner == playerId)
                return 0;
            else
                return ap._fee;
        }

        return -1;
    }

    // 0: ko thể đấu giá dc nữa
    // >0: chi phi dau gia
    public int GetCostToAuction(int playerId, int position)
    {
        AuctionPlace ap = GetAuctionPlace(position);
        Debug.Log("Debug playerID " + playerId.ToString());
        if (!ap._canBuy[playerId])
            return 0;
        
        return ap._curMoney - ap._auctedMoney[playerId] + ap._rate;
    }

    public void ReceiveAuctionResult(int position, int playerId, bool accept)
    {
        AuctionPlace ap = GetAuctionPlace(position);

        if (!accept)
        {
            Debug.Log("No card is chosen");
            string log = "Bạn đã mất quyền đấu giá ";
           
            int cnt = 0;
            ap._canBuy[playerId] = false;
            PlayerController.GetInstance().AddMoneyPlayer(playerId, ap._auctedMoney[playerId]);
            if (ap._auctedMoney[playerId] > 0)
            {
                log = log + "và nhận lại số tiền: " + ap._auctedMoney[playerId].ToString();
            }
            GameController.GetInstance().showDialog(log);
            GameController.GetInstance().StartCoroutineHideDialog();
            ap._auctedMoney[playerId] = 0;   
        }
        else
        {
            Debug.Log("Yes card is chosen");
            ap._curMoney += ap._rate;
            GameController.GetInstance()._isWaitCardChoice = false;
            PlayerController.GetInstance().IncreaseMoney(ap._auctedMoney[playerId] - ap._curMoney);
            GameController.GetInstance().showDialog("Bạn đấu giá lên " + ap._curMoney.ToString());
            GameController.GetInstance().StartCoroutineHideDialog();
            ap._auctedMoney[playerId] = ap._curMoney;
        }
        CheckOwner(position);
        GameController.GetInstance().HandleAfterFinishAuction();
    }

    public void CheckOwner(int position)
    {
        AuctionPlace ap = GetAuctionPlace(position);
        int cnt = 0;
        for (int i = 0; i < 4; i++)
            if (ap._canBuy[i])
                cnt++;
        if (cnt == 1)
        {
            for (int i = 0; i < 4; i++)
                if (ap._canBuy[i] && ap._auctedMoney[i] > 0)
                {
                    GameController.GetInstance().showDialog("Người chơi " + i.ToString() + " đã chiến thắng đấu giá");
                    GameController.GetInstance().StartCoroutineHideDialog();
                    ap._owner = i;
                    GameController.GetInstance()._places[position].GetComponent<CellUtil>().setOwnerIdx(i);
                    ap._fee = ap._auctedMoney[i] / 2;
                }
        }
    }
}
