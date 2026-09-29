using Godot;
using System;

public partial class DebugManager : Control
{
	[Export]
	private Button _add100Dollars;
	[Export]
	private Button _toggleBgm;
	[Export]
	private Button _clearInventory;
	
	private ShopSoundManager _shopSoundManager;
	private PlayerManager _player;
	private ShopManager _shopManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		_add100Dollars.Pressed += Add100Dollars;
		_toggleBgm.Pressed += ToggleBgm;
		
		_player = GetNode<PlayerManager>("/root/PlayerManager");
		_shopSoundManager = GetNode<ShopSoundManager>("/root/World/ShopUI/ShopSoundManager");
		_shopManager = GetNode<ShopManager>("/root/World/ShopUI");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Debug")) {
			Visible = !(Visible);
		}
	}
	
	private void Add100Dollars() {
		_player.AddMoney(10000);
		_shopManager.UpdateWalletLabel();
		GD.Print("DEBUG: Added 100 dollars to wallet");
	}
	
	private void ToggleBgm() {
		_shopSoundManager.EnableBGM = !(_shopSoundManager.EnableBGM);
		if (_shopSoundManager.EnableBGM) {
			_shopSoundManager.PlayBGM();
		} else {
			_shopSoundManager.StopBGM();
		}
		GD.Print("DEBUG: Toggled BGM");
	}
}
