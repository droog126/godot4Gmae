#@tool
extends Node


@export var value: = 0:
	set(_val):
		value = _val
@onready var label = $Label


var tasks: Array = []

func _ready():
	test()
	if !OS.is_debug_build():
		label.queue_free()


func _process(delta):
	if is_multiplayer_authority():
		value += 1
		
		for task in tasks:
			if task.one_shot && task.is_stopped():
				remove_task(task)
			pass

	if OS.is_debug_build():
		label.text = str(value)
	pass




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

func test():
#	add_task('test', 5.0, false, Callable(self,"print_message").bind(["Task 1 executed!"]))
#	add_task('test2', 2.0, true, Callable(self,"print_message").bind(["Task 2 executed!"]))
	pass
	
func print_message(args):
	print('时间到了',args);

func remove_task(timer):
	# 从数组中移除指定的Timer节点引用
	tasks.erase(timer)
	# 从场景树中移除Timer节点
	timer.queue_free()

