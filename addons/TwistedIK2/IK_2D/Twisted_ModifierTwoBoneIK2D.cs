using Godot;
using System;

/// <summary>
/// A modifier that rotates two Twisted_Bone3D nodes using the Law of Cosigns.
/// </summary>
[Tool]
public partial class Twisted_ModifierTwoBoneIK2D : Twisted_Modifier2D
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
    /// A NodePath to the first Twisted_Bone2D node in this modifier
    /// </summary>
    public NodePath path_to_twisted_bone_one;
    /// <summary>
    /// A reference to the first Twisted_Bone2D node in this modifier
    /// </summary>
    public Twisted_Bone2D twisted_bone_one;

    /// <summary>
    /// A NodePath to the second Twisted_Bone2D node in this modifier
    /// </summary>
    public NodePath path_to_twisted_bone_two;
    /// <summary>
    /// A reference to the second Twisted_Bone2D node in this modifier
    /// </summary>
    public Twisted_Bone2D twisted_bone_two;

    /// <summary>
    /// The amount of additional rotation applied to the first Twisted_Bone2D node in this modifier.
    /// </summary>
    public float bone_one_additional_rotation = 0;
    /// <summary>
    /// The amount of additional rotation applied to the second Twisted_Bone2D node in this modifier.
    /// </summary>
    public float bone_two_additional_rotation = 0;

    /// <summary>
    /// If <c>true</c>, the joint will bend in the opposite direction when contracting.
    /// </summary>
    public bool flip_bend_direction = false;

    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node2D>(path_to_target);
        }
        if (twisted_bone_one != null) {
            twisted_bone_one = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_one);
        }
        if (twisted_bone_two != null) {
            twisted_bone_two = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_two);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "two_bone/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            return true;
        }
        else if (property == "two_bone/twisted_bone_one") {
            path_to_twisted_bone_one = (NodePath)value;
            if (path_to_twisted_bone_one != null) {
                twisted_bone_one = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_one);
            }
            return true;
        }
        else if (property == "two_bone/twisted_bone_two") {
            path_to_twisted_bone_two = (NodePath)value;
            if (path_to_twisted_bone_two != null) {
                twisted_bone_two = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_two);
            }
            return true;
        }
        else if (property == "two_bone/bone_one_additional_rotation") {
            bone_one_additional_rotation = Mathf.DegToRad((float)value);
            return true;
        }
        else if (property == "two_bone/bone_two_additional_rotation") {
            bone_two_additional_rotation = Mathf.DegToRad((float)value);
            return true;
        }
        else if (property == "two_bone/flip_bend_direction") {
            flip_bend_direction = (bool)value;
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
        if (property == "two_bone/target") {
            return path_to_target;
        }
        else if (property == "two_bone/twisted_bone_one") {
            return path_to_twisted_bone_one;
        }
        else if (property == "two_bone/twisted_bone_two") {
            return path_to_twisted_bone_two;
        }
        else if (property == "two_bone/bone_one_additional_rotation") {
            return Mathf.RadToDeg(bone_one_additional_rotation);
        }
        else if (property == "two_bone/bone_two_additional_rotation") {
            return Mathf.RadToDeg(bone_two_additional_rotation);
        }
        else if (property == "two_bone/flip_bend_direction") {
            return flip_bend_direction;
        }
        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = base._GetPropertyList();
        Godot.Collections.Dictionary tmp_dict;

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "two_bone/target");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "two_bone/twisted_bone_one");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "two_bone/twisted_bone_two");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "two_bone/bone_one_additional_rotation");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "two_bone/bone_two_additional_rotation");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "two_bone/flip_bend_direction");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        return list;
    }

    public override void _ExecuteModification(Twisted_ModifierStack2D modifier_stack, double delta)
    {
        base._ExecuteModification(modifier_stack, delta);

        if (target_node == null) {
            target_node = GetNodeOrNull<Node2D>(path_to_target);
            if (target_node == null) {
                GD.PrintErr("TwoBoneIK 2D: Cannot find target!");
                return;
            }
        }
        if (twisted_bone_one == null) {
            twisted_bone_one = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_one);
            if (twisted_bone_one == null) {
                GD.PrintErr("TwoBoneIK 2D: Cannot find twisted_bone one!");
                return;
            }
        }
        if (twisted_bone_two == null) {
            twisted_bone_two = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_two);
            if (twisted_bone_two == null) {
                GD.PrintErr("TwoBoneIK 2D: Cannot find twisted_bone two!");
                return;
            }
        }

        // The IK algorithm
        // =================
        twisted_bone_one.set_executing_ik(true);
        twisted_bone_two.set_executing_ik(true);
        
        Transform2D bone_one_trans = Twisted_2DFunctions.world_transform_to_global_pose(twisted_bone_one.GlobalTransform, modifier_stack.skeleton);
        Transform2D bone_two_trans = Twisted_2DFunctions.world_transform_to_global_pose(twisted_bone_two.GlobalTransform, modifier_stack.skeleton);
        Transform2D target_trans = Twisted_2DFunctions.world_transform_to_global_pose(target_node.GlobalTransform, modifier_stack.skeleton);

        Vector2 target_difference = target_trans.Origin - bone_one_trans.Origin;
        float joint_one_to_target = target_difference.Length();
        float angle_atan = Mathf.Atan2(target_difference.Y, target_difference.X);

        float bone_one_length = twisted_bone_one.bone_length;
        float bone_two_length = twisted_bone_one.bone_length;
        bool override_angles_due_to_out_of_range = false;

        if (bone_one_length + bone_two_length < joint_one_to_target) {
            override_angles_due_to_out_of_range = true;
        }

        if (!override_angles_due_to_out_of_range) {
            float angle_0 = Mathf.Acos(((joint_one_to_target * joint_one_to_target) + (bone_one_length * bone_one_length) - (bone_two_length * bone_two_length)) / (2.0f * joint_one_to_target * bone_one_length));
            float angle_1 = Mathf.Acos(((bone_two_length * bone_two_length) + (bone_one_length * bone_one_length) - (joint_one_to_target * joint_one_to_target)) / (2.0f * bone_two_length * bone_one_length));

            if (flip_bend_direction) {
                angle_0 = -angle_0;
                angle_1 = -angle_1;
            }

            if (Mathf.IsNaN(angle_0) || Mathf.IsNaN(angle_1)) {
                // We cannot solve for this angle! Do nothing to avoid setting the rotation (and scale) to NaN.
            } else {
                twisted_bone_one.Rotation = (angle_atan - angle_0 - twisted_bone_one.bone_angle);
                twisted_bone_two.Rotation = (-Mathf.Pi - angle_1 - twisted_bone_two.bone_angle + twisted_bone_one.bone_angle);
            }
        
        } else {
            // Out of range - so just use LookAt
            twisted_bone_one.LookAt(target_node.GlobalPosition);
            twisted_bone_one.Rotate(-twisted_bone_one.bone_angle);
            twisted_bone_two.LookAt(target_node.GlobalPosition);
            twisted_bone_two.Rotate(-twisted_bone_two.bone_angle);
        }

        // Additional rotation
        twisted_bone_one.Rotate(bone_one_additional_rotation);
        twisted_bone_two.Rotate(bone_two_additional_rotation);
        // ==============
    }
}