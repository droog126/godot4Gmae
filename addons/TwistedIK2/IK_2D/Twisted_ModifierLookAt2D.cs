using Godot;
using System;

/// <summary>
/// A modifier that rotates a single Twisted_Bone2D node to look at a target node using the <c>look_at</c> function.
/// The simplest of all of the Twisted_Modifier2D classes and a good reference to use when making your own.
/// </summary>
[Tool]
public partial class Twisted_ModifierLookAt2D : Twisted_Modifier2D
{
    /// <summary>
    /// A NodePath to the Node2D-based node that is used as the target position
    /// </summary>
    public NodePath path_to_target;
    /// <summary>
    /// A reference to the Node2D-based node that is used as the target position
    /// </summary>
    public Node2D target_node;

    /// <summary>
    /// A NodePath to the Twisted_Bone2D node that this modifier operates on
    /// </summary>
    public NodePath path_to_twisted_bone;
    /// <summary>
    /// A reference to the Twisted_Bone2D node that this modifier operates on
    /// </summary>
    public Twisted_Bone2D twisted_bone;

    /// <summary>
    /// A float that stores a rotation offset (in radiens) that is applied after making the bone look at the target
    /// </summary>
    public float additional_rotation = 0;

    /// <summary>
    /// When <c>true</c>, constraints will be applied to keep the modifier result within the given min and max bounds.
    /// </summary>
    public bool constraint_angle_enabled = false;
    /// <summary>
    /// The minimum angle for the Twisted_Bone2D result. The angle is expected to be in radians.
    /// </summary>
    public float constraint_angle_min = 0;
    /// <summary>
    /// The maximum angle for the Twisted_Bone2D result. The angle is expected to be in radians.
    /// </summary>
    public float constraint_angle_max = Mathf.Pi * 2.0f;
    /// <summary>
    /// If <c>true</c>, the result will be constrained to the bounds that the minimum and maximum bounds are NOT.
    /// In other words, it constraints the result to the inverted space created by the min and max bounds.
    /// </summary>
    public bool constraint_angle_invert = false;
    /// <summary>
    /// If <c>true</c>, the constraint will be applied to the localspace angle (local rotation) rather than
    /// global rotation relative to the scene origin.
    /// </summary>
    public bool constraint_angle_in_localspace = true;

    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node2D>(path_to_target);
        }
        if (path_to_twisted_bone != null) {
            twisted_bone = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "look_at/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            return true;
        }
        else if (property == "look_at/twisted_bone") {
            path_to_twisted_bone = (NodePath)value;
            if (path_to_twisted_bone != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone);
            }
            return true;
        }
        else if (property == "look_at/additional_rotation") {
            additional_rotation = Mathf.DegToRad((float)value);
            return true;
        }
        else if (property == "look_at/constraint_angle_enabled") {
            constraint_angle_enabled = (bool)value;
            NotifyPropertyListChanged();
            update_gizmo();
            return true;
        }
        else if (property == "look_at/constraint_angle_min") {
            constraint_angle_min = Mathf.DegToRad((float)value);
            update_gizmo();
            return true;
        }
        else if (property == "look_at/constraint_angle_max") {
            constraint_angle_max = Mathf.DegToRad((float)value);
            update_gizmo();
            return true;
        }
        else if (property == "look_at/constraint_angle_invert") {
            constraint_angle_invert = (bool)value;
            update_gizmo();
            return true;
        }
        else if (property == "look_at/constraint_angle_in_localspace") {
            constraint_angle_in_localspace = (bool)value;
            update_gizmo();
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
        if (property == "look_at/target") {
            return path_to_target;
        }
        else if (property == "look_at/twisted_bone") {
            return path_to_twisted_bone;
        }
        else if (property == "look_at/additional_rotation") {
            return Mathf.RadToDeg(additional_rotation);
        }
        else if (property == "look_at/constraint_angle_enabled") {
            return constraint_angle_enabled;
        }
        else if (property == "look_at/constraint_angle_min") {
            return Mathf.RadToDeg(constraint_angle_min);
        }
        else if (property == "look_at/constraint_angle_max") {
            return Mathf.RadToDeg(constraint_angle_max);
        }
        else if (property == "look_at/constraint_angle_invert") {
            return constraint_angle_invert;
        }
        else if (property == "look_at/constraint_angle_in_localspace") {
            return constraint_angle_in_localspace;
        }

        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = base._GetPropertyList();
        Godot.Collections.Dictionary tmp_dict;

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "look_at/target");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "look_at/twisted_bone");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "look_at/additional_rotation");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "look_at/constraint_angle_enabled");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        if (constraint_angle_enabled == true) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "look_at/constraint_angle_min");
            tmp_dict.Add("type", (int)Variant.Type.Float);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "look_at/constraint_angle_max");
            tmp_dict.Add("type", (int)Variant.Type.Float);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "look_at/constraint_angle_invert");
            tmp_dict.Add("type", (int)Variant.Type.Bool);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "look_at/constraint_angle_in_localspace");
            tmp_dict.Add("type", (int)Variant.Type.Bool);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);
        }

        return list;
    }

    public override void _ExecuteModification(Twisted_ModifierStack2D modifier_stack, double delta)
    {
        base._ExecuteModification(modifier_stack, delta);

        if (target_node == null) {
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            if (target_node == null) {
                GD.PrintErr("Cannot execute 2D LookAt: No target found!");
                return;
            }
        }

        if (twisted_bone == null) {
            if (path_to_twisted_bone != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone);
            }
            if (twisted_bone == null) {
                GD.PrintErr("Cannot execute 2D LookAt: No Twisted_Bone2D found!");
                return;
            }
        }

        twisted_bone.set_executing_ik(true);

        twisted_bone.LookAt(target_node.GlobalTransform.Origin);
        twisted_bone.Rotate(additional_rotation);

        // Account for Bone2D rotation
        // Without doing this, the Bone gizmo won't point to the target, just the node will
        twisted_bone.Rotate(-twisted_bone.bone_angle);

        // Constrain?
        if (constraint_angle_enabled == true) {
            if (constraint_angle_in_localspace == true) {
                twisted_bone.Rotation = Twisted_2DFunctions.clamp_angle(
                    twisted_bone.Rotation, constraint_angle_min, constraint_angle_max, constraint_angle_invert);
            }
            else {
                twisted_bone.GlobalRotation = Twisted_2DFunctions.clamp_angle(
                    twisted_bone.GlobalRotation, constraint_angle_min, constraint_angle_max, constraint_angle_invert);
            }
        }
    }


    public override void _Draw()
    {
        base._Draw();
        if (Engine.IsEditorHint() == true) {
            if (gizmo_can_draw_in_editor == false) {
                return;
            }
        }
        else {
            if (gizmo_can_draw_in_game == false) {
                return;
            }
        }

        draw_angle_constraints(twisted_bone,
            constraint_angle_min, constraint_angle_max, constraint_angle_invert, 
            constraint_angle_in_localspace, constraint_angle_enabled);
    }
}