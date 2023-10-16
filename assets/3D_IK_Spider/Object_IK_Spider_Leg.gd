extends Node3D

const DISTANCE_REQUIRED_FOR_IK_UPDATE = 1.5 * 1.5;
const MIN_LEG_DISTANCE = 0.25;

var current_ik_position = Vector3.ZERO;
var target_ik_position = Vector3.ZERO;

var ik_raycast : RayCast3D;
var ik_target : Node3D;

var move_to_target_ik = false;
var move_to_target_ik_speed = 22.0;

var is_in_air = false;


func _ready():
	ik_raycast = get_node("RayCast");
	ik_target = get_node("IK_Target");
	
	current_ik_position = ik_raycast.global_transform.origin + ik_raycast.target_position;


func _physics_process(delta):
	ik_raycast.force_raycast_update();
	if (ik_raycast.is_colliding() == true):
		target_ik_position = ik_raycast.get_collision_point();
		ik_target.global_transform.origin = current_ik_position;
		
		if (is_in_air == true):
			move_to_target_ik = true;
			current_ik_position = ik_raycast.global_transform.origin + ik_raycast.target_position;
			ik_target.global_transform.origin = current_ik_position;
			is_in_air = false;
	else:
		is_in_air = true;
	
	if (current_ik_position.distance_squared_to(target_ik_position) >= DISTANCE_REQUIRED_FOR_IK_UPDATE):
		move_to_target_ik = true;
	
	if (move_to_target_ik == true):
		current_ik_position = current_ik_position.lerp(target_ik_position, move_to_target_ik_speed * delta);
		if (current_ik_position.distance_to(target_ik_position) <= MIN_LEG_DISTANCE):
			move_to_target_ik = false;
	
