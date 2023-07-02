extends Node



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


