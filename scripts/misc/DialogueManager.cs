using Godot;
using System;

public enum DialogueTypeStyle {
	Instant,
	Normal
};

public partial class DialogueManager : Label
{
	private String _text;
	
	private double _timer = 0.0;
	private double _textTime = 0.03;
	private String _iteratedText = "";
	private int _iterationIndex = 0;
	
	public bool Animating { get; private set; }
	
	public override void _Ready() {
		Animating = false;
	}
	
	public void SetDialogue(String input, DialogueTypeStyle dialogueType) {
		if (dialogueType == DialogueTypeStyle.Normal) {
			if (!Animating) {
				_text = input;
				_iteratedText = "";
				_iterationIndex = 0;
				Animating = true;
			}
		} else if (dialogueType == DialogueTypeStyle.Instant) {
			_text = input;
			_iteratedText = input;
			Text = input;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Animating || _text != _iteratedText) {
			_timer += delta;
			
			if (_timer >= _textTime) {
				_timer -= _textTime;
				
				_iteratedText += _text[_iterationIndex];
				Text = _iteratedText;
				_iterationIndex++;
				
				if (_text == _iteratedText) {
					Animating = false;
				}
			}
		}
	}
}
