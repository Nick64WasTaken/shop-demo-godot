using Godot;
using System;

public partial class PlayerManager : Node
{
	
	private int _money = 9935;
	private Godot.Collections.Dictionary _inventory = new Godot.Collections.Dictionary();
	
	public int GetMoney() {
		return _money;
	}
	
	public bool SpendMoney(int moneySpent) {
		if (_money > moneySpent) {
			_money = _money - moneySpent;
			return true;
		}
		return false;
	}
	
	public void AddMoney(int moneyAdded) {
		if ((_money + moneyAdded) >= 9999999) {
			_money = 9999999;
		} else {
			_money += moneyAdded;
		}
		
	}
	
	public int GetItemQuantityFromInventory(Item item) {
		int itemID = item.GetItemID();
		
		if (_inventory.ContainsKey(itemID)) {
			return (int) _inventory[itemID];
		} else {
			return 0;
		}
	}
	
	// Grabs the item's ID and checks if it exists in the inventory. If it doesn't, add that ID to the inventory dict and add the given quantity. 
	// Otherwise, change quantity on existing ID.
	public void AddToInventory(Item item, int quantity) {
		int itemID = item.GetItemID();
		
		if (_inventory.ContainsKey(itemID)) {
			if ((_inventory[itemID].AsInt32() + quantity) >= 99) {
				_inventory[itemID] = 99;
			} else {
				_inventory[itemID] = _inventory[itemID].AsInt32() + quantity;
			}
		} else {
			if (quantity >= 99) {
				_inventory[itemID] = 99;
			} else {
				_inventory[itemID] = quantity;
			}
		}
	}
	
	// Removes an item from inventory. Not used.
	public void RemoveFromInventory(Item item, int quantity) {
		int itemID = item.GetItemID();
		
		if (_inventory.ContainsKey(itemID)) {
			if (_inventory[itemID].AsInt32() >= 1) {
				_inventory[itemID] = _inventory[itemID].AsInt32() - quantity;
			}
		} 
	}
	
	public override void _Ready() {
		GD.Print("--------");
		GD.Print("Player node initialized!");
	}
}
