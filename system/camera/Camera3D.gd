extends Node3D
@onready var camera = $Camera3D
var CameraMode = _G.CameraMode

func _ready():
	_G.singletonNodeGroup['cameraNode'] = self

func debug():
	camera.current = true;
	if Input.is_action_just_pressed('f2'):
		_G.cameraMode_add_circle() 
	if  Input.is_action_just_pressed('mouse_right'):
		if _G.cameraMode!= CameraMode.Static:
			_G.cameraMode = CameraMode.Static
		else:
			_G.cameraMode = CameraMode.Debug

		
func _input(event):
	debug()
	if _G.cameraMode == CameraMode.Static:
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		pass
	elif _G.cameraMode == CameraMode.Debug:
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
		if event is InputEventMouseMotion:
			rotate_y(-event.relative.x * .005)
			camera.rotate_x(-event.relative.y * .005)
			camera.rotation.x = clamp(camera.rotation.x,-PI/2,PI/2)
	elif _G.cameraMode == CameraMode.ThirdPerson:
		if _G.cameraTarget == null:
			_G.cameraMode = CameraMode.Static
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
			print("没有找到附身对象")
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
		if event is InputEventMouseMotion:	
			camera.rotate_x(-event.relative.y * .005)
			pass
	elif _G.cameraMode == CameraMode.FirstPerson:
		if _G.cameraTarget == null:
			_G.cameraMode = CameraMode.Static
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
			print("没有找到附身对象")
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
		if event is InputEventMouseMotion:
			camera.rotate_x(-event.relative.y * .005)
			camera.rotation.x = clamp(camera.rotation.x,-PI/2,PI/2)
			pass


var mouse_switch = false
func _process(delta):
	if _G.cameraMode == CameraMode.Debug:
	#	print("here",camera.global_rotation,global_rotation,global_transform.basis)
		var x = Input.get_axis("a","d");
		var z = Input.get_axis("w","s");
		var y = Input.get_axis("shift","space");
	#	var direction = Vector3(x,y,z); 火车
		# 含义是啥
		var direction = (transform.basis * Vector3(x,y,z)).normalized()
		global_transform.origin += direction * .05;	
	elif _G.cameraMode == CameraMode.ThirdPerson:
		global_position = _G.cameraTarget.global_position + Vector3(0,1,1) * 5
		look_at(_G.cameraTarget.global_position);
		pass
	elif _G.cameraMode == CameraMode.FirstPerson:
		global_transform.origin = _G.cameraTarget.global_position + Vector3(0,0.4,-0.2).rotated(Vector3(0,1,0),_G.cameraTarget.rotation.y)
		rotation.y = _G.cameraTarget.rotation.y
		pass

