extends Node
@onready var main_menu = $LoginSystem/MainMenu
@onready var line_edit = $LoginSystem/MainMenu/MarginContainer/VBoxContainer/LineEdit
@onready var hud = $LoginSystem/HUD
@onready var health_bar = $LoginSystem/HUD/HealthBar
@onready var players = $common/players


const SPAWN_RANDOM := 5.0
const PORT = 9999
var enet_peer = ENetMultiplayerPeer.new()

func _ready():
#	Log.info('游戏开始，我是mian')
	if DisplayServer.get_name() == "headless":
		_on_host_button_pressed.call_deferred()


func _unhandled_input(event):
	if Input.is_action_just_pressed('q'):
		get_tree().quit()


func _on_host_button_pressed():

	enet_peer.create_server(PORT)
	multiplayer.multiplayer_peer = enet_peer

#	Log.info(str('我开启了服务器'))
	multiplayer.peer_connected.connect(player_connect)
	multiplayer.peer_disconnected.connect(player_disconnect)
	
	
	# 进入游戏	
	main_menu.hide()
	hud.hide()
	change_level.call_deferred(load("res://level/level_main.tscn"))
	if not OS.has_feature("dedicated_server"):
		add_player(1)

func player_connect(peer_id):
#	Log.info(str('有玩家连接服务器',peer_id))
	add_player(peer_id)
#	rpc("register_player", Global.player_info)
#	Global.rpc("register_player", Global.player_info)
#	rpc_id(peer_id, "register_player", Global.player_info)
	
	
	

func player_disconnect(peer_id):
#	Log.info(str('有玩家离开了服务器',peer_id))
	del_player(peer_id)
	
	
	
func add_player(id: int):
#	Log.debug(str('添加一名玩家',id))
	var character = preload("res://instance/Player/player.tscn").instantiate()
	character.player = id
	var pos := Vector2.from_angle(randf() * 2 * PI)
	character.position = Vector3(pos.x * SPAWN_RANDOM * randf(), 100, pos.y * SPAWN_RANDOM * randf())
	character.name = str(id)
	players.add_child(character, true)

func del_player(id: int):
	if not players.has_node(str(id)):
		return 
	players.get_node(str(id)).queue_free()
	

func change_level(scene: PackedScene):
	var level = get_node('common/level')
	for c in level.get_children():
		level.remove_child(c)
		c.queue_free()
	level.add_child(scene.instantiate())




func upnp_setup():
	var upnp = UPNP.new()
	var discover_result = upnp.discover()
	
	assert(discover_result == UPNP.UPNP_RESULT_SUCCESS, \
		"UPNP Discover Failed! Error %s" % discover_result)

	print("here",upnp.get_gateway())
	assert(upnp.get_gateway() and upnp.get_gateway().is_valid_gateway(), \
		"UPNP Invalid Gateway!")

	var map_result = upnp.add_port_mapping(PORT)
	assert(map_result == UPNP.UPNP_RESULT_SUCCESS, \
		"UPNP Port Mapping Failed! Error %s" % map_result)
	
	print("Success! Join Address: %s" % upnp.query_external_address())

	
