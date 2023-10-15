using Godot;
using System;


[Tool]
public partial class Twisted_ModifierSliderJoint2D : Twisted_Modifier2D
{
    /// <summary>
    /// A NodePath to the Node2D-based node that is used as the target position
    /// </summary>
    public NodePath path_to_target = null;
    /// <summary>
    /// A reference to the Node2D-based node that is used as the target position
    /// </summary>
    public Node2D target_node = null;

    /// <summary>
    /// A NodePath to the first point that makes the 2D line
    /// </summary>
    public NodePath path_to_line_point_one = null;
    /// <summary>
    /// The Node2D node that makes the first point on the 2D line
    /// </summary>
    public Node2D line_point_one = null;

    /// <summary>
    /// A NodePath to the second point that makes the 2D line
    /// </summary>
    public NodePath path_to_line_point_two = null;
    /// <summary>
    /// The Node2D node that makes the second point on the 2D line
    /// </summary>
    public Node2D line_point_two = null;

    /// <summary>
    /// A NodePath to the Twisted_Bone2D that will be constrained to the line
    /// </summary>
    public NodePath path_to_twisted_bone = null;
    /// <summary>
    /// The Twisted_Bone2D that will be constrained to the line
    /// </summary>
    public Twisted_Bone2D twisted_bone = null;

    /// <summary>
    /// If true, the bone will rotate to look at the target
    /// </summary>
    public bool rotate_to_target = false;
    /// <summary>
    /// Additional rotation to be applied
    /// </summary>
    public float additional_rotation = 0.0f;


    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node2D>(path_to_target);
        }
        if (path_to_line_point_one != null) {
            line_point_one = GetNodeOrNull<Node2D>(path_to_line_point_one);
        }
        if (path_to_line_point_two != null) {
            line_point_two = GetNodeOrNull<Node2D>(path_to_line_point_two);
        }
        if (path_to_twisted_bone != null) {
            twisted_bone = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "SliderJoint3D/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            return true;
        }
        else if (property == "SliderJoint3D/point_one") {
            path_to_line_point_one = (NodePath)value;
            if (path_to_target != null) {
                line_point_one = GetNodeOrNull<Node2D>(path_to_line_point_one);
            }
            return true;
        }
        else if (property == "SliderJoint3D/point_two") {
            path_to_line_point_two = (NodePath)value;
            if (path_to_target != null) {
                line_point_two = GetNodeOrNull<Node2D>(path_to_line_point_two);
            }
            return true;
        }
        else if (property == "SliderJoint3D/twisted_bone") {
            path_to_twisted_bone = (NodePath)value;
            if (path_to_target != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone);
            }
            return true;
        }
        else if (property == "SliderJoint3D/rotate_to_target") {
            rotate_to_target = (bool)value;
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "SliderJoint3D/additional_rotation") {
            additional_rotation = Mathf.DegToRad((float)value);
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
        if (property == "SliderJoint3D/target") {
            return path_to_target;
        }
        else if (property == "SliderJoint3D/point_one") {
            return path_to_line_point_one;
        }
        else if (property == "SliderJoint3D/point_two") {
            return path_to_line_point_two;
        }
        else if (property == "SliderJoint3D/twisted_bone") {
            return path_to_twisted_bone;
        }
        else if (property == "SliderJoint3D/rotate_to_target") {
            return rotate_to_target;
        }
        else if (property == "SliderJoint3D/additional_rotation") {
            return Mathf.RadToDeg(additional_rotation);
        }

        try {
            return base._Get(property);
        } catch {
            return false;
        }
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = base._GetPropertyList();
        Godot.Collections.Dictionary tmp_dict;

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "SliderJoint3D/target");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "SliderJoint3D/point_one");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "SliderJoint3D/point_two");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "SliderJoint3D/twisted_bone");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "SliderJoint3D/rotate_to_target");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        if (rotate_to_target == true) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "SliderJoint3D/additional_rotation");
            tmp_dict.Add("type", (int)Variant.Type.Float);
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
                GD.PrintErr("Cannot execute 2D SliderJoint3D: No target found!");
            }
        }

        if (line_point_one == null) {
            if (path_to_line_point_one != null) {
                line_point_one = GetNodeOrNull<Node2D>(path_to_line_point_one);
            }
            if (line_point_one == null) {
                GD.PrintErr("Cannot execute 2D SliderJoint3D: No point 1 node found!");
            }
        }

        if (line_point_two == null) {
            if (path_to_line_point_two != null) {
                line_point_two = GetNodeOrNull<Node2D>(path_to_line_point_two);
            }
            if (line_point_two == null) {
                GD.PrintErr("Cannot execute 2D SliderJoint3D: No point 2 node found!");
            }
        }

        if (twisted_bone == null) {
            if (path_to_twisted_bone != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone2D>(path_to_twisted_bone);
            }
            if (twisted_bone == null) {
                GD.PrintErr("Cannot execute 2D SliderJoint3D: No twisted bone 2D node found!");
            }
        }

        _ExecuteSliderJoint();
    }

    private void _ExecuteSliderJoint() {
        twisted_bone.set_executing_ik(true);

        // Get the transforms in global-pose space
        Transform2D bone_pose = Twisted_2DFunctions.world_transform_to_global_pose(twisted_bone.GlobalTransform, twisted_bone.get_skeleton());
        Transform2D point_one_pose = Twisted_2DFunctions.world_transform_to_global_pose(line_point_one.GlobalTransform, twisted_bone.get_skeleton());
        Transform2D point_two_pose = Twisted_2DFunctions.world_transform_to_global_pose(line_point_two.GlobalTransform, twisted_bone.get_skeleton());
        Transform2D target_pose = Twisted_2DFunctions.world_transform_to_global_pose(target_node.GlobalTransform, twisted_bone.get_skeleton());

        // Get the point on a line
        // Credit (https://stackoverflow.com/questions/3120357/get-closest-point-to-a-line)

        Vector2 one_to_target = target_pose.Origin - point_one_pose.Origin;
        Vector2 one_to_two = point_two_pose.Origin - point_one_pose.Origin;
        float point_distance = one_to_target.Dot(one_to_two) / one_to_two.LengthSquared();
        if (point_distance < 0) {
            bone_pose.Origin = point_one_pose.Origin;
        }
        else if (point_distance > 1) {
            bone_pose.Origin = point_two_pose.Origin;
        }
        else {
            bone_pose.Origin = point_one_pose.Origin + (one_to_two * point_distance);
        }

        twisted_bone.GlobalTransform = Twisted_2DFunctions.global_pose_to_world_transform(bone_pose, twisted_bone.get_skeleton());

        if (rotate_to_target == true) {
            twisted_bone.LookAt(target_node.GlobalPosition);
            twisted_bone.Rotate(additional_rotation);
            twisted_bone.Rotate(-twisted_bone.bone_angle);
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

        if (twisted_bone == null || line_point_one == null || line_point_two == null) {
            return;
        }

        DrawLine(line_point_one.GlobalPosition, line_point_two.GlobalPosition, twisted_bone.gizmo_bone_normal_color, 2.0f, true);
    }
}