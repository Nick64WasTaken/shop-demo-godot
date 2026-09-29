using Godot;
using System;

public partial class ShopItem : Node
{
	private Item _item;
	private int _price = 000;
	private int _quantity = 0;
	private bool _selected = false;
	
	private Node3D _modelInstance;
	private PlayerManager _playerManager;
	//private SubViewport _viewport;
	
	[Export]
	private Label nameLabel;
	[Export]
	private Label priceLabel;
	[Export]
	private Label quantityLabel;
	[Export]
	private ColorRect selectionBox;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var _modelScene = GD.Load<PackedScene>(_item.GetModelPath());
		_modelInstance = _modelScene.Instantiate<Node3D>();
		_modelInstance.Position = new Vector3(-30, 0, -7);
		_modelInstance.RotateX(0.35f);
		_modelInstance.Visible = false;
		AddChild(_modelInstance);
		
		_playerManager = GetNode<PlayerManager>("/root/PlayerManager");
		
		selectionBox.Visible = false;
		
		nameLabel.Text = _item.GetItemName();
		priceLabel.Text = ConvertMoneyIntToDecimal(_price).ToString();
		UpdateInventoryQuantity();
	}
	
	public void SetPrice(int price) {
		_price = price;
	}
	
	private decimal ConvertMoneyIntToDecimal(int money) {
		return money / 100m;
	}
	
	public int GetPrice() {
		return _price;
	}
	
	public void UpdateInventoryQuantity() {
		quantityLabel.Text = _playerManager.GetItemQuantityFromInventory(_item).ToString();
	}
	
	
	public void SetSelected(bool selected) {
		_selected = selected;
		if (selected) {
			selectionBox.Visible = true;
			_modelInstance.Visible = true;
		} else {
			selectionBox.Visible = false;
			_modelInstance.Visible = false;
		}
	}
	
	public void SetShopItem(Item item, int price) {
		_item = item;
		SetPrice(price);
	}
	
	public String GetItemDescription() {
		return _item.GetItemDescription();
	}
	
	public String GetItemName() {
		return _item.GetItemName();
	}
	
	public int GetItemID() {
		return _item.GetItemID();
	}
	
	public override void _Process(double delta) {
		if (_modelInstance != null) {
			_modelInstance.RotateY((float)delta);
		}
	}
}
