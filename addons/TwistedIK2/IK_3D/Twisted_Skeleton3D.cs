using Godot;
using System;
using System.Collections.Generic;


/// <summary>
/// The forward direction for the bones in a Skeleton3D node.
/// This is needed because bones in a Skeleton3D node can face any axis, but we want all Twisted_Bone3D nodes to have
/// -Z as the forward direction, as that is the Godot default forward direction.
/// </summary>
public enum TWISTED_BONE_FORWARD_DIRECTION {
    AUTO_CALCULATE,
    X_FORWARD,
    NEGATIVE_X_FORWARD,
    Y_FORWARD,
    NEGATIVE_Y_FORWARD,
    Z_FORWARD,
    NEGATIVE_Z_FORWARD,
}

/// <summary>
/// A helper struct containing additional data for bones in a Skeleton3D node.
/// </summary>
public struct SKELETON3D_BONE_DATA {
    public int bone_index;
    public List<int> children_indexes;
    public int parent_index;
}

/// <summary>
/// The Twisted_Skeleton3D node provides a bunch of helper functionality to Twisted_Bone3D nodes that allow them to perform the work
/// they need to do. Twisted_Bone3D nodes cannot function properly without a Twisted_Skeleton3D currently.
/// </summary>
[Tool]
public partial class Twisted_Skeleton3D : Node3D
{

    /// <summary>
    /// A NodePath to the Skeleton3D node that this Twisted_Skeleton3D uses.
    /// </summary>
    [Export]
    public NodePath _path_to_skeleton;
    /// <summary>
    /// A reference to the Skeleton3D node that this Twisted_Skeleton3D uses.
    /// </summary>
    public Skeleton3D current_skeleton;

    /// <summary>
    /// The forward direction used by the bones in the Skeleton3D node. We need this so we can convert forward directions before applying to the bones.
    /// </summary>
    public TWISTED_BONE_FORWARD_DIRECTION bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.AUTO_CALCULATE;

    private TWISTED_BONE_FORWARD_DIRECTION _internal_bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.AUTO_CALCULATE;


    /// <summary>
    /// Used for making a dictionary tree that represents each of the bones in a Skeleton3D node.
    /// </summary>
    /// <typeparam name="int">The ID of the bone in the Skeleton3D</typeparam>
    /// <typeparam name="SKELETON3D_BONE_DATA"></typeparam>
    /// <returns></returns>
    private Dictionary<int, SKELETON3D_BONE_DATA> skeleton3d_bone_data_dict = new Dictionary<int, SKELETON3D_BONE_DATA>();
    /// <summary>
    /// A list of all the bones that are root bones, meaning bones without parents.
    /// </summary>
    /// <typeparam name="int"></typeparam>
    /// <returns>The ID of the root bone in the Skeleton3D</returns>
    public List<int> skeleton3d_bone_data_root_bones = new List<int>();


    public override void _Ready()
    {
        if (_path_to_skeleton != null) {
            current_skeleton = GetNodeOrNull<Skeleton3D>(_path_to_skeleton);
        }

        if (current_skeleton != null) {
            _internal_bone_forward_direction = bone_forward_direction;
            if (bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.AUTO_CALCULATE) {
                auto_calculate_bone_forward_direction();
            }

            GlobalTransform = current_skeleton.GlobalTransform;
        }

        // So we can keep in sync with the Skeleton3D!
        SetNotifyTransform(true);

        base._Ready();
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "/bone_forward_direction") {
            bone_forward_direction = (TWISTED_BONE_FORWARD_DIRECTION)(int)value;
            _internal_bone_forward_direction = bone_forward_direction;
            if (bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.AUTO_CALCULATE) {
                auto_calculate_bone_forward_direction();
            }
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
        if (property == "/bone_forward_direction") {
            return (int)bone_forward_direction;
        }

        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        Godot.Collections.Dictionary tmp_dict;

        // ===========================================
        // ===== AUTOMATION
        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "/bone_forward_direction");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.Enum);
        tmp_dict.Add("hint_string", "Auto_Calculate, X_Forward, Negative_X_Forward, Y_Forward, Negative_Y_Forward, Z_Forward, Negative_Z_Forward");
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        return list;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (current_skeleton != null) {
            GlobalTransform = current_skeleton.GlobalTransform;
        }
    }

    /// <summary>
    /// Converts a global Transform3D to a global pose Transform3D that is usable in a Skeleton3D node. This is because a global
    /// bone pose is different than a global Transform3D.
    /// </summary>
    /// <param name="world_transform">The global Transform3D you want to convert</param>
    /// <returns>The Transform3D converted to a global pose relative to the Skeleton3D node</returns>
    public Transform3D world_transform_to_global_pose(Transform3D world_transform) {
        return GlobalTransform.AffineInverse() * world_transform;
    }
    /// <summary>
    /// Converts a global pose Transform3D to a global Transform3D. This allows you to convert Transforms from the <c>get_global_pose</c>
    /// function in a Skeleton3D and use it on a Node3D-based node.
    /// </summary>
    /// <param name="global_pose"></param>
    /// <returns></returns>
    public Transform3D global_pose_to_world_transform(Transform3D global_pose) {
        return GlobalTransform * global_pose;
    }


    /// <summary>
    /// Calculates the bone forward direction of the bones in the Skeleton3D this Twisted_Skeleton3D is linked to.
    /// </summary>
    public void auto_calculate_bone_forward_direction() {
        if (current_skeleton == null) {
            current_skeleton = GetNodeOrNull<Skeleton3D>(_path_to_skeleton);
        }

        if (current_skeleton != null) {
            Vector3 accum_bone_direction = Vector3.Zero;
            int accum_bone_count = current_skeleton.GetBoneCount();
            for (int i = 0; i < accum_bone_count; i++) {
                accum_bone_direction += current_skeleton.GetBoneRest(i).Origin;
            }
            accum_bone_direction = (accum_bone_direction / accum_bone_count).Normalized();
            Vector3 accum_bone_abs = accum_bone_direction.Abs();

            if (accum_bone_abs.X > accum_bone_abs.Y && accum_bone_abs.X > accum_bone_abs.Z) {
                if (accum_bone_direction.X > 0) {
                    _internal_bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.X_FORWARD;
                } else {
                    _internal_bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_X_FORWARD;
                }
            }
            else if (accum_bone_abs.Y > accum_bone_abs.X && accum_bone_abs.Y > accum_bone_abs.Z) {
                if (accum_bone_direction.Y > 0) {
                    _internal_bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.Y_FORWARD;
                } else {
                    _internal_bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Y_FORWARD;
                }
            }
            else if (accum_bone_abs.Z > accum_bone_abs.X && accum_bone_abs.Z > accum_bone_abs.Y) {
                if (accum_bone_abs.Z > 0) {
                    _internal_bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.Z_FORWARD;
                } else {
                    _internal_bone_forward_direction = TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Z_FORWARD;
                }
            }
            else {
                GD.PrintErr("Twisted_Skeleton3D: Cannot auto calculate bone forward direction!");
            }
        }
    }

    /// <summary>
    /// Converts a Basis from the -Z forward direction to the forward direction used by the bones in the Skeleton3D
    /// </summary>
    /// <param name="basis">The -Z forward direction Basis</param>
    /// <returns>A Basis rotated so the forward direction is correct for the bones in the Skeleton3D</returns>
    public Basis negative_z_forward_to_bone_forward(Basis basis) {
        Basis return_basis = basis;

        if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.X_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 1, 0), (Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_X_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 1, 0), -(Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.Y_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(1, 0, 0), -(Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Y_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 1, 0), (Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.Z_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 0, 1), Mathf.Pi);
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Z_FORWARD) {
            // Nothing to do here!
        }

        return return_basis;
    }

    /// <summary>
    /// Converts a bone-forward facing Basis and rotates it so the forward direction is on the -Z axis.
    /// This allows the Basis to be used with things like the <c>look_at</c> function. Once finished, you can call the
    /// <c>negative_z_forward_to_bone_forward</c> function before applying to the Bone so the bone faces in the proper direction.
    /// </summary>
    /// <param name="basis"></param>
    /// <returns></returns>
    public Basis bone_forward_to_negative_z_forward(Basis basis) {
        Basis return_basis = basis;

        if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.X_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 1, 0), -(Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_X_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 1, 0), (Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.Y_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(1, 0, 0), (Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Y_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 1, 0), -(Mathf.Pi * 0.5f));
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.Z_FORWARD) {
            return_basis = rotate_basis_local(return_basis, new Vector3(0, 0, 1), -Mathf.Pi);
        } else if (_internal_bone_forward_direction == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Z_FORWARD) {
            // Nothing to do here!
        }

        return return_basis;
    }

    /// <summary>
    /// A port of the C++ function with the same name. Rotates a Basis locally around the given axis by the given angle amount
    /// </summary>
    /// <param name="basis"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Basis rotate_basis_local(Basis basis, Vector3 axis, float angle) {
        return basis * new Basis(axis, angle);
    }

    /// <summary>
    /// Updates the dictionary of Skeleton3D bone data. Should be called once before using the dictionary of bone data.
    /// </summary>
    public void update_skeleton3d_bone_data() {
        if (current_skeleton == null) {
            GD.PrintErr("Cannot make skeleton3D bone data: No skeleton assigned!");
            return;
        }

        skeleton3d_bone_data_dict.Clear();
        skeleton3d_bone_data_root_bones.Clear();

        // Populate the dictionary (first pass)
        for (int i = 0; i < current_skeleton.GetBoneCount(); i++) {
            SKELETON3D_BONE_DATA bone_data = new SKELETON3D_BONE_DATA();
            bone_data.bone_index = i;
            bone_data.children_indexes = new List<int>();
            bone_data.parent_index = current_skeleton.GetBoneParent(i);

            // Handle root bones (add them to the array)
            if (bone_data.parent_index == -1) {
                skeleton3d_bone_data_root_bones.Add(i);
            }

            skeleton3d_bone_data_dict.Add(i, bone_data);
        }

        // Add the children-bone connections
        for (int i = 0; i < current_skeleton.GetBoneCount(); i++) {
            int bone_parent = current_skeleton.GetBoneParent(i);
            if (bone_parent != -1) {
                SKELETON3D_BONE_DATA parent_data = (SKELETON3D_BONE_DATA)skeleton3d_bone_data_dict[bone_parent];
                parent_data.children_indexes.Add(i);
                skeleton3d_bone_data_dict[bone_parent] = parent_data;
            }
        }
        // Done!
    }

    public Dictionary<int, SKELETON3D_BONE_DATA> get_skeleton3d_bone_data() {
        if (skeleton3d_bone_data_dict.Count <= 0) {
            update_skeleton3d_bone_data();
        }
        return skeleton3d_bone_data_dict;
    }
    public List<int> get_skeleton3d_root_bones() {
        if (skeleton3d_bone_data_dict.Count <= 0) {
            update_skeleton3d_bone_data();
        }
        return skeleton3d_bone_data_root_bones;
    }

    public TWISTED_BONE_FORWARD_DIRECTION get_bone_forward_direction() {
        return _internal_bone_forward_direction;
    }

}