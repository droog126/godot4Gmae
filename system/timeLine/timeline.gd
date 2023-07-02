
extends Node


@export var value: = 0:
	set(_val):
		value = _val
@onready var label = $Label



func _ready():
	test()
	if !OS.is_debug_build():
		label.queue_free()


func _process(delta):
	if is_multiplayer_authority():
		value += 1
		
		for task in Utils.tasks:
			if task.one_shot && task.is_stopped():
				Utils.remove_task(task)
			pass

	if OS.is_debug_build():
		label.text = str(value)
	pass





func test():
	#Utils.add_task('test', 5.0, false, Callable(self,"print_message").bind(["Task 1 executed!"]))
	#Utils.add_task('test2', 2.0, true, Callable(self,"print_message").bind(["Task 2 executed!"]))
	pass
	
func print_message(args):
	print('时间到了',args);

