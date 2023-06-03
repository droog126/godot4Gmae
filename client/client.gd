extends Node
@onready var main_menu = $LoginSystem/MainMenu
@onready var line_edit = $LoginSystem/MainMenu/MarginContainer/VBoxContainer/LineEdit
@onready var hud = $LoginSystem/HUD
@onready var health_bar = $LoginSystem/HUD/HealthBar


const PORT = 9999
var enet_peer = ENetMultiplayerPeer.new()
#const CUSTOM = Log.MAX << 1; # Bitwise left shift the MAX value for a custom level.
#const CUSTOM = Log.MAX << 1;
func _ready():
	Log.info('游戏开始，我是client')

func _unhandled_input(event):
	if Input.is_action_just_pressed('q'):
		get_tree().quit()




func _on_join_button_pressed():
	enet_peer.create_client('127.0.0.1',PORT)
	multiplayer.multiplayer_peer = enet_peer
	Log.debug(str('正在连接服务器',enet_peer))
	
	multiplayer.connected_to_server.connect(join_success)
	multiplayer.connection_failed.connect(join_failed)
	multiplayer.server_disconnected.connect(server_not_fuond)

func join_success():
	Log.debug(str('成功连接服务器'));
	main_menu.hide()
	hud.show()
	
func join_failed():
	Log.debug(str('连接失败'))

func server_not_fuond():
	Log.debug(str('服务器找不到'))







func update_health_bar(health_value):
	health_bar.value = health_value
	
func _on_multiplayer_spawner_spawned(node):	
	if node.is_multiplayer_authority():
		node.health_changed.connect(update_health_bar)
	


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


	
