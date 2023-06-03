extends Panel


@onready var Status = $Status



func _ready():
	Status.add_theme_font_size_override('font_size',12)
	pass 


func _process(delta):
	var textArr = []
	textArr.push_back(str('调试菜单状态: ',_G.menuDebug_get_text()))
	textArr.push_back(str('相机状态: ',_G.cameraMode_get_text()))
	textArr.push_back(str('输入状态: ',_G.inputMode_get_text()))
	var text = Utils.joinArrayElements(textArr,'\n')
	Status.text = text
	pass
