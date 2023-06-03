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
