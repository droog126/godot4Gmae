using Godot;
using System;

/// <summary>
/// A modifier that use the Coordinate Cyclic Descent (CCD) Inverse Kinematics (IK) algorithm (CCDIK)
/// to rotate a series of Twisted_Bone3D nodes to reach a target.
/// </summary>
[Tool]
public partial class Twisted_ModifierCCDIK3D : Twisted_Modifier3D
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
    /// A NodePath to the Node3D-based node that is used as the tip position
    /// </summary>
    public NodePath path_to_tip = null;
    /// <summary>
    /// A reference to the Node3D-based node that is used as the tip position.
    /// This node should be a child of the last Twisted_Bone3D node in the CCDIK chain for best results.
    /// </summary>
    public Node3D tip_node = null;

    /// <summary>
    /// If <c>true</c>, the CCDIK algorithm will run more often per solve to get a higher quality result. Costs a bit of performance over stock CCDIK.
    /// (This feature is based on a CCDIK presentation by Unity3D)
    /// </summary>
    public bool high_quality_solve = false;

    /// <summary>
    /// The Struct used to hold all of the data for each joint in the CCDIK joint chain.
    /// </summary>
    public struct CCDIK_JOINT {
        public NodePath path_to_twisted_bone;
        public Twisted_Bone3D twisted_bone;
        
        /// <summary>
        /// The axis of rotation.c 0 = X-axis, 1 = Y-axis, and 2 = Z-axis
        /// </summary>
        public int axis;

        /// <summary>
        /// If <c>true</c>, then angle constraints will be applied to this bone. Constraints are in <c>radians</c>.
        /// </summary>
        public bool use_constraints;
        public float constraint_angle_min;
        public float constraint_angle_max;
        /// <summary>
        /// If <c>true</c>, the join will be constrained to the negative space defined by the minimum and maximum angle constraints.
        /// </summary>
        public bool constraint_angle_invert;

        public CCDIK_JOINT(NodePath path) {
            this.path_to_twisted_bone = path;
            this.twisted_bone = null;
            this.axis = 0;
            this.use_constraints = false;
            this.constraint_angle_min = 0;
            this.constraint_angle_max = Mathf.Pi * 2.0f;
            this.constraint_angle_invert = false;
        }
    }
    /// <summary>
    /// All of the joints in the CCDIK chain.
    /// </summary>
    public CCDIK_JOINT[] ccdik_joints = new CCDIK_JOINT[0];

    private int joint_count = 0;

    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node3D>(path_to_target);
        }
        if (path_to_tip != null) {
            tip_node = GetNodeOrNull<Node3D>(path_to_tip);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "CCDIK/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node3D>(path_to_target);
            }
            return true;
        }
        else if (property == "CCDIK/tip") {
            path_to_tip = (NodePath)value;
            if (path_to_tip != null) {
                tip_node = GetNodeOrNull<Node3D>(path_to_tip);
            }
            return true;
        }
        else if (property == "CCDIK/high_quality_solve") {
            high_quality_solve = (bool)value;
            return true;
        }
        else if (property == "CCDIK/joint_count") {
            joint_count = (int)value;

            CCDIK_JOINT[] new_array = new CCDIK_JOINT[joint_count];
            for (int i = 0; i < joint_count; i++) {
                if (i < ccdik_joints.Length) {
                    new_array[i] = ccdik_joints[i];
                } else {
                    new_array[i] = new CCDIK_JOINT(null);
                }
            }
            ccdik_joints = new_array;

            NotifyPropertyListChanged();
            return true;
        }
        else if (property.ToString().StartsWith("CCDIK/joint/")) {
            String[] ccdik_data = property.ToString().Split("/");
            int joint_index = ccdik_data[2].ToInt();
            
            if (joint_index < 0 || joint_index > ccdik_joints.Length-1) {
                GD.PrintErr("ERROR - Cannot get CCDIK joint at index " + joint_index.ToString());
                return false;
            }
            CCDIK_JOINT current_joint = ccdik_joints[joint_index];

            if (ccdik_data[3] == "twisted_bone") {
                current_joint.path_to_twisted_bone = (NodePath)value;
                if (current_joint.path_to_twisted_bone != null) {
                    current_joint.twisted_bone = GetNodeOrNull<Twisted_Bone3D>(current_joint.path_to_twisted_bone);
                }
            }
            else if (ccdik_data[3] == "axis") {
                current_joint.axis = (int)value;
            }
            else if (ccdik_data[3] == "use_constraints") {
                current_joint.use_constraints = (bool)value;
                NotifyPropertyListChanged();
            }
            else if (ccdik_data[3] == "constraint_angle_min") {
                current_joint.constraint_angle_min = Mathf.DegToRad((float)value);
            }
            else if (ccdik_data[3] == "constraint_angle_max") {
                current_joint.constraint_angle_max = Mathf.DegToRad((float)value);
            }
            else if (ccdik_data[3] == "constraint_angle_invert") {
                current_joint.constraint_angle_invert = (bool)value;
            }
            ccdik_joints[joint_index] = current_joint;
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
        if (property == "CCDIK/target") {
            return path_to_target;
        }
        else if (property == "CCDIK/tip") {
            return path_to_tip;
        }
        else if (property == "CCDIK/high_quality_solve") {
            return high_quality_solve;
        }
        else if (property == "CCDIK/joint_count") {
            return joint_count;
        }

        else if (property.ToString().StartsWith("CCDIK/joint/")) {
            String[] ccdik_data = property.ToString().Split("/");
            int joint_index = ccdik_data[2].ToInt();

            if (ccdik_data[3] == "twisted_bone") {
                return ccdik_joints[joint_index].path_to_twisted_bone;
            }
            else if (ccdik_data[3] == "axis") {
                return ccdik_joints[joint_index].axis;
            }
            else if (ccdik_data[3] == "use_constraints") {
                return ccdik_joints[joint_index].use_constraints;
            }
            else if (ccdik_data[3] == "constraint_angle_min") {
                return Mathf.RadToDeg(ccdik_joints[joint_index].constraint_angle_min);
            }
            else if (ccdik_data[3] == "constraint_angle_max") {
                return Mathf.RadToDeg(ccdik_joints[joint_index].constraint_angle_max);
            }
            else if (ccdik_data[3] == "constraint_angle_invert") {
                return ccdik_joints[joint_index].constraint_angle_invert;
            }
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
        tmp_dict.Add("name", "CCDIK/target");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "CCDIK/tip");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "CCDIK/high_quality_solve");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "CCDIK/joint_count");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        // The CCDIK Joints
        // ===================
        String ccdik_string = "CCDIK/joint/";
        for (int i = 0; i < joint_count; i++) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", ccdik_string + i.ToString() + "/twisted_bone");
            tmp_dict.Add("type", (int)Variant.Type.NodePath);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", ccdik_string + i.ToString() + "/axis");
            tmp_dict.Add("type", (int)Variant.Type.Int);
            tmp_dict.Add("hint", (int)PropertyHint.Enum);
            tmp_dict.Add("hint_string", "LOCAL_X_AXIS, LOCAL_Y_AXIS, LOCAL_Z_AXIS");
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", ccdik_string + i.ToString() + "/use_constraints");
            tmp_dict.Add("type", (int)Variant.Type.Bool);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            if (ccdik_joints[i].use_constraints == true) {
                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", ccdik_string + i.ToString() + "/constraint_angle_min");
                tmp_dict.Add("type", (int)Variant.Type.Float);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);

                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", ccdik_string + i.ToString() + "/constraint_angle_max");
                tmp_dict.Add("type", (int)Variant.Type.Float);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);

                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", ccdik_string + i.ToString() + "/constraint_angle_invert");
                tmp_dict.Add("type", (int)Variant.Type.Bool);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);
            }
        }

        return list;
    }

    public override void _ExecuteModification(Twisted_ModifierStack3D modifier_stack, double delta)
    {
        base._ExecuteModification(modifier_stack, delta);

        if (target_node == null || tip_node == null) {
            GD.PrintErr("Cannot execute CCDIK: No target or tip found!");
            return;
        }

        if (high_quality_solve == false) {
            for (int i = 0; i < ccdik_joints.Length; i++) {
                _ExecuteCCDIKJoint(i);
            }
        }
        else {
            for (int i = 0; i < ccdik_joints.Length; i++) {
                for (int j = i; j < ccdik_joints.Length; j++) {
                    _ExecuteCCDIKJoint(j);
                }
            }
        }
    }

    private void _ExecuteCCDIKJoint(int joint_index) {
        Twisted_Bone3D bone = ccdik_joints[joint_index].twisted_bone;
        if (bone == null) {
            if (ccdik_joints[joint_index].path_to_twisted_bone != null) {
                bone = GetNodeOrNull<Twisted_Bone3D>(ccdik_joints[joint_index].path_to_twisted_bone);
                if (bone == null) {
                    GD.PrintErr("Cannot execute CCDIK joint " + joint_index.ToString() + ": No bone found!");
                    return;
                }
            }
        }
        Quaternion end_rotation = bone.Transform.Basis.GetRotationQuaternion();

        end_rotation = Twisted_3DFunctions.quat_from_two_vectors(
            bone.ToLocal(tip_node.GlobalTransform.Origin),
            bone.ToLocal(target_node.GlobalTransform.Origin)
        );
        end_rotation = end_rotation * bone.Transform.Basis.GetRotationQuaternion();
        end_rotation = end_rotation.Normalized();

        // Enforce rotation only on the selected joint axis
        // -------------
        Vector3 ccdik_euler_angles = end_rotation.GetEuler();
        Vector3 rotation_to_apply = bone.Rotation;
        if (ccdik_joints[joint_index].axis == 0) { // Local X axis
            rotation_to_apply.X = ccdik_euler_angles.X;
        }
        else if (ccdik_joints[joint_index].axis == 1) { // Local Y axis
            rotation_to_apply.Y = ccdik_euler_angles.Y;
        }
        else if (ccdik_joints[joint_index].axis == 2) { // Local Z axis
            rotation_to_apply.Z = ccdik_euler_angles.Z;
        }

        // Apply constraints
        // ---------------
        if (ccdik_joints[joint_index].use_constraints == true) {
            // Get the current angle
            float rotation_angle;
            if (ccdik_joints[joint_index].axis == 0) { // Local X axis
                rotation_angle = rotation_to_apply.X;
            }
            else if (ccdik_joints[joint_index].axis == 1) { // Local Y axis
                rotation_angle = rotation_to_apply.Y;
            }
            else if (ccdik_joints[joint_index].axis == 2) { // Local Z axis
                rotation_angle = rotation_to_apply.Z;
            } else {
                rotation_angle = rotation_to_apply.Z;
            }

            // Convert to the 0 to 360 range (instead of -180 to 180 range)
            if (rotation_angle < 0) {
                rotation_angle = (Mathf.Pi * 2.0f) + rotation_angle;
            }

            // Clamp the angle
            float clamped_rotation_angle = clamp_angle(rotation_angle, ccdik_joints[joint_index].constraint_angle_min,
                ccdik_joints[joint_index].constraint_angle_max, ccdik_joints[joint_index].constraint_angle_invert);
            
            // Set the clamped angle
            if (ccdik_joints[joint_index].axis == 0) { // Local X axis
                rotation_to_apply.X = clamped_rotation_angle;
            }
            else if (ccdik_joints[joint_index].axis == 1) { // Local Y axis
                rotation_to_apply.Y = clamped_rotation_angle;
            }
            else if (ccdik_joints[joint_index].axis == 2) { // Local Z axis
                rotation_to_apply.Z = clamped_rotation_angle;
            }
        }

        // Apply the rotation to the bone
        // -------------
        bone.Rotation = rotation_to_apply;

        if (force_bone_application) {
            bone.force_apply_transform();
        }
        // -------------
    }

    private float clamp_angle(float angle, float min_bound, float max_bound, bool invert) {
        // Map to the 0 to 360 range (in radians though) instead of the -180 to 180 range.
        if (angle < 0) {
            angle = (Mathf.Pi * 2) + angle;
        }

        // Make min and max in the range of 0 to 360 (in radians), and make sure they are in the right order
        if (min_bound < 0) {
            min_bound = (Mathf.Pi * 2) + min_bound;
        }
        if (max_bound < 0) {
            max_bound = (Mathf.Pi * 2) + max_bound;
        }
        if (min_bound > max_bound) {
            float tmp = min_bound;
            min_bound = max_bound;
            max_bound = tmp;
        }

        // Note: May not be the most optimal way to clamp, but it always constraints to the nearest angle.
        if (invert == false) {
            if (angle < min_bound || angle > max_bound) {
                Vector2 min_bound_vec = new Vector2(Mathf.Cos(min_bound), Mathf.Sin(min_bound));
                Vector2 max_bound_vec = new Vector2(Mathf.Cos(max_bound), Mathf.Sin(max_bound));
                Vector2 angle_vec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                if (angle_vec.DistanceSquaredTo(min_bound_vec) <= angle_vec.DistanceSquaredTo(max_bound_vec)) {
                    angle = min_bound;
                } else {
                    angle = max_bound;
                }
            }
        } else {
            if (angle > min_bound && angle < max_bound) {
                Vector2 min_bound_vec = new Vector2(Mathf.Cos(min_bound), Mathf.Sin(min_bound));
                Vector2 max_bound_vec = new Vector2(Mathf.Cos(max_bound), Mathf.Sin(max_bound));
                Vector2 angle_vec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                if (angle_vec.DistanceSquaredTo(min_bound_vec) <= angle_vec.DistanceSquaredTo(max_bound_vec)) {
                    angle = min_bound;
                } else {
                    angle = max_bound;
                }
            }
        }
        return angle;
    }
}