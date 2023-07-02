extends CharacterBody3D

signal health_changed(health_value)

@onready var camera = $Camera3D
@onready var anim_player = $AnimationPlayer
#@onready var muzzle_flash = $Camera3D/Pistol/MuzzleFlash
@onready var raycast = $Camera3D/RayCast3D
@onready var people_3 = $people3
@onready var animation_player = $people3/AnimationPlayer
@export var player := 1 :
	set(id):
		player = id
		$PlayerInput.set_multiplayer_authority(id)




#ctrl+drag 
const SPEED = 3.0
const JUMP_VELOCITY = 4

var health = 3

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")


func _enter_tree():
#	Log.debug(str('enter_tree',position))
#	set_multiplayer_authority(str(name).to_int())
	pass
	
func _ready():
#	set_multiplayer_authority(str(name).to_int())
	if not is_multiplayer_authority(): return
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	camera.current = true

var input_dir = Vector2();
func _unhandled_input(event):
	if _G.inputMode != _G.InputMode.Game: return
	if not is_multiplayer_authority(): return
	
	input_dir = Input.get_vector("move_left", "move_right", "move_up", "move_down")
	if event is InputEventMouseMotion:
		rotate_y(-event.relative.x * .005)
		camera.rotate_x(-event.relative.y * .005)
		camera.rotation.x = clamp(camera.rotation.x,-PI/2,PI/2)


	if Input.is_action_just_pressed('mouse_left') \
		and anim_player.current_animation != 'shoot':

		play_shoot_effects.rpc()
		if raycast.is_colliding():
			var hit_player = raycast.get_collider()
			var id = hit_player.get_multiplayer_authority()
			hit_player.receive_damage.rpc_id(id)
		
func _physics_process(_delta):
	if not is_multiplayer_authority(): return
	
	# Add the gravity.
	if not is_on_floor():
		velocity.y =  move_toward(velocity.y,-gravity,SPEED/20)

	# Handle Jump.
	if _G.inputMode == _G.InputMode.Game and \
		Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = move_toward(velocity.y, JUMP_VELOCITY*2, SPEED*20)

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.

	var direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if direction:
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)

	if anim_player.current_animation == 'shoot':
		pass
	elif input_dir != Vector2.ZERO and is_on_floor():
		anim_player.play('move')
		animation_player.play("walk")
	else:
		anim_player.play('idle')
		animation_player.play("idle")

	move_and_slide()


@rpc("call_local")
func play_shoot_effects():
	anim_player.stop()
	anim_player.play('shoot')
#	muzzle_flash.restart()
#	muzzle_flash.emitting = true

@rpc("any_peer")
func receive_damage():
	health -= 1
	if health <= 0:
		health = 3
		position = Vector3.ZERO
	health_changed.emit(health)


func _on_animation_player_animation_finished(anim_name):
	if anim_name == "shoot":
		anim_player.play("idle")
