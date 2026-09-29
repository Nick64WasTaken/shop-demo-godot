using Godot;
using System;

public partial class ActionPrompt : Node
{
	private Label _controlText;
	
	private TextureRect _controlTextureRect;
	private ControlsManager _controlsManager;
	
	private String _action = "blank";
	
	public override void _Ready() {
		// Grab all the nodes from the tree
		_controlText = GetNode<Label>("HBoxContainer/Description");
		_controlTextureRect = GetNode<TextureRect>("HBoxContainer/Texture");
		_controlsManager = GetNode<ControlsManager>("/root/ControlsManager");
		
		_controlText.Text = _action;
		
		// Add OnInputTypeChanged to the InputChanged signal
		_controlsManager.InputChanged += OnInputTypeChanged;
	
		SetControlTexture(_action);
	}
	
	// Sets the action string (if the control text node is loaded, change the text), and set the texture for the corresponding input
	public void SetAction(String action) {
		_action = action;
		
		if (_controlText.Text != null) {
			_controlText.Text = _action;
		}
		SetControlTexture(_action);
	}
	
	// Change the texture to the appropriate texture when the input type changes
	private void OnInputTypeChanged(InputType type)
	{
		//GD.Print("Input changed to: " + type);
		SetControlTexture(_action);
	}
	
	// Set the control texture
	public void SetControlTexture(String input) {
		Texture2D texture = GD.Load<Texture2D>(GetTextureForInput(input));
		_controlTextureRect.Texture = texture;
	}
	
	// Checks the current input type and grabs the corresponding texture for whatever input is assigned to the action
	public String GetTextureForInput(String actionName) {
		var events = InputMap.ActionGetEvents(actionName);
		var device = _controlsManager.CurrentDevice;
		
		foreach (var ev in events) {
			if (device == InputType.Keyboard) {
				if (ev is InputEventKey key) {
					return $"res://assets/textures/input/keyboard/{key.PhysicalKeycode}.png";
				}
			} else if (device == InputType.Xbox) {
				if (ev is InputEventJoypadButton joy) {
					return $"res://assets/textures/input/xbox/{(int)joy.ButtonIndex}.png";
				}
			}
		}
		
		return "res://assets/textures/placeholder.png";
	}
}
