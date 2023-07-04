extends Node3D
@export var move_speed: float = 5.0
@export var turn_speed: float = 1.0
@export var ground_offset: float = 4


@onready var left_ik_target = $modal/LeftIkTarget
@onready var right_ik_target = $modal/RightIkTarget
@onready var head = $Head
@onready var body = $Body
@onready var skeleton_3d = $modal/Skeleton3D

 
@onready var boneLeftId = skeleton_3d.find_bone("Leg2.L")
@onready var boneRightID = skeleton_3d.find_bone("Leg2.R")

var foot_pos = [null,null]



var feet_width = 10


func update_bone(id: int,pos:Vector3):
	skeleton_3d.set_bone_pose_position(id,pos)


func init_foot_pos():
	var x = global_position.x;
	var y = global_position.y;
	var z = global_position.z;
	for  i in range(2):
		var side = i * 2 - 1
		foot_pos[i]=Vector3(x +  feet_width * side, y +  feet_width * side,0)

func update_foot_pos():
	print("给个小脚移动的动画")
	for  i in range(2):
		var side = i * 2 - 1

	pass

var move_timer = 0.0
var move_duration = 0.5



func checkMoveInput(delta):
	var z_dir = Input.get_axis('move_down', 'move_up')
	var x_dir = Input.get_axis('move_right', 'move_left')
	if x_dir | z_dir:
		translate(Vector3(x_dir, 0, -z_dir) * move_speed * delta)
		move_timer += delta
		if move_timer >= move_duration:
			update_foot_pos()
			resetMoveTimer()
		else:
			resetMoveTimer()

func resetMoveTimer():
	move_timer = 0.0



func _ready():
	init_foot_pos()

	

func _physics_process(delta):
	update_foot_pos()
	pass





func _process(delta):
# 计算移动按下的时间
	checkMoveInput(delta)
	
	
#	var plane1 = Plane(bl_leg.global_position, fl_leg.global_position, fr_leg.global_position)
#	var plane2 = Plane(fr_leg.global_position, br_leg.global_position, bl_leg.global_position)
#	var avg_normal = ((plane1.normal + plane2.normal) / 2).normalized()
	
#	print('here',avg_normal)
#
#	var target_basis = _basis_from_normal(avg_normal)
#	transform.basis = lerp(transform.basis, target_basis, move_speed * delta).orthonormalized()
#
	var avg = (left_ik_target.position + right_ik_target.position) / 2
	var target_pos = avg + transform.basis.y * ground_offset
	var distance = transform.basis.y.dot(target_pos - position)
#	position = lerp(position, position + transform.basis.y * distance, move_speed * delta)



	



func _basis_from_normal(normal: Vector3) -> Basis:
	var result = Basis()
	print("here",transform.basis)
	result.x = normal.cross(transform.basis.z)
	result.y = normal
	result.z = transform.basis.x.cross(normal)

	result = result.orthonormalized()
	result.x *= scale.x 
	result.y *= scale.y 
	result.z *= scale.z 

	return result
