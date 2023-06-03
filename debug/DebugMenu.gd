extends CanvasLayer


@onready var debug_menu = $"."
@onready var option_button = $Opertor/OptionButton
@onready var cmd = $Cmd
@onready var opertor = $Opertor

var MenuDebugMode = _G.MenuDebugMode
var opertorNodes:  Array = []

func _input(event):
	if event.is_action_pressed('tab'):
		_G.menuDebug_add_circle()
		CloseMenu()
		if _G.menuDebugMode == MenuDebugMode.Opertor:
			DebugMenuOpertorInit()
		elif _G.menuDebugMode == MenuDebugMode.Cmd:
			CmdMenuInit()

func _ready():
	pass


func CloseMenu():
	opertor.visible = false
	cmd.visible = false


func DebugMenuOpertorInit():
	opertor.visible = true
	var options = []
	option_button.clear()
	var player_nodes = get_tree().get_nodes_in_group("player")
	var debug_nodes = get_tree().get_nodes_in_group("debug")
	options += player_nodes
	options += debug_nodes
	print('here',options)
	
	for i in range(options.size()):
		var node = options[i]
		var index = i
		option_button.add_item(str(node.name,'-',index,'-',node.get_groups()),index)

	option_button.select(-1)

	if option_button.item_count  == 0:
		option_button.add_item('没有找到玩家')
	pass


func _on_option_button_item_selected(index):
	var selectedNode = opertorNodes[index];
	GCamera.cameraTarget = selectedNode;
	pass 


func CmdMenuInit():
	cmd.visible = true
	pass
	
func click_callback():

#	var deviation = 
	pass 



