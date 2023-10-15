using Godot;
using System;

/// <summary>
/// A modifier that rotates a single Twisted_Bone3D node to look at a target node using the <c>look_at</c> function.
/// The simplest of all of the Twisted_Modifier3D classes and a good reference to use when making your own.
/// </summary>
[Tool]
public partial class Twisted_ModifierLookAt3D : Twisted_Modifier3D
{
    /// <summary>
    /// A NodePath to the Node3D-based node that is used as the target position
    /// </summary>
    public NodePath path_to_target;
    /// <summary>
    /// A reference to the Node3D-based node that is used as the target position
    /// </summary>
    public Node3D target_node;

    /// <summary>
    /// A NodePath to the Twisted_Bone3D node that this modifier operates on
    /// </summary>
    public NodePath path_to_twisted_bone;
    /// <summary>
    /// A reference to the Twisted_Bone3D node that this modifier operates on
    /// </summary>
    public Twisted_Bone3D twisted_bone;

    /// <summary>
    /// A Vector3 that stores a rotation offset (in radiens) that is applied after making the bone look at the target
    /// </summary>
    public Vector3 tweak_additional_rotation = Vector3.Zero;

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
        if (path_to_twisted_bone != null) {
            twisted_bone = GetNodeOrNull<Twisted_Bone3D>(path_to_twisted_bone);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "look_at/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node3D>(path_to_target);
            }
            return true;
        }
        else if (property == "look_at/twisted_bone") {
            path_to_twisted_bone = (NodePath)value;
            if (path_to_twisted_bone != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone3D>(path_to_twisted_bone);
            }
            return true;
        }
        else if (property == "look_at/additional_rotation") {
            Vector3 degree_rot = (Vector3)value;
            tweak_additional_rotation.X = Mathf.DegToRad(degree_rot.X);
            tweak_additional_rotation.Y = Mathf.DegToRad(degree_rot.Y);
            tweak_additional_rotation.Z = Mathf.DegToRad(degree_rot.Z);
            return true;
        }
        else if (property == "look_at/basis_direction") {
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
        if (property == "look_at/target") {
            return path_to_target;
        }
        else if (property == "look_at/twisted_bone") {
            return path_to_twisted_bone;
        }
        else if (property == "look_at/additional_rotation") {
            Vector3 degree_rot = Vector3.One;
            degree_rot.X = Mathf.RadToDeg(tweak_additional_rotation.X);
            degree_rot.Y = Mathf.RadToDeg(tweak_additional_rotation.Y);
            degree_rot.Z = Mathf.RadToDeg(tweak_additional_rotation.Z);
            return degree_rot;
        }
        else if (property == "look_at/basis_direction") {
            return (int)lookat_basis_direction;
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
        tmp_dict.Add("type", (int)Variant.Type.Vector3);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "look_at/basis_direction");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.Enum);
        tmp_dict.Add("hint_string", "X_Basis, Y_Basis, Z_Basis");
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

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
                GD.PrintErr("Cannot execute LookAt: No target found!");
                return;
            }
        }
        if (twisted_bone == null) {
            if (path_to_twisted_bone != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone3D>(path_to_twisted_bone);
            }
            if (twisted_bone == null) {
                GD.PrintErr("Cannot execute LookAt: No Twisted_Bone2D found!");
                return;
            }
        }

        // Set the look-at basis (Default to Y basis)
        // This is useful for some cases where the bone direction isn't Y-axis-aligned.
        Vector3 bone_up_dir = twisted_bone.get_reset_bone_global_pose().Basis.Y;
        if (lookat_basis_direction == LOOKAT_BASIS_DIRECTION.X_BASIS) {
            bone_up_dir = twisted_bone.get_reset_bone_global_pose().Basis.X;
        } else if (lookat_basis_direction == LOOKAT_BASIS_DIRECTION.Z_BASIS) {
            bone_up_dir = twisted_bone.get_reset_bone_global_pose().Basis.Z;
        }
        
        bone_up_dir = bone_up_dir.Normalized(); 
        twisted_bone.LookAt(target_node.GlobalTransform.Origin, bone_up_dir);

        twisted_bone.Rotate(twisted_bone.Transform.Basis.X.Normalized(), tweak_additional_rotation.X);
        twisted_bone.Rotate(twisted_bone.Transform.Basis.Y.Normalized(), tweak_additional_rotation.Y);
        twisted_bone.Rotate(twisted_bone.Transform.Basis.Z.Normalized(), tweak_additional_rotation.Z);

        // Keep the scale consistent with the global pose
        twisted_bone.Scale = twisted_bone.get_reset_bone_global_pose(false).Basis.Scale;

        if (force_bone_application) {
            twisted_bone.force_apply_transform();
        }
    }
}
