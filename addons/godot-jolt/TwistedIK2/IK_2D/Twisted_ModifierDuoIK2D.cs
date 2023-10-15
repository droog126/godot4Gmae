using Godot;
using System;

/// <summary>
/// Rotates two bones to reach the target. This is an alternate version/implementation of TwoBoneIK that doesn't
/// rely as heavily on the law of Cosigns for placement. This solver should be a little more performant than
/// the TwoBoneIK solver. For 2D, the result doesn't have much difference than TwoBoneIK.
/// NOTE: This IK solver works with any scale, but the scale has to be uniform (X value = Y value)
/// </summary>
[Tool]
public partial class Twisted_ModifierDuoIK2D : Twisted_Modifier2D
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
        if (property == "duo_ik/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            return true;
        }
        else if (property == "duo_ik/twisted_bone_one") {
            path_to_twisted_bone_one = (NodePath)value;
            if (path_to_twisted_bone_one != null) {
                twisted_bone_one = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_one);
            }
            return true;
        }
        else if (property == "duo_ik/twisted_bone_two") {
            path_to_twisted_bone_two = (NodePath)value;
            if (path_to_twisted_bone_two != null) {
                twisted_bone_two = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_two);
            }
            return true;
        }
        else if (property == "duo_ik/bone_one_additional_rotation") {
            bone_one_additional_rotation = Mathf.DegToRad((float)value);
            return true;
        }
        else if (property == "duo_ik/bone_two_additional_rotation") {
            bone_two_additional_rotation = Mathf.DegToRad((float)value);
            return true;
        }
        else if (property == "duo_ik/flip_bend_direction") {
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
        if (property == "duo_ik/target") {
            return path_to_target;
        }
        else if (property == "duo_ik/twisted_bone_one") {
            return path_to_twisted_bone_one;
        }
        else if (property == "duo_ik/twisted_bone_two") {
            return path_to_twisted_bone_two;
        }
        else if (property == "duo_ik/bone_one_additional_rotation") {
            return Mathf.RadToDeg(bone_one_additional_rotation);
        }
        else if (property == "duo_ik/bone_two_additional_rotation") {
            return Mathf.RadToDeg(bone_two_additional_rotation);
        }
        else if (property == "duo_ik/flip_bend_direction") {
            return flip_bend_direction;
        }
        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = base._GetPropertyList();
        Godot.Collections.Dictionary tmp_dict;

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "duo_ik/target");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "duo_ik/twisted_bone_one");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "duo_ik/twisted_bone_two");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "duo_ik/bone_one_additional_rotation");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "duo_ik/bone_two_additional_rotation");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "duo_ik/flip_bend_direction");
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
                GD.PrintErr("Duo IK 2D: Cannot find target!");
                return;
            }
        }
        if (twisted_bone_one == null) {
            twisted_bone_one = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_one);
            if (twisted_bone_one == null) {
                GD.PrintErr("Duo IK 2D: Cannot find twisted_bone one!");
                return;
            }
        }
        if (twisted_bone_two == null) {
            twisted_bone_two = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone_two);
            if (twisted_bone_two == null) {
                GD.PrintErr("Duo IK 2D: Cannot find twisted_bone two!");
                return;
            }
        }

        // The IK algorithm
        // =================
        twisted_bone_one.set_executing_ik(true);
        twisted_bone_two.set_executing_ik(true);

        Transform2D bone_one_global_pose = Twisted_2DFunctions.world_transform_to_global_pose(twisted_bone_one.GlobalTransform, modifier_stack.skeleton);
        Transform2D bone_two_global_pose = Twisted_2DFunctions.world_transform_to_global_pose(twisted_bone_two.GlobalTransform, modifier_stack.skeleton);
        Transform2D target_global_pose = Twisted_2DFunctions.world_transform_to_global_pose(target_node.GlobalTransform, modifier_stack.skeleton);

        twisted_bone_one.GlobalTransform = bone_one_global_pose;
        twisted_bone_two.GlobalTransform = bone_two_global_pose;
        target_node.GlobalTransform = target_global_pose;

        Vector2 bone_one_scale = twisted_bone_one.Scale;
        Vector2 bone_two_scale = twisted_bone_two.Scale;

        // Make the bones straight
        twisted_bone_one.LookAt(target_node.GlobalTransform.Origin);
        twisted_bone_one.Rotate(-twisted_bone_one.bone_angle);
        twisted_bone_two.LookAt(target_node.GlobalTransform.Origin);
        twisted_bone_two.Rotate(-twisted_bone_two.bone_angle);
        
        Vector2 target_difference = target_node.GlobalTransform.Origin - twisted_bone_one.GlobalTransform.Origin;
        float joint_one_to_target = target_difference.Length();
        float bone_one_length = twisted_bone_one.bone_length;
        float bone_two_length = twisted_bone_two.bone_length;
        float bone_length_min = Mathf.Min(bone_one_length, bone_two_length);

        bool override_angles_due_to_out_of_range = false;

        if (bone_one_length + bone_two_length < joint_one_to_target) {
            override_angles_due_to_out_of_range = true;
        }
        else if (joint_one_to_target < bone_length_min) {
            joint_one_to_target = bone_length_min;
        }

        if (override_angles_due_to_out_of_range == false) {
            // Use Law of Cosigns to figure out the first angle (only one we need!)
            float angle_0 = Mathf.Acos(((joint_one_to_target * joint_one_to_target) + (bone_one_length * bone_one_length) - (bone_two_length * bone_two_length)) / (2.0f * joint_one_to_target * bone_one_length));
            if (Mathf.IsNaN(angle_0) == true) {
                return; // Cannot solve this angle!
            }

            Vector2 middle_point = twisted_bone_one.GlobalTransform.Origin;

            // Get the axis of rotation:
            Vector2 bone_direction = new Vector2(Mathf.Cos(twisted_bone_one.GlobalRotation + twisted_bone_one.bone_angle), Mathf.Sin(twisted_bone_one.GlobalRotation + twisted_bone_one.bone_angle));
            if (flip_bend_direction == true) {
                middle_point += (bone_direction.Rotated(angle_0)) * bone_one_length;
            } else {
                middle_point += (bone_direction.Rotated(-angle_0)) * bone_one_length;
            }

            twisted_bone_one.LookAt(middle_point);
            twisted_bone_one.Rotate(-twisted_bone_one.bone_angle);
            // Additional rotation
            twisted_bone_one.Rotate(bone_one_additional_rotation);

            twisted_bone_two.LookAt(target_node.GlobalTransform.Origin);
            twisted_bone_two.Rotate(-twisted_bone_two.bone_angle);
            // Additional rotation
            twisted_bone_two.Rotate(bone_two_additional_rotation);
        }
        else {
            twisted_bone_one.LookAt(target_node.GlobalTransform.Origin);
            twisted_bone_one.Rotate(-twisted_bone_one.bone_angle);
            // Additional rotation
            twisted_bone_one.Rotate(bone_one_additional_rotation);

            twisted_bone_two.LookAt(target_node.GlobalTransform.Origin);
            twisted_bone_two.Rotate(-twisted_bone_two.bone_angle);
            // Additional rotation
            twisted_bone_two.Rotate(bone_two_additional_rotation);
        }

        twisted_bone_one.Scale = bone_one_scale;
        twisted_bone_two.Scale = bone_two_scale;

        bone_one_global_pose = twisted_bone_one.GlobalTransform;
        bone_two_global_pose = twisted_bone_two.GlobalTransform;
        target_global_pose = target_node.GlobalTransform;

        twisted_bone_one.GlobalTransform = Twisted_2DFunctions.global_pose_to_world_transform(bone_one_global_pose, modifier_stack.skeleton);
        twisted_bone_two.GlobalTransform = Twisted_2DFunctions.global_pose_to_world_transform(bone_two_global_pose, modifier_stack.skeleton);
        target_node.GlobalTransform = Twisted_2DFunctions.global_pose_to_world_transform(target_global_pose, modifier_stack.skeleton);
        // ==============
    }
}