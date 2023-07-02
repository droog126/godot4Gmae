extends CanvasLayer


@onready var debug_menu = $"."
@onready var option_button = $Opertor/OptionButton

@onready var opertor = $Opertor

var MenuDebugMode = _G.MenuDebugMode
var SelectOptions:  Array = []

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



func DebugMenuOpertorInit():
	opertor.visible = true

	SelectOptions.clear()
	option_button.clear()
	var player_nodes = get_tree().get_nodes_in_group("player")
	var debug_nodes = get_tree().get_nodes_in_group("debug")
	SelectOptions += player_nodes
	SelectOptions += debug_nodes

	
	for i in range(SelectOptions.size()):
		var node = SelectOptions[i]
		var index = i
		option_button.add_item(str(node.name,'-',node.get_groups()),index)

	option_button.select(-1)

	if option_button.item_count  == 0:
		option_button.add_item('没有找到玩家')
	pass


func _on_option_button_item_selected(index):
	var selectedNode = SelectOptions[index];
	_G.cameraTarget = selectedNode;
	pass 


func CmdMenuInit():

	pass
	
func click_callback():

#	var deviation = 
	pass 



