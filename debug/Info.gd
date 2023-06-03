extends Panel

@onready var debug_menu_status = $debugMenuStatus
@onready var viewpoint_status = $viewpointStatus


var debugModeMap = {
	0 : '关闭',
	1 : 'Cmd',
	2 : 'Opertor'
}
var cameraModeMap = {
	0 :
}

func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	debugMenuStatusUpdate()
	viewPortStatusUpdate()
	pass

func debugMenuStatusUpdate():
	var mode = get_parent().debugMenuMode
	debug_menu_status.text = str('debug菜单状态',debugModeMap[mode])
	pass
	
func viewPortStatusUpdate():
	var node = get_node('/root/GCamera')
	pass
