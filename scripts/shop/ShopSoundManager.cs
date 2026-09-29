using Godot;
using System;

public partial class ShopSoundManager : Node
{
	[Export]
	public AudioStream backgroundMusic;
	[Export]
	public AudioStream shopMenuMoveSound;
	[Export]
	public AudioStream shopBuySound;
	[Export]
	public AudioStream errorSound;
	
	[ExportGroup("Debug Settings")]
	[Export]
	public bool EnableBGM = true;
	[Export]
	public bool EnableAudio = true;
	
	private AudioStreamPlayer _bgmStream;
	private AudioStreamPlayer _menuMoveSoundStream;
	private AudioStreamPlayer _buySoundStream;
	private AudioStreamPlayer _errorSoundStream;
	
	public void PlayMenuMoveSound() {
		if (EnableAudio) {
			_menuMoveSoundStream.Play();
		}
	}
	
	public void PlayBuySound() {
		if (EnableAudio) {
			_buySoundStream.Play();
		}
	}
	
	public void PlayErrorSound() {
		if (EnableAudio) {
			_errorSoundStream.Play();
		}
	}
	
	public void PlayBGM() {
		if (EnableAudio) {
			if (EnableBGM) {
				_bgmStream.Play();
			}
		}
	}
	
	public void StopBGM() {
		_bgmStream.Stop();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bgmStream = new AudioStreamPlayer();
		_menuMoveSoundStream = new AudioStreamPlayer();
		_buySoundStream = new AudioStreamPlayer();
		_errorSoundStream = new AudioStreamPlayer();
		AddChild(_bgmStream);
		AddChild(_menuMoveSoundStream);
		AddChild(_buySoundStream);
		AddChild(_errorSoundStream);
		
		_bgmStream.Stream = backgroundMusic;
		_errorSoundStream.Stream = errorSound;
		_menuMoveSoundStream.Stream = shopMenuMoveSound;
		_buySoundStream.Stream = shopBuySound;
	}
}
