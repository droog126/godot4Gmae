extends Node
enum InputMode {
	MENU_MODE,
	GAME_MODE,
}

var input_mode: InputMode = InputMode.MENU_MODE

func _process(delta):
	if Input.is_action_pressed('f1'):
		input_mode = (input_mode + 1) % 2
	


var player_info = {}
var my_info = { name = "Johnson Magenta", favorite_color = Color8(255, 0, 255) }
@rpc("any_peer")	
func register_player(info):
	var id = multiplayer.get_remote_sender_id()
	player_info[id] = info
	Log.debug(str(multiplayer.get_remote_sender_id(),'要求我调用了这个东西',multiplayer.get_unique_id()))
	
