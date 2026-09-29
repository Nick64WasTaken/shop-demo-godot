using Godot;
using System;
using System.Collections.Generic;

public partial class ItemManager : Node
{
	private string jsonLocation = "res://assets/json/items.json";
	
	// Holds all the items as a key/value pair (ID number, Item object)
	private Godot.Collections.Dictionary _items = new Godot.Collections.Dictionary();
	
	public override void _Ready()
	{
		GD.Print("--------");
		GD.Print("Initializing ItemManager...");
		InitializeJSON(jsonLocation);
		GD.Print("ItemManager initialized!");
	}
	
	
	// Initialize the given JSON 
	private bool InitializeJSON(String jsonLocation) {
		// Checks if the JSON file exists
		if (!(FileAccess.FileExists(jsonLocation))) {
			GD.PushError("JSON file does not exist!");
			GD.PrintErr("JSON file does not exist!");
			return false;
		}
		
		// Checks if the JSON file contains data
		string itemsString = FileAccess.GetFileAsString(jsonLocation);
		if (string.IsNullOrEmpty(itemsString))
		{
			GD.PushError("JSON file is empty!");
			GD.PrintErr("JSON file is empty!");
			return false;
		}
		
		var itemsJson = new Json();
		var itemsError = itemsJson.Parse(itemsString);
		
		// Checks for errors in the JSON. If failed, does not conintue to parse the JSON
		if (itemsError == Error.Ok) {
			InitializeItemsIntoDict(itemsJson.Data.AsGodotDictionary());
			GD.Print("Total items: " + Count());
		} else {
			GD.PushError("Failed to parse JSON file!");
			GD.PrintErr("Failed to parse JSON file!");
			return false;
		}
		
		return true;
	} 
	
	// Takes the Dictionary generated from the JSON file and creates Item objects, putting them in a new dictionary with (ID, Item) pairing
	public void InitializeItemsIntoDict(Godot.Collections.Dictionary itemsJsonDict) {
		foreach (var key in itemsJsonDict.Keys) {
			var itemDict = itemsJsonDict[key].AsGodotDictionary();
			Item item = new Item(key.AsInt32(), itemDict);
			_items[key] = item;
		}
	}
	
	public int Count() {
		return (int) _items.Count;
	}
	
	public bool Exists(int id) {
		Godot.Variant key = id.ToString();
		if (_items.ContainsKey(key)) {
			return true;
		}
		return false;
	}
	
	public Item GetItemByID(int id) {
		Godot.Variant key = id.ToString();
		if (_items.ContainsKey(key)) {
			return (Item) _items[key];
		} else {
			GD.PushError("Item with ID " + id + " does not exist in ItemManager!");
			GD.PrintErr("Item with ID " + id + " does not exist in ItemManager!");
			return null;
		}
	}
	
	public String GetNameFromID(int id) {
		Godot.Variant key = id.ToString();
		if (_items.ContainsKey(key)) {
			return ((Item) _items[key]).GetItemName();
		} else {
			GD.PushError("Item with ID " + id + " does not exist in ItemManager!");
			GD.PrintErr("Item with ID " + id + " does not exist in ItemManager!");
			return null;
		}
	}
	
	public String GetDescriptionFromID(int id) {
		Godot.Variant key = id.ToString();
		if (_items.ContainsKey(key)) {
			return ((Item) _items[key]).GetItemDescription();
		} else {
			GD.PushError("Item with ID " + id + " does not exist in ItemManager!");
			GD.PrintErr("Item with ID " + id + " does not exist in ItemManager!");
			return null;
		}
	}
}
