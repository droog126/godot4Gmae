extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func find_nodes_with_name(name: String, node: Node = null) -> Array:
	# 如果未指定节点，则默认从场景根节点开始遍历
	if node == null:
		node = get_tree().get_root()
	
	var found_nodes = []

	print('node_name:',node.name,'     class:',node.get_class(),'   groups:',node.get_groups())
	# 检查当前节点是否符合要求
	if node.name == name:

		found_nodes.append(node)

	# 遍历当前节点的所有子节点
	for child in node.get_children():
		# 递归搜索子节点
		var child_found = find_nodes_with_name(name, child)
		if child_found.size() > 0:
			found_nodes.append_array(child_found)

	return found_nodes


func traverse_scene_tree(scene: Node = null) -> void:
	# 如果未指定场景，则默认使用当前场景
	if scene == null:
		scene = get_tree().get_current_scene()

	print()
	# 遍历场景中的所有根节点
	for child in scene.get_children():
		# 对于每个节点，可以在这里执行你的代码
		print(child.get_name())

		# 递归遍历子节点
		traverse_scene_tree(child)


func joinArrayElements(array: Array, separator: String) -> String:
	var joinedString = ""

	for i in range(array.size()):
		joinedString += str(array[i])
		if i < array.size() - 1:
			joinedString += separator

	return joinedString


### 物理
# func has_lost_line_of_sight() -> bool:
# 	var space_state = get_world_3d().get_direct_space_state()
# 	var params = PhysicsRayQueryParameters3D.new()
# 	params.from = global_transform.origin + Vector3.UP
# 	params.to = player.global_transform.origin
# 	params.exclude = []
	
# 	params.collision_mask = 1
# 	var result = space_state.intersect_ray(params)
# 	return result

	


### 获取当前相机
func get_camera():
	# var from: Vector3 = camera.project_ray_origin(mouse_position)
	# var to: Vector3 = camera.project_ray_normal(mouse_position) * 1000
	# PhysicsRayQueryParameters3D.create(from, to)
	return get_viewport().get_camera_3d()



### 获取world_3d
func get_space_state():
	return get_tree().get_root().get_world_3d().direct_space_state


func get_ray_collision(pos:Vector2):
	var camera = get_camera()
	var ray_origin = camera.project_ray_origin(pos)
	var ray_direction = camera.project_ray_normal(pos)
	var space_state = get_space_state()
	if space_state:
		return space_state.intersect_ray(PhysicsRayQueryParameters3D.create(ray_origin,ray_origin+ray_direction*100))
	else: 
		return null



### 动态执行脚本
func load_and_execute_class(class_path: String):
	var script_resource = load(class_path)
	
	if script_resource:
		var instance = script_resource.new()
		if instance:
			instance.execute()


### 创建一个定时器

#	add_task('test', 5.0, false, Callable(self,"print_message").bind(["Task 1 executed!"]))
#	add_task('test2', 2.0, true, Callable(self,"print_message").bind(["Task 2 executed!"]))
var tasks: Array = []
func add_task(name: String, delay: float, one_shot: bool, callback: Callable):
	var timer = Timer.new()
	timer.wait_time = delay
	timer.name = name
	timer.one_shot = one_shot 

	timer.connect("timeout",callback)
	add_child(timer)
	tasks.append(timer)
	timer.start()
	return timer

func remove_task(timer):
	# 从数组中移除指定的Timer节点引用
	tasks.erase(timer)
	# 从场景树中移除Timer节点
	timer.queue_free()

			


### 从数组中随机选一个
func choose(choice):
	randomize()
	var rand_index = randi() % choice.size()
	return choice[rand_index]
	pass


### 获取鼠标的位置
func get_mouse_pos():
	return get_viewport().get_mouse_position()
	


### node - 实时创建一个线
func line(pos1: Vector3, pos2: Vector3, color = Color.WHITE_SMOKE) -> MeshInstance3D:
	var mesh_instance := MeshInstance3D.new()
	var immediate_mesh := ImmediateMesh.new()
	var material := ORMMaterial3D.new()
	
	mesh_instance.mesh = immediate_mesh
	# mesh_instance.cast_shadow = GeometryInstance3D.ShadowCastingSetting

	immediate_mesh.surface_begin(Mesh.PRIMITIVE_LINES, material)
	immediate_mesh.surface_add_vertex(pos1)
	immediate_mesh.surface_add_vertex(pos2)
	immediate_mesh.surface_end()	
	
	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	material.albedo_color = color
	
	get_tree().get_root().add_child(mesh_instance)
	
	return mesh_instance


func immediate_mesh_update(mesh:ImmediateMesh,pos1:Vector3,pos2:Vector3):
	pass
	var material := ORMMaterial3D.new()
	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	material.albedo_color = Color.WHITE_SMOKE
	mesh.clear_surfaces()
	mesh.surface_begin(Mesh.PRIMITIVE_LINES, material)
	mesh.surface_add_vertex(pos1)
	mesh.surface_add_vertex(pos2)
	mesh.surface_end()	
	pass
	


func point(pos:Vector3, radius = 0.05, color = Color.WHITE_SMOKE) -> MeshInstance3D:
	var mesh_instance := MeshInstance3D.new()
	var sphere_mesh := SphereMesh.new()
	var material := ORMMaterial3D.new()
		
	mesh_instance.mesh = sphere_mesh
	# mesh_instance.cast_shadow = false
	mesh_instance.position = pos
	
	sphere_mesh.radius = radius
	sphere_mesh.height = radius*2
	sphere_mesh.material = material
	
	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	material.albedo_color = color
	
	get_tree().get_root().add_child(mesh_instance)
	
	return mesh_instance
	
