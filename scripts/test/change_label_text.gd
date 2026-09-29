extends Node


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Label.text = "i think ... i think i like the penis cereal"
	

func _input(event):
	if event.is_action_pressed("Down"):
		$Label.modulate = Color.RED
		
	if event.is_action_released("Down"):
		$Label.modulate = Color.WHITE
