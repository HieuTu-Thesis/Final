using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player class stores information of Player
public class Player {
	public GameObject _player;
	public int _position;
	public int _money;
	public int _turnExtra;
	 
	// Còn phải ở trong tù bao lâu. -1: ko ở trong tù, 0: phải dùng tiền để ra tù, 3: mới bị nhốt vào tù. Qua một lượt ném xúc xắc mà chưa ra dc thì trừ đi dần 
	public int _inPrison;
	// Đếm số lần có xúc xắc trùng nhau, 3 lần là bị nhốt tù 
	public int _sameDiceCount;

	// Số thẻ miễn vào tù khi vào ô may mắn nhận được 
	public int _prisonLicense;


	public Player(GameObject playerObject, int position, int money) {
		_player = playerObject;
		_position = position;
		_money = money;
		_inPrison = -1;
		_sameDiceCount = 0;
		_prisonLicense = 0;
		_turnExtra = 0;
	}

	public void IncreasePosition(int delta) {
		int num = GameController.GetPlacesNum ();
		_position = (_position + delta + num) % num;
	}

	public int GetNextPosition() {
		return (_position + 1) % GameController.GetPlacesNum();
	}

	// Lấy vị trí cách player delta đơn vị  
	public int GetPositionWithDelta(int delta) {
		int num = GameController.GetPlacesNum ();
		return (_position + delta + num) % num;
	}
}
