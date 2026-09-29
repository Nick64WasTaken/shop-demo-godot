using Godot;
using System;
using System.Collections.Generic;

public enum InputType {
	Keyboard,
	Xbox,
	PlayStation,
	Nintendo
}

public partial class ControlsManager : Node
{
	[Signal]
	public delegate void InputChangedEventHandler(InputType type);
	public InputType CurrentDevice = InputType.Keyboard;
	
	public bool PrintUnhandledInput = false;
	
	public override void _Ready() {
		GD.Print("ControlsManager ready!");
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventKey keyEvent) {
			if (CurrentDevice != InputType.Keyboard) {
				CurrentDevice = InputType.Keyboard;
				EmitSignal(SignalName.InputChanged, (int)InputType.Keyboard);
				//GD.Print("Input set to keyboard");
			}
		} else if (@event is InputEventMouseButton mouseButton) {
			if (CurrentDevice != InputType.Keyboard) {
				CurrentDevice = InputType.Keyboard;
				EmitSignal(SignalName.InputChanged, (int)InputType.Keyboard);
				//GD.Print("Input set to keyboard");
			}
		} else if (@event is InputEventMouseMotion mouseMotion) {
			if (CurrentDevice != InputType.Keyboard) {
				CurrentDevice = InputType.Keyboard;
				EmitSignal(SignalName.InputChanged, (int)InputType.Keyboard);
				//GD.Print("Input set to keyboard");
			}
		} else if (@event is InputEventJoypadButton joyButton) {
			if (CurrentDevice != InputType.Xbox) {
				CurrentDevice = InputType.Xbox;
				EmitSignal(SignalName.InputChanged, (int)InputType.Xbox);
				//GD.Print("Input set to Xbox");
			}
		} else if (@event is InputEventJoypadMotion joyMotion) {
			if (CurrentDevice != InputType.Xbox) {
				CurrentDevice = InputType.Xbox;
				EmitSignal(SignalName.InputChanged, (int)InputType.Xbox);
				//GD.Print("Input set to Xbox");
			}
		}
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (PrintUnhandledInput) {
			if (@event is InputEventKey key && key.Pressed && !key.Echo)
			{
				GD.Print($"PhysicalKeycode: {key.PhysicalKeycode} | Keycode: {key.Keycode} | As string: {OS.GetKeycodeString(key.PhysicalKeycode)}");
			}

			if (@event is InputEventJoypadButton joyButton && joyButton.Pressed)
			{
				GD.Print($"Joypad button index: {(int)joyButton.ButtonIndex} | Device: {joyButton.Device} | Name: {Input.GetJoyName(joyButton.Device)}");
			}

			if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
			{
				GD.Print($"Mouse button index: {(int)mouseButton.ButtonIndex}");
			}
		}
	}
}
