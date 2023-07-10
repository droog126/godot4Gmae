extends Node

func _ready():
	pankuRegister('_G',self)



## 只存储，或者一些非常简单的逻辑，不使用该节点方法的信息
func _process(delta):
	if Input.is_action_pressed('f1'):
		inputMode_add_circle()
	pass




var singletonNodeGroup:Dictionary = {
	"cameraNode": null,
}

### inputMode
var inputModeMap = {
	0 : '菜单输入',
	1 : '游戏输入'
}
enum InputMode {
	Menu,
	Game,
}
var inputMode = InputMode.Menu
func inputMode_add_circle():
	inputMode = (inputMode + 1) % InputMode.size()

func inputMode_get_text():
	return inputModeMap[inputMode]




### cameraMode
var cameraModeMap = {
	0 : '静止',
	1 : '随意移动',
	2 : '第二人称',
	3 : '第一人称'
}
var cameraMode: CameraMode = CameraMode.Static
var cameraTarget = null
enum CameraMode {
	Static,
	Debug,
	ThirdPerson,
	FirstPerson
}

func cameraMode_add_circle():
	cameraMode = (cameraMode + 1) % CameraMode.size()
	if cameraMode == CameraMode.Debug :
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	if !cameraTarget:
		cameraTarget = get_tree().get_nodes_in_group("debug")[0]
		
func cameraMode_get_text():
	return cameraModeMap[cameraMode]



###  menuDebug
enum MenuDebugMode{
	Close,
	Cmd,
	Opertor
}

var menuDebugModeMap = {
	0 : '关闭',
	1 : 'Cmd',
	2 : 'Opertor'
}

var menuDebugMode = MenuDebugMode.Close
func menuDebug_add_circle():
	menuDebugMode = (menuDebugMode + 1) % MenuDebugMode.size()
	if menuDebugMode == MenuDebugMode.Close:
		inputMode = InputMode.Game
	else:
		inputMode = InputMode.Menu
		cameraMode = CameraMode.Static
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)


func menuDebug_get_text():
	return menuDebugModeMap[menuDebugMode]


### gameMenu


func pankuRegister(name, space):
	if has_node(PankuConsole.SingletonPath):
		var console:PankuConsole = get_node(PankuConsole.SingletonPath)
		console.gd_exprenv.register_env(name, space)


class debug:	
	static func getDebugInfo():
		var textArr = []
		textArr.push_back(str('调试菜单状态: ',_G.menuDebug_get_text()))
		textArr.push_back(str('相机状态: ',_G.cameraMode_get_text()))
		textArr.push_back(str('输入状态: ',_G.inputMode_get_text()))
		var text = Utils.joinArrayElements(textArr,'\n')
		return text


