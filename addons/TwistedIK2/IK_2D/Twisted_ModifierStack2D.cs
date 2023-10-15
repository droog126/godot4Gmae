using Godot;
using System;

/// <summary>
/// A class that is responsible for taking a series of Twisted_Modifier2D nodes and executing them at the correct time.
/// </summary>
[Tool]
public partial class Twisted_ModifierStack2D : Node2D
{

	/// <summary>
	/// If <c>true</c>, the Twisted_Modifier2D child nodes will be executed
	/// </summary>
	public bool execution_enabled = false;
	private Godot.Collections.Array<Node> children_array = new Godot.Collections.Array<Node>();

	/// <summary>
	/// A NodePath to the Skeleton2D node that contains the Twisted_Bone2D nodes that the modifiers in this stack use.
	/// </summary>
	public NodePath path_to_skeleton;
	/// <summary>
	/// A reference to the Skeleton2D node that contains the Twisted_Bone3D nodes that the modifiers in this stack use.
	/// </summary>
	public Skeleton2D skeleton;

	private bool has_waited_a_frame = false;

	public override void _Ready()
	{
		if (path_to_skeleton != null) {
			skeleton = GetNodeOrNull<Skeleton2D>(path_to_skeleton);
		}

		update_modification_children_array();
	}

	/// <summary>
	/// Updates the array of modifiers. Modifiers have to be children of the Twisted_ModifierStack2D node and this function will
	/// update the internal list. This is generally not needed to be called manually, but if you add a modifier through code you will
	/// need to call this function once after adding it so the stack properly executes it.
	/// </summary>
	public void update_modification_children_array() {
		children_array.Clear();
		Godot.Collections.Array<Node> tmp_array = GetChildren();
		for (int i = 0; i < tmp_array.Count; i++) {
			if (tmp_array[i] as Twisted_Modifier2D != null) {
				children_array.Add(tmp_array[i]);
			}
		}
		children_array = GetChildren();
	}

	public override bool _Set(StringName property, Variant value)
	{
		if (property == "stack/skeleton") {
			path_to_skeleton = (NodePath)value;

			if (path_to_skeleton != null) {
				skeleton = GetNodeOrNull<Skeleton2D>(path_to_skeleton);
			}
			return true;
		}
		else if (property == "stack/execution_enabled") {
			execution_enabled = (bool)value;
			return true;
		}
		
		try {
			return base._Set(property, value);
		} catch {
			return false;
		}
	}

	public override Variant _Get(StringName property)
	{
		if (property == "stack/skeleton") {
			return path_to_skeleton;
		}
		else if (property == "stack/execution_enabled") {
			return execution_enabled;
		}

		return base._Get(property);
	}

	public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
	{
		Godot.Collections.Array<Godot.Collections.Dictionary> list = new Godot.Collections.Array<Godot.Collections.Dictionary>();
		Godot.Collections.Dictionary tmp_dict;

		tmp_dict = new Godot.Collections.Dictionary();
		tmp_dict.Add("name", "stack/skeleton");
		tmp_dict.Add("type", (int)Variant.Type.NodePath);
		tmp_dict.Add("hint", (int)PropertyHint.None);
		tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
		list.Add(tmp_dict);

		tmp_dict = new Godot.Collections.Dictionary();
		tmp_dict.Add("name", "stack/execution_enabled");
		tmp_dict.Add("type", (int)Variant.Type.Bool);
		tmp_dict.Add("hint", (int)PropertyHint.None);
		tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
		list.Add(tmp_dict);

		return list;
	}

	public override void _Process(double delta)
	{
		if (has_waited_a_frame == false) {
			has_waited_a_frame = true;
			return;
		}

		if (execution_enabled == true) {
			if (skeleton == null) {
				GD.PrintErr("ERROR - Twisted_ModifierStack2D - No Skeleton2D set!");
				return;
			}

			for (int i = 0; i < children_array.Count; i++) {
				if (children_array[i] as Twisted_Modifier2D != null) {
					Twisted_Modifier2D child_mod = children_array[i] as Twisted_Modifier2D;
					if (child_mod.execution_enabled == true && child_mod.execution_mode == TWISTED_IK_MODIFIER_MODES_2D._PROCESS) {
						child_mod._ExecuteModification(this, delta);
					}
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (execution_enabled == true) {
			if (skeleton == null) {
				GD.PrintErr("ERROR - Twisted_ModifierStack2D - No Skeleton2D set!");
				return;
			}

			for (int i = 0; i < children_array.Count; i++) {
				if (children_array[i] as Twisted_Modifier2D != null) {
					Twisted_Modifier2D child_mod = children_array[i] as Twisted_Modifier2D;
					if (child_mod.execution_enabled == true && child_mod.execution_mode == TWISTED_IK_MODIFIER_MODES_2D._PHYSICS_PROCESS) {
						child_mod._ExecuteModification(this, delta);
					}
				}
			}
		}
	}
}
