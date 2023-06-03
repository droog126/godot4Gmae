extends Node



## 只存储，或者一些非常简单的逻辑，不使用该节点方法的信息

var singletonNodeGroup:Dictionary = {
	"cameraNode": null,
}



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

func cameraMode_get_text():
	return cameraModeMap[cameraMode]



##  menuDebug
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

func menuDebug_get_text():
	return menuDebugModeMap[menuDebugMode]



## inputMode
