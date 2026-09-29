using Godot;
using System;
using System.Collections.Generic;

public partial class Item : GodotObject
{
	private int _id = 0;
	private String _name = "None";
	private String _description = "None";
	private String _modelLocation = "res://assets/3d/placeholder.glb";
	private int _itemPoints = 0;
	private Type _type;
	
	enum Type {
		Heal,
		Recover,
		Defense,
		Attack,
		Misc,
		Invalid
	}
	
	public String GetItemName() {
		return _name;
	}
	
	public String GetItemDescription() {
		return _description;
	}
	
	public int GetItemID() {
		return _id;
	}
	
	public String GetModelPath() {
		return _modelLocation;
	}
	
	public void PrintItemToConsole() {
		GD.Print("");
		GD.Print("Name: " + _name);
		GD.Print("Description: " + _description);
		GD.Print("Type: " + _type);
		GD.Print("Item Points: " + _itemPoints);
	}
	
	public Item(int id, Godot.Collections.Dictionary dict) {
		_id = id;
		
		try {
			_name = dict["name"].AsString();
		} catch (KeyNotFoundException) {
			_name = "None";
			GD.PushError("Name variable not found in JSON object. Set to None");
		}
		
		try {
			_description = dict["description"].AsString();
		} catch (KeyNotFoundException) {
			_description = "";
			GD.PushError("Description variable not found in JSON object. Set to blank");
		}
		
		try {
			_modelLocation = dict["model"].AsString();
		} catch (KeyNotFoundException e) {
			GD.PushError("No model found for " + _id + "! Using placeholder cube");
			GD.PrintErr("No model found for " + _id + "! Using placeholder cube");
		}

		try {
			string typeString = dict["type"].AsString();
			if (!Enum.TryParse(typeString, true, out _type)) {
				GD.PushError("Type invalid! Item and values set to invalid state");
				_type = Type.Invalid;
				_id = 0;
				_name = "Invalid";
				_description = "Invalid";
				_itemPoints = 0;
			}
		} catch (KeyNotFoundException) {
			GD.PushError("No type found! Item and values set to invalid state");
				_type = Type.Invalid;
				_id = 0;
				_name = "Invalid";
				_description = "Invalid";
				_itemPoints = 0;
		}
		
		try {
			if (_type != Type.Misc) {
				_itemPoints = dict["itemPoints"].AsInt32();
			}
		} catch (KeyNotFoundException) {
			GD.PrintErr("No model found for " + dict["name"] + "! Using placeholder cube");
			GD.PushError("No model found for " + dict["name"] + "! Using placeholder cube");
		}
		
	}
}
