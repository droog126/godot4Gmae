using Godot;
using System;

/// <summary>
/// A modifier that use the Coordinate Cyclic Descent (CCD) Inverse Kinematics (IK) algorithm (CCDIK)
/// to rotate a series of Twisted_Bone2D nodes to reach a target.
/// </summary>
[Tool]
public partial class Twisted_ModifierCCDIK2D : Twisted_Modifier2D
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
    /// A NodePath to the Node2D-based node that is used as the tip position
    /// </summary>
    public NodePath path_to_tip = null;
    /// <summary>
    /// A reference to the Node2D-based node that is used as the tip position.
    /// This node should be a child of the last Twisted_Bone2D node in the CCDIK chain for best results.
    /// </summary>
    public Node2D tip_node = null;

    /// <summary>
    /// The mode this modifier will use to solve CCDIK. There are four modes currently:
    /// 0 = Normal CCDIK
    /// 1 = High Quality CCDIK
    /// 2 = Backwards CCDIK
    /// 3 = Forward And Backwards CCDIK
    /// </summary>
    public int solve_mode = 0;

    /// <summary>
    /// The Struct used to hold all of the data for each joint in the CCDIK joint chain.
    /// </summary>
    public struct CCDIK_JOINT {
        public NodePath path_to_twisted_bone;
        public Twisted_Bone2D twisted_bone;

        /// <summary>
        /// If <c>true</c>, then angle constraints will be applied to this bone. Constraints are in <c>radians</c>.
        /// </summary>
        public bool constraint_angle_enabled;
        public float constraint_angle_min;
        public float constraint_angle_max;
        /// <summary>
        /// If <c>true</c>, the join will be constrained to the negative space defined by the minimum and maximum angle constraints.
        /// </summary>
        public bool constraint_angle_invert;
        public bool constraint_angle_in_localspace;

        public CCDIK_JOINT(NodePath path) {
            this.path_to_twisted_bone = path;
            this.twisted_bone = null;
            this.constraint_angle_enabled = false;
            this.constraint_angle_min = 0;
            this.constraint_angle_max = Mathf.Pi * 2.0f;
            this.constraint_angle_invert = false;
            this.constraint_angle_in_localspace = true;
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
            target_node = GetNodeOrNull<Node2D>(path_to_target);
        }
        if (path_to_tip != null) {
            tip_node = GetNodeOrNull<Node2D>(path_to_tip);
        }

        for (int i = 0; i < ccdik_joints.Length; i++) {
            if (ccdik_joints[i].twisted_bone == null) {
                ccdik_joints[i].twisted_bone = GetNodeOrNull<Twisted_Bone2D>(ccdik_joints[i].path_to_twisted_bone);
            }
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "CCDIK/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            return true;
        }
        else if (property == "CCDIK/tip") {
            path_to_tip = (NodePath)value;
            if (path_to_tip != null) {
                tip_node = GetNodeOrNull<Node2D>(path_to_tip);
            }
            return true;
        }
        else if (property == "CCDIK/solve_mode") {
            solve_mode = (int)value;
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
                GD.PrintErr("ERROR - Cannot get 2D CCDIK joint at index " + joint_index.ToString());
                return false;
            }
            CCDIK_JOINT current_joint = ccdik_joints[joint_index];

            if (ccdik_data[3] == "twisted_bone") {
                current_joint.path_to_twisted_bone = (NodePath)value;
                if (current_joint.path_to_twisted_bone != null) {
                    current_joint.twisted_bone = GetNodeOrNull<Twisted_Bone2D>(current_joint.path_to_twisted_bone);
                }
            }
            else if (ccdik_data[3] == "constraint_angle_enabled") {
                current_joint.constraint_angle_enabled = (bool)value;
                NotifyPropertyListChanged();
                update_gizmo();
            }
            else if (ccdik_data[3] == "constraint_angle_min") {
                current_joint.constraint_angle_min = Mathf.DegToRad((float)value);
                update_gizmo();
            }
            else if (ccdik_data[3] == "constraint_angle_max") {
                current_joint.constraint_angle_max = Mathf.DegToRad((float)value);
                update_gizmo();
            }
            else if (ccdik_data[3] == "constraint_angle_invert") {
                current_joint.constraint_angle_invert = (bool)value;
                update_gizmo();
            }
            else if (ccdik_data[3] == "constraint_angle_in_localspace") {
                current_joint.constraint_angle_in_localspace = (bool)value;
                update_gizmo();
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
        else if (property == "CCDIK/solve_mode") {
            return solve_mode;
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
            else if (ccdik_data[3] == "constraint_angle_enabled") {
                return ccdik_joints[joint_index].constraint_angle_enabled;
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
            else if (ccdik_data[3] == "constraint_angle_in_localspace") {
                return ccdik_joints[joint_index].constraint_angle_in_localspace;
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
        tmp_dict.Add("name", "CCDIK/solve_mode");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.Enum);
        tmp_dict.Add("hint_string", "Normal, High Quality, Backwards, Forward And Backwards");
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
            tmp_dict.Add("name", ccdik_string + i.ToString() + "/constraint_angle_enabled");
            tmp_dict.Add("type", (int)Variant.Type.Bool);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            if (ccdik_joints[i].constraint_angle_enabled == true) {
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

                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", ccdik_string + i.ToString() + "/constraint_angle_in_localspace");
                tmp_dict.Add("type", (int)Variant.Type.Bool);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);
            }
        }

        return list;
    }

    public override void _ExecuteModification(Twisted_ModifierStack2D modifier_stack, double delta)
    {
        base._ExecuteModification(modifier_stack, delta);

        if (target_node == null || tip_node == null) {
            GD.PrintErr("Cannot execute 2D CCDIK: No target or tip found!");
            return;
        }

        if (solve_mode == 0) // Normal
        {
            for (int i = 0; i < ccdik_joints.Length; i++) {
                _ExecuteCCDIKJoint(i);
            }
        }
        else if (solve_mode == 1) // High quality
        {
            for (int i = 0; i < ccdik_joints.Length; i++) {
                for (int j = i; j < ccdik_joints.Length; j++) {
                    _ExecuteCCDIKJoint(j);
                }
            }
        }
        else if (solve_mode == 2) // Backwards
        {
            for (int i = ccdik_joints.Length-1; i >= 0; i--) {
                _ExecuteCCDIKJoint(i);
            }
        }
        else if (solve_mode == 3) // Forward and Backwards
        {
            for (int i = ccdik_joints.Length-1; i >= 0; i--) {
                _ExecuteCCDIKJoint(i);
            }
            for (int i = 0; i < ccdik_joints.Length; i++) {
                _ExecuteCCDIKJoint(i);
            }
        }
        else // Unknown/Unsupported - use normal CCDIK
        {
            GD.PrintErr("Unknown 2D CCDIK mode! Solving using normal CCDIK instead");
            for (int i = 0; i < ccdik_joints.Length; i++) {
                _ExecuteCCDIKJoint(i);
            }
        }
    }

    private void _ExecuteCCDIKJoint(int joint_index) {
        Twisted_Bone2D twisted_bone = ccdik_joints[joint_index].twisted_bone;
        if (twisted_bone == null) {
            if (ccdik_joints[joint_index].path_to_twisted_bone != null) {
                twisted_bone = GetNodeOrNull<Twisted_Bone2D>(ccdik_joints[joint_index].path_to_twisted_bone);
                if (twisted_bone == null) {
                    GD.PrintErr("Cannot execute CCDIK joint " + joint_index.ToString() + ": No bone found!");
                    return;
                }
            }
        }
        twisted_bone.set_executing_ik(true);

        // Get the transforms in global-pose space
        Transform2D bone_pose = Twisted_2DFunctions.world_transform_to_global_pose(twisted_bone.GlobalTransform, twisted_bone.get_skeleton());
        Transform2D tip_pose = Twisted_2DFunctions.world_transform_to_global_pose(tip_node.GlobalTransform, twisted_bone.get_skeleton());
        Transform2D target_pose = Twisted_2DFunctions.world_transform_to_global_pose(target_node.GlobalTransform, twisted_bone.get_skeleton());

        float bone_to_tip = bone_pose.Origin.AngleToPoint(tip_pose.Origin);
        float bone_to_target = bone_pose.Origin.AngleToPoint(target_pose.Origin);
        bone_pose = bone_pose.RotatedLocal(bone_to_target - bone_to_tip);

        // We can skip handling the bone_angle here, as we're just applying a delta

        // Convert back to a world transform
        twisted_bone.GlobalTransform = Twisted_2DFunctions.global_pose_to_world_transform(bone_pose, twisted_bone.get_skeleton());

        // Constrain?
        if (ccdik_joints[joint_index].constraint_angle_enabled == true) {
            if (ccdik_joints[joint_index].constraint_angle_in_localspace == true) {
                twisted_bone.Rotation = Twisted_2DFunctions.clamp_angle(
                    twisted_bone.Rotation, ccdik_joints[joint_index].constraint_angle_min,
                    ccdik_joints[joint_index].constraint_angle_max, ccdik_joints[joint_index].constraint_angle_invert);
            }
            else {
                twisted_bone.GlobalRotation = Twisted_2DFunctions.clamp_angle(
                    twisted_bone.GlobalRotation, ccdik_joints[joint_index].constraint_angle_min,
                    ccdik_joints[joint_index].constraint_angle_max, ccdik_joints[joint_index].constraint_angle_invert);
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

        for (int i = 0; i < ccdik_joints.Length; i++) {
            draw_angle_constraints(ccdik_joints[i].twisted_bone,
                ccdik_joints[i].constraint_angle_min, ccdik_joints[i].constraint_angle_max, ccdik_joints[i].constraint_angle_invert, 
                ccdik_joints[i].constraint_angle_in_localspace, ccdik_joints[i].constraint_angle_enabled);
        }
    }
}