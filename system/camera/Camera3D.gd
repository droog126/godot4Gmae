extends Node3D
@onready var camera = $Camera3D





var CameraMode = _G.CameraMode

func _ready():
	_G.singletonNodeGroup['cameraNode'] = self

func debug():
	camera.current = true;
	if Input.is_action_just_pressed('f2'):
		_G.cameraMode_add_circle() 
		
		
func _input(event):
	debug()
	if _G.cameraMode == CameraMode.Static:
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		pass
	elif _G.cameraMode == CameraMode.Debug:
		if event is InputEventMouseMotion:
			rotate_y(-event.relative.x * .005)
			camera.rotate_x(-event.relative.y * .005)
			camera.rotation.x = clamp(camera.rotation.x,-PI/2,PI/2)
	elif _G.cameraMode == CameraMode.ThirdPerson:

		if _G.cameraTarget == null:
			_G.cameraMode = CameraMode.Static
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
			
			Log.debug(str('第三人称，没找到附身对象'))

		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

		if event is InputEventMouseMotion:	

			camera.rotate_x(-event.relative.y * .005)

#			rotation.x = clamp(camera.rotation.x,-PI/2,PI/2)
			pass

	
	elif _G.cameraMode == CameraMode.FirstPerson:

		if _G.cameraTarget == null:
			_G.cameraMode = CameraMode.Static
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
			Log.debug(str('第一人称，没找到附身对象'))
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
		global_transform.origin += direction * .01;

		
		if Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
			mouse_switch = !mouse_switch
			if(mouse_switch):
				Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
			else:
				Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
				
	elif _G.cameraMode == CameraMode.ThirdPerson:
		global_position = _G.cameraTarget.global_position + _G.cameraTarget.transform.basis.z * 2 + Vector3(0,2,0)
		rotation.y = _G.cameraTarget.rotation.y
		
		pass
	elif _G.cameraMode == CameraMode.FirstPerson:
		global_transform.origin = _G.cameraTarget.global_position + Vector3(0,0.4,-0.2).rotated(Vector3(0,1,0),_G.cameraTarget.rotation.y)
		rotation.y = _G.cameraTarget.rotation.y

		pass

