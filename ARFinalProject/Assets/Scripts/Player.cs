using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		_position = (_position + 1) % GameController.GetPlacesNum();
	}

	public int GetNextPosition() {
		return (_position + 1) % GameController.GetPlacesNum();
	}
}
