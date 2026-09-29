using Godot;
using System;
using System.Collections.Generic;

public partial class ShopManager : Node
{
	[ExportGroup("Shop Settings")]
	[Export(PropertyHint.File, "*.json")]
	public string jsonLocation { get; set; }
	
	[Export]
	public double debounceTime { get; set; } = 0.00;
	
	// Game objects
	private PlayerManager _player;
	private ItemManager _itemManager;
	private ShopSoundManager _soundManager;
	private DialogueManager _dialogueManager;
	
	// Godot objects
	[ExportGroup("Shop Nodes")]
	[Export]
	private Label _playerWallet;
	[Export]
	private Label _dialogueLabel;
	[Export]
	private VBoxContainer _shopGrid;
	[Export]
	private PanelContainer _dialogueContainer;
	[Export]
	private HBoxContainer _controlsContainer;
	[Export]
	private AnimationPlayer _animationPlayer;
	
	[ExportGroup("Description Labels")]
	[Export]
	private Label _description;
	
	// Local variables
	private int _itemCount = 0;
	private int _selected = 0;
	private bool _descriptionVisible = false;
	private Godot.Collections.Dictionary _shopItemsDict;
	private List<ShopItem> _shopItemsList = new List<ShopItem>();
	
	private double _timer = 0.0;
	private double _waitUntilQuit = 2.0;
	private double _quitInitTime;
	private bool _quit = false;
	

	// Debounce variables
	private double _lastUpTime = -999;
	private double _lastDownTime = -999;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Grab main manager nodes
		_player = GetNode<PlayerManager>("/root/PlayerManager");
		_itemManager = GetNode<ItemManager>("/root/ItemManager");
		_soundManager = GetNode<ShopSoundManager>("ShopSoundManager");
		_dialogueManager = GetNode<DialogueManager>("VBoxContainer/MainContainer/ShopLeftMarginContainer/DialogueContainer/MarginContainer/Dialogue");
		
		GD.Print("--------");
		GD.Print("ShopManager initializing...");
		InitializeItemJSON(jsonLocation);
		InstantiateItemsIntoMenu();
		
		// Set current selected item to the first item in menu when loaded, set the description to that item, etc
		ChangeSelection(0);
		
		SetWalletLabel(_player.GetMoney());
		UpdateDescriptionScreen();
		AddControlsToShopUI();
		
		
		GD.Print("ShopManager initialized!");
		_soundManager.PlayBGM();
		_dialogueManager.SetDialogue("Welcome in.", DialogueTypeStyle.Normal);
	}
	
	public override void _Input(InputEvent @event)
	{
		double now = Time.GetTicksMsec() / 1000.0;
		
		if (@event.IsActionPressed("Down") && (now - _lastDownTime) >= debounceTime) {
			_lastDownTime = now;
			if (!(_selected >= (_itemCount - 1))) {
				_selected += 1;
				ChangeSelection(_selected);
				UpdateDescriptionScreen();
				_soundManager.PlayMenuMoveSound();
			}
		}
		
		if (@event.IsActionPressed("Up") && (now - _lastUpTime) >= debounceTime) {
			_lastUpTime = now;
			if (_selected > 0) {
				_selected -= 1;
				_soundManager.PlayMenuMoveSound();
			}
			ChangeSelection(_selected);
			UpdateDescriptionScreen();
		}
		
		if (@event.IsActionPressed("Description") && (now - _lastUpTime) >= debounceTime) {
			_lastUpTime = now;
			if (_descriptionVisible == true) {
				_descriptionVisible = false;
				if (!(_dialogueManager.Animating)) {
					SetDialogue("...");
				}
				_animationPlayer.Play("unblur_screen");
				
				_soundManager.PlayMenuMoveSound();
			} else {
				_animationPlayer.Play("blur_screen");
				_descriptionVisible = true;
				if (!(_dialogueManager.Animating)) {
					UpdateDescriptionScreen();
				}
				_soundManager.PlayMenuMoveSound();
			}
		}
		
		if (@event.IsActionPressed("Exit") && (now - _lastUpTime) >= debounceTime) {
			_lastUpTime = now;
			if (_descriptionVisible == true) {
				_descriptionVisible = false;
				_animationPlayer.Play("unblur_screen");
				
				_soundManager.PlayMenuMoveSound();
			} else {
				
				_soundManager.PlayMenuMoveSound();
				_dialogueManager.SetDialogue("Thanks for coming in.", DialogueTypeStyle.Normal);
				
				_quitInitTime = _timer;
				_quit = true;
			}
			
		}
		
		if (@event.IsActionPressed("Buy") && (now - _lastUpTime) >= debounceTime) {
			_lastUpTime = now;
			Buy(_selected);
		}
	}
	
	public override void _Process(double delta) {
		_timer += delta;
	
		if (_quit) {
			if ((_timer - _quitInitTime) >= _waitUntilQuit) {
				GetTree().Quit();
			}
		}
	}
	
	
	
	
	
	
	// Access JSON file and parse items
	public bool InitializeItemJSON(String jsonLocation) {
		
		// Checks if JSON files exists
		if (!(FileAccess.FileExists(jsonLocation))) {
			GD.PushError("JSON file does not exist!");
			return false;
		}
	
		// Read data as a String from the JSON
		string shopItemsString = FileAccess.GetFileAsString(jsonLocation);
		if (string.IsNullOrEmpty(shopItemsString))
		{
			GD.PushError("JSON file is empty!");
			return false;
		}
		
		// Initialize JSON and parse
		var shopItemsJson = new Json();
		var shopItemsError = shopItemsJson.Parse(shopItemsString);

		
		// Checks for errors in the JSON. If failed, does not conintue to parse the JSON
		if (shopItemsError == Error.Ok) {
			var shopItemsJsonDict = (shopItemsJson.Data.AsGodotDictionary())["items"].AsGodotDictionary();
			_itemCount = shopItemsJsonDict.Count;
			
			GD.Print("Total items in shop items JSON: " + _itemCount);

			if (_itemCount > 18) {
				GD.PushError("Too many items in shop JSON! Max is 18, there are " + _itemCount);
				return false;
			} 
			_shopItemsDict = shopItemsJsonDict;
		} else {
			GD.PushError("Failed to parse JSON file");
			return false;
		}
		
		return true;
	} 

	// Instantiates all the items from the JSON as ShopItem scenes into the shop UI
	public bool InstantiateItemsIntoMenu() {
		// Grabs VBoxContainer where items need to be instantiated
		
		_shopGrid = GetTree().GetFirstNodeInGroup("ItemList") as VBoxContainer;
		
		// Loads MenuItem scene and creates new instances based on how many items are in the JSON
		var shopItemScene = GD.Load<PackedScene>("res://scenes/shop/shop_item.tscn");
		
		GD.Print("Creating ShopItem instances...");
		
		// For each item in the dictionary of shop items, create a new instance in the menu and change the name and price to correspond with the JSON data
		foreach (int id in _shopItemsDict.Keys) {
			var shopItemInstance = shopItemScene.Instantiate<ShopItem>();
			Item item = _itemManager.GetItemByID(id);

			GD.Print("Instantiating ShopItem with ID " + id);
			if (_itemManager.Exists(id)) {
				shopItemInstance.SetShopItem(item, _shopItemsDict[id.ToString()].AsInt32());
				
				_shopGrid.AddChild(shopItemInstance);
				_shopItemsList.Add(shopItemInstance);
			} else {
				GD.PrintErr("Failed to initialize item with ID " + id + ", item does not exist!");
				_itemCount--;
			}
		}
		return true;
	}
	
	
	
	
	
	// Set all the items in the list to "not selected" and then set the given item index to "selected"
	public void ChangeSelection(int itemNumber) {
		foreach (var item in _shopItemsList) {
			item.SetSelected(false);
		}
		_shopItemsList[itemNumber].SetSelected(true);
	}
	
	// Purchaes the selected item from a given list index
	private void Buy(int selectedItem) {
		// Grab price and check if the player has enough money.
		int price = _shopItemsList[selectedItem].GetPrice();
		bool spendResult = _player.SpendMoney(price);
		
		// If the player has enough money, add the item to the player's inventory, play sound, and update the wallet label 
		if (spendResult) {
			_player.AddToInventory(_itemManager.GetItemByID(_shopItemsList[selectedItem].GetItemID()), 1);
			_soundManager.PlayBuySound();
			_shopItemsList[selectedItem].UpdateInventoryQuantity();
			//UpdateDescriptionScreen();
			SetWalletLabel(_player.GetMoney());
			_dialogueManager.SetDialogue("Thanks for shopping.", DialogueTypeStyle.Normal);
		} else {
			_soundManager.PlayErrorSound();
			_dialogueManager.SetDialogue("You're a bit short. Sorry, man.", DialogueTypeStyle.Normal);
			return;
		}
	}
	
	// Instantiates ActionPrompt scenes and sets the actionss
	public bool AddControlsToShopUI() {
		var actionPromptScene = GD.Load<PackedScene>("res://scenes/shop/action_prompt.tscn");
		
		var buyActionPromptInstance = actionPromptScene.Instantiate<ActionPrompt>();
		var descriptionActionPromptInstance = actionPromptScene.Instantiate<ActionPrompt>();
		var exitActionPromptInstance = actionPromptScene.Instantiate<ActionPrompt>();
		//var debugActionPromptInstance = actionPromptScene.Instantiate<ActionPrompt>();
	
		//_controlsContainer.AddChild(debugActionPromptInstance);
		_controlsContainer.AddChild(descriptionActionPromptInstance);
		_controlsContainer.AddChild(buyActionPromptInstance);
		_controlsContainer.AddChild(exitActionPromptInstance);
		

		buyActionPromptInstance.SetAction("Buy");
		descriptionActionPromptInstance.SetAction("Description");
		exitActionPromptInstance.SetAction("Exit");
		//debugActionPromptInstance.SetAction("Debug");
		return true;
	}
	
	private decimal ConvertMoneyIntToDecimal(int money) {
		return money / 100m;
	}
	
	private void SetWalletLabel(int money) {
		_playerWallet.Text = ("$" + (ConvertMoneyIntToDecimal(money)).ToString("0.00"));
	}
	
	public void UpdateWalletLabel() {
		SetWalletLabel(_player.GetMoney());
	}
	
	private void SetDialogue(String text) {
		_dialogueLabel.Text = text;
	}
	
	private void UpdateDescriptionScreen() {
		//_descriptionItemName.Text = _shopItemsList[_selected].GetItemName();
		if (_descriptionVisible) {
			_description.Text = _shopItemsList[_selected].GetItemDescription();
		}
		//_descriptionPrice.Text = ("$" + (ConvertMoneyIntToDecimal(_shopItemsList[_selected].GetPrice())).ToString("0.00"));
		//_descriptionInInventory.Text = "In inventory: " + _player.GetItemQuantityFromInventory(_itemManager.GetItemByID(_shopItemsList[_selected].GetItemID())).ToString(); 
	}
	
}
