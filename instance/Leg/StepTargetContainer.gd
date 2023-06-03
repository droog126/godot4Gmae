extends Node3D


@export var offset: float = 20.0

@onready var parent = get_parent_node_3d()
@onready var previous_position = parent.global_position

func _process(delta):
	# 相减 指向 首
	var dir = parent.global_position - previous_position
	global_position = parent.global_position + dir * offset
	previous_position = parent.global_position
	pass

#在Godot 4.0中，修改子节点的 global_position 不会影响其父节点的 global_position。
#
#global_position 是子节点相对于世界坐标系的位置。当您修改子节点的 global_position 时，Godot 会自动计算并更新其相对于父节点的局部位置（position），以保持子节点的全局位置。但是，这个操作不会影响到父节点的位置或者其他属性。
#
#如果需要通过移动子节点去影响兄弟节点或者父节点的位置，可以间接地修改父节点的 position 或者使用Tween节点或AnimationPlayer节点在一定时间范围内让子节点按某种方式运动，从而达到整体平滑运动的效果。当然这类操作在开发中并不常见，通常我们只调整子节点的局部位置。
