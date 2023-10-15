using Godot;
using System;


[Tool]
public partial class Twisted_ModifierSliderJoint3D : Twisted_Modifier3D
{
    /// <summary>
    /// A NodePath to the Node3D-based node that is used as the target position
    /// </summary>
    public NodePath path_to_target = null;
    /// <summary>
    /// A reference to the Node3D-based node that is used as the target position
    /// </summary>
    public Node3D target_node = null;

    /// <summary>
    /// A NodePath to the first point that makes the 3D line
    /// </summary>
    public NodePath path_to_line_point_one = null;
    /// <summary>
    /// The Node3D node that makes the first point on the 3D line
    /// </summary>
    public Node3D line_point_one = null;

    /// <summary>
    /// A NodePath to the second point that makes the 3D line
    /// </summary>
    public NodePath path_to_line_point_two = null;
    /// <summary>
    /// The Node3D node that makes the second point on the 3D line
    /// </summary>
    public Node3D line_point_two = null;

    /// <summary>
    /// A NodePath to the Twisted_Bone3D that will be constrained to the line
    /// </summary>
    public NodePath path_to_twisted_bone = null;
    /// <summary>
    /// The Twisted_Bone3D that will be constrained to the line
    /// </summary>
    public Twisted_Bone3D twisted_bone = null;

    /// <summary>
    /// If true, the bone will rotate to look at the target
    /// </summary>
    public bool rotate_to_target = false;
    /// <summary>
    /// Additional rotation to be applied
    /// </summary>
    public Vector3 additional_rotation = Vector3.Zero;

    /// <summary>
    /// Enums to show which basis-axis will be used as the base reference when resetting the bone prior to
    /// performing the look-at.
    /// </summary>    
    public enum LOOKAT_BASIS_DIRECTION {
        X_BASIS,
        Y_BASIS,
        Z_BASIS
    }

    /// <summary>
    /// The basis-axis to use as the base reference when resetting the bone prior to performing the look-at.
    /// This should be left at Y_Basis for the majority of uses, but there are cases where you may want to
    /// start at another basis to get correct rotation results. If you are getting the incorrect rotation
    /// compared to what you expect, try changing the basis direction and see if it helps.
    /// </summary>
    public LOOKAT_BASIS_DIRECTION lookat_basis_direction = LOOKAT_BASIS_DIRECTION.Y_BASIS;


    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node3D>(path_to_target);
        }
        if (path_to_line_point_one != null) {
            line_point_one = GetNodeOrNull<Node3D>(path_to_line_point_one);
        }
        if (path_to_line_point_two != null) {
            line_point_two = GetNodeOrNull<Node3D>(path_to_line_point_two);
        }
        if (path_to_twisted_bone != null) {
            twisted_bone = GetNodeOrNull<Twisted_Bone3D>(path_to_twisted_bone);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "SliderJoint3D/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node3D>(path_to_target);
            }
            return true;
        }
        else if (property == "SliderJoint3D/point_one") {
            path_to_line_point_one = (NodePath)value;
            if (path_to_target != null) {
                line_point_one = GetNodeOrNull<Node3D>(path_to_line_point_one);
            }
            return true;
        }
        else if (property == "SliderJoint3D/point_two") {
            path_to_line_point_two = (NodePath)value;
            if (path_to_target != null) {
                line_point_two = GetNodeOrNull<Node3D>(path_to_line_point_two);
            }
            return true;
        }
        else if (property == "SliderJoint3D/twisted_bone") {
            path_to_twisted_bone = (NodePath)value;
            if (path_to_target != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone3D>(path_to_twisted_bone);
            }
            return true;
        }
        else if (property == "SliderJoint3D/rotate_to_target") {
            rotate_to_target = (bool)value;
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "SliderJoint3D/additional_rotation") {
            Vector3 value_convert = (Vector3)value;
            value_convert.X = Mathf.DegToRad(value_convert.X);
            value_convert.Y = Mathf.DegToRad(value_convert.Y);
            value_convert.Z = Mathf.DegToRad(value_convert.Z);
            additional_rotation = value_convert;
            return true;
        }
        else if (property == "SliderJoint3D/basis_direction") {
            lookat_basis_direction = (LOOKAT_BASIS_DIRECTION)(int)value;
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
            Vector3 value_convert = additional_rotation;
            value_convert.X = Mathf.RadToDeg(value_convert.X);
            value_convert.Y = Mathf.RadToDeg(value_convert.Y);
            value_convert.Z = Mathf.RadToDeg(value_convert.Z);
            return value_convert;
        }
        else if (property == "SliderJoint3D/basis_direction") {
            return (int)lookat_basis_direction;
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
            tmp_dict.Add("type", (int)Variant.Type.Vector3);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "SliderJoint3D/basis_direction");
            tmp_dict.Add("type", (int)Variant.Type.Int);
            tmp_dict.Add("hint", (int)PropertyHint.Enum);
            tmp_dict.Add("hint_string", "X_Basis, Y_Basis, Z_Basis");
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);
        }

        return list;
    }

    public override void _ExecuteModification(Twisted_ModifierStack3D modifier_stack, double delta)
    {
        base._ExecuteModification(modifier_stack, delta);

        if (target_node == null) {
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node3D>(path_to_target);
            }
            if (target_node == null) {
                GD.PrintErr("Cannot execute 3D SliderJoint3D: No target found!");
            }
        }

        if (line_point_one == null) {
            if (path_to_line_point_one != null) {
                line_point_one = GetNodeOrNull<Node3D>(path_to_line_point_one);
            }
            if (line_point_one == null) {
                GD.PrintErr("Cannot execute 3D SliderJoint3D: No point 1 node found!");
            }
        }

        if (line_point_two == null) {
            if (path_to_line_point_two != null) {
                line_point_two = GetNodeOrNull<Node3D>(path_to_line_point_two);
            }
            if (line_point_two == null) {
                GD.PrintErr("Cannot execute 3D SliderJoint3D: No point 2 node found!");
            }
        }

        if (twisted_bone == null) {
            if (path_to_twisted_bone != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone3D>(path_to_twisted_bone);
            }
            if (twisted_bone == null) {
                GD.PrintErr("Cannot execute 3D SliderJoint3D: No twisted bone 3D node found!");
            }
        }

        _ExecuteSliderJoint();
    }

    private void _ExecuteSliderJoint() {
        Transform3D transform_to_apply = twisted_bone.GlobalTransform;

        // Get the point on a line
        // Credit (https://stackoverflow.com/questions/3120357/get-closest-point-to-a-line)
        Vector3 one_to_target = target_node.GlobalTransform.Origin - line_point_one.GlobalTransform.Origin;
        Vector3 one_to_two = line_point_two.GlobalTransform.Origin - line_point_one.GlobalTransform.Origin;
        float point_distance = one_to_target.Dot(one_to_two) / one_to_two.LengthSquared();
        if (point_distance < 0) {
            transform_to_apply.Origin = line_point_one.GlobalTransform.Origin;
        }
        else if (point_distance > 1) {
            transform_to_apply.Origin = line_point_two.GlobalTransform.Origin;
        }
        else {
            transform_to_apply.Origin = line_point_one.GlobalTransform.Origin + (one_to_two * point_distance);
        }

        twisted_bone.GlobalTransform = transform_to_apply;

        if (rotate_to_target == true) {
            Vector3 bone_up_dir = twisted_bone.get_reset_bone_global_pose().Basis.Y;
            if (lookat_basis_direction == LOOKAT_BASIS_DIRECTION.X_BASIS) {
                bone_up_dir = twisted_bone.get_reset_bone_global_pose().Basis.X;
            } else if (lookat_basis_direction == LOOKAT_BASIS_DIRECTION.Z_BASIS) {
                bone_up_dir = twisted_bone.get_reset_bone_global_pose().Basis.Z;
            }
            bone_up_dir = bone_up_dir.Normalized(); 

            twisted_bone.LookAt(target_node.GlobalTransform.Origin, bone_up_dir);

            twisted_bone.Rotate(twisted_bone.Transform.Basis.X.Normalized(), additional_rotation.X);
            twisted_bone.Rotate(twisted_bone.Transform.Basis.Y.Normalized(), additional_rotation.Y);
            twisted_bone.Rotate(twisted_bone.Transform.Basis.Z.Normalized(), additional_rotation.Z);

             // Keep the scale consistent with the global pose
            twisted_bone.Scale = twisted_bone.get_reset_bone_global_pose(false).Basis.Scale;
        }

        if (force_bone_application) {
            twisted_bone.force_apply_transform();
        }
    }
}