using Godot;
using System;

/// <summary>
/// A class that is responsible for taking a series of Twisted_Modifier3D nodes and executing them at the correct time.
/// </summary>
[Tool]
public partial class Twisted_ModifierStack3D : Node3D
{

    /// <summary>
    /// If <c>true</c>, the Twisted_Modifier3D child nodes will be executed
    /// </summary>
    public bool execution_enabled = false;
    private Godot.Collections.Array<Node> children_array = new Godot.Collections.Array<Node>();

    /// <summary>
    /// If <c>true</c>, then the modifier stack will enable itself in _ready (not in the editor though)
    /// </summary>
    public bool execute_on_ready = false;

    /// <summary>
    /// A NodePath to the Twisted_Skeleton3D node that contains the Twisted_Bone3D nodes that the modifiers in this stack use.
    /// </summary>
    public NodePath path_to_twisted_skeleton;
    /// <summary>
    /// A reference to the Twisted_Skeleton3D node that contains the Twisted_Bone3D nodes that the modifiers in this stack use.
    /// </summary>
    public Twisted_Skeleton3D twisted_skeleton;

    private bool has_waited_a_frame = false;

    public override void _Ready()
    {
        if (path_to_twisted_skeleton != null) {
            twisted_skeleton = GetNodeOrNull<Twisted_Skeleton3D>(path_to_twisted_skeleton);
        }

        if (execute_on_ready == true && Engine.IsEditorHint() == false) {
            execution_enabled = true;
        }

        update_modification_children_array();
    }

    /// <summary>
    /// Updates the array of modifiers. Modifiers have to be children of the Twisted_ModifierStack3D node and this function will
    /// update the internal list. This is generally not needed to be called manually, but if you add a modifier through code you will
    /// need to call this function once after adding it so the stack properly executes it.
    /// </summary>
    public void update_modification_children_array() {
        children_array.Clear();
        Godot.Collections.Array<Node> tmp_array = GetChildren();
        for (int i = 0; i < tmp_array.Count; i++) {
            if (tmp_array[i] as Twisted_Modifier3D != null) {
                children_array.Add(tmp_array[i]);
            }
        }
        children_array = GetChildren();
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "stack/twisted_skeleton") {
            path_to_twisted_skeleton = (NodePath)value;

            if (path_to_twisted_skeleton != null) {
                twisted_skeleton = GetNodeOrNull<Twisted_Skeleton3D>(path_to_twisted_skeleton);
            }
            return true;
        }
        else if (property == "stack/execution_enabled") {
            execution_enabled = (bool)value;
            return true;
        }
        else if (property == "stack/execute_on_ready") {
            execute_on_ready = (bool)value;
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
        if (property == "stack/twisted_skeleton") {
            return path_to_twisted_skeleton;
        }
        else if (property == "stack/execution_enabled") {
            return execution_enabled;
        }
        else if (property == "stack/execute_on_ready") {
            return execute_on_ready;
        }
        
        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        Godot.Collections.Dictionary tmp_dict;

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "stack/twisted_skeleton");
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

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "stack/execute_on_ready");
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
            if (twisted_skeleton == null) {
                GD.PrintErr("ERROR - Twisted_ModifierStack3D - No Twisted_Skeleton3D set!");
                return;
            }

            for (int i = 0; i < children_array.Count; i++) {
                if (children_array[i] as Twisted_Modifier3D != null) {
                    Twisted_Modifier3D child_mod = children_array[i] as Twisted_Modifier3D;
                    if (child_mod.execution_enabled == true && child_mod.execution_mode == TWISTED_IK_MODIFIER_MODES_3D._PROCESS) {
                        child_mod._ExecuteModification(this, delta);
                    }
                }
            }
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        if (execution_enabled == true) {
            if (twisted_skeleton == null) {
                GD.PrintErr("ERROR - Twisted_ModifierStack3D - No Twisted_Skeleton3D set!");
                return;
            }

            for (int i = 0; i < children_array.Count; i++) {
                if (children_array[i] as Twisted_Modifier3D != null) {
                    Twisted_Modifier3D child_mod = children_array[i] as Twisted_Modifier3D;
                    if (child_mod.execution_enabled == true && child_mod.execution_mode == TWISTED_IK_MODIFIER_MODES_3D._PHYSICS_PROCESS) {
                        child_mod._ExecuteModification(this, delta);
                    }
                }
            }
        }
    }
}
