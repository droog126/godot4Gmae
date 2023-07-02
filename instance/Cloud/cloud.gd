extends MeshInstance3D


@onready var gpu_particles_3d = $GPUParticles3D


var velocity = Vector3.ZERO
var acceleration = 50.0
var curGrass: Node3D = null;

func _ready():
	set_process(is_multiplayer_authority())
	cloud_find_grass_timer()
	pass


func _physics_process(delta):
	# 计算到目标位置的方向向量
	if curGrass:
		var grss2d = Vector2(curGrass.position.x, curGrass.position.z)
		var self2d = Vector2(position.x,position.z)
		var direction = (grss2d - self2d).normalized()
		var distance = grss2d.distance_to(self2d)
		
		if distance > 0.5:
			position += Vector3(direction.x,0,direction.y) * delta
			gpu_particles_3d.set_emitting(false)
		else:
			gpu_particles_3d.set_emitting(true)
			cloud_find_grass_timer()
			
func cloud_find_grass_timer():
	Utils.add_task('cloudFindGrass',randf_range(5.0,10.0),true,Callable(self,'cloud_find_grass'))
	pass
	
func cloud_find_grass():
	var nodes: Array = get_tree().get_nodes_in_group('grass')
	if nodes.size() > 0:
		var index = randi_range(0,nodes.size() - 1)
		while curGrass == nodes[index]:
			index = randi_range(0,nodes.size() - 1)
		curGrass = nodes[index]

	


