extends Node3D
@onready var camera = $Camera3D

var cameraMode: CameraMode = CameraMode.Static
enum CameraMode {
	Static,
	Debug,
	ThirdPerson,
	FirstPerson
}

var cameraTarget = null;
var cameraDevitation = Vector3();

func debug():
	camera.current = true;
	if Input.is_action_just_pressed('f2'):
		cameraMode = ( cameraMode + 1 ) % len(CameraMode)
		
func _input(event):
	debug()
	if cameraMode == CameraMode.Static:
		camera.rotation.x = 0
		rotation.y = 0
	elif cameraMode == CameraMode.Debug:
		if event is InputEventMouseMotion:
			rotate_y(-event.relative.x * .005)
			camera.rotate_x(-event.relative.y * .005)
			camera.rotation.x = clamp(camera.rotation.x,-PI/2,PI/2)
	elif cameraMode == CameraMode.ThirdPerson:

		if cameraTarget == null:
			cameraMode = CameraMode.Static
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
			
			Log.debug(str('第三人称，没找到附身对象'))

		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

		if event is InputEventMouseMotion:	

			camera.rotate_x(-event.relative.y * .005)

#			rotation.x = clamp(camera.rotation.x,-PI/2,PI/2)
			pass

	
	elif cameraMode == CameraMode.FirstPerson:

		if cameraTarget == null:
			cameraMode = CameraMode.Static
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
	if cameraMode == CameraMode.Debug:
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
				
	elif cameraMode == CameraMode.ThirdPerson:
		global_position = cameraTarget.global_position + cameraTarget.transform.basis.z * 2 + Vector3(0,2,0)
		rotation.y = cameraTarget.rotation.y
		
		pass
	elif cameraMode == CameraMode.FirstPerson:
		global_transform.origin = cameraTarget.global_position + Vector3(0,0.4,-0.2).rotated(Vector3(0,1,0),cameraTarget.rotation.y)
		rotation.y = cameraTarget.rotation.y

		pass

