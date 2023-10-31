extends CharacterBody3D

const MAX_SPEED = 5;
const JUMP_SPEED = 7;
const ACCELERATION = 2;
const DECELERATION = 4;
const MAX_SLOPE_ANGLE = 45;
var gravity = -9.8;


var movement_direction = Vector3.ZERO;

var is_grounded = false;
var raycast_grounded : RayCast3D;

var camera_rot_y : Node3D;
var camera_rot_x : Node3D;
var camera : Camera3D;
const CAMERA_MIN_ROT_X = deg_to_rad(-85);
const CAMERA_MAX_ROT_X = deg_to_rad(20);

var dino_stuff : Node3D;


func _ready():
	camera_rot_y = get_node("Camera_Rot_Y");
	camera_rot_x = get_node("Camera_Rot_Y/Camera_Rot_X");
	camera = get_node("Camera_Rot_Y/Camera_Rot_X/Camera");
	
	dino_stuff = get_node("Dino_Stuff");
	
	raycast_grounded = get_node("RayCast_Grounded");
	is_grounded = false;
	
	# START IK
	get_node("Dino_Stuff/Twisted_ModifierStack3D").set("stack/execution_enabled", true);


func _physics_process(delta):	
	# Capture/uncapture mouse
	if (Input.is_action_just_pressed("escape") == true):
		if (Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED):
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE);
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED);
	
	handle_is_grounded();
	handle_movement(delta);


func handle_is_grounded():
	# Update is_grounded
	raycast_grounded.force_raycast_update();
	if (raycast_grounded.is_colliding() == true and is_grounded == false):
		is_grounded = true;
	else:
		is_grounded = raycast_grounded.is_colliding();


func handle_movement(delta):
	movement_direction = Vector3.ZERO;
	movement_direction.x = Input.get_action_strength("move_right") - Input.get_action_strength("move_left")
	movement_direction.z = Input.get_action_strength("move_down") - Input.get_action_strength("move_up")

	# Get the camera's transform basis, but remove the X rotation such
	# that the Y axis is up and Z is horizontal.
	var cam_basis = camera.global_transform.basis
	var basis = cam_basis.rotated(cam_basis.x, -cam_basis.get_euler().x)
	movement_direction = basis * movement_direction

	if movement_direction.length_squared() > 1:
		movement_direction /= movement_direction.length()
	
	velocity.y += delta * gravity
	
	var hvel = velocity
	hvel.y = 0
	
	var target = movement_direction * MAX_SPEED
	var acceleration
	if movement_direction.dot(hvel) > 0:
		acceleration = ACCELERATION
	else:
		acceleration = DECELERATION
	
	hvel = hvel.lerp(target, acceleration * delta)
	
	velocity.x = hvel.x
	velocity.z = hvel.z
	
	if (hvel.length_squared() > 0.2):
		dino_stuff.look_at(dino_stuff.global_transform.origin - hvel, Vector3.UP);
	
	if is_grounded == true and Input.is_action_pressed("jump"):
		velocity.y = JUMP_SPEED
		is_grounded = false;
	
	if(is_grounded == true):
		move_and_slide()
	else:
		move_and_slide()
	

func _input(event):
	if (event is InputEventMouseMotion and Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED):
		camera_rot_y.rotate_y(0.005 * event.relative.x);
		camera_rot_x.rotate_x(-0.005 * event.relative.y);
		camera_rot_x.rotation.x = clamp(camera_rot_x.rotation.x, CAMERA_MIN_ROT_X, CAMERA_MAX_ROT_X);
