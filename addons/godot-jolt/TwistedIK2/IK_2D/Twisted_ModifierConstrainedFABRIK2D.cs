using Godot;
using System;

/// <summary>
/// A modifier that uses the Forwards And Backwards Inverse Kinematics (FABRIK) algorithm
/// to rotate a series of joints so the tip of the last joint touches the target position.
/// </summary>
[Tool]
public partial class Twisted_ModifierConstrainedFABRIK2D : Twisted_ModifierFABRIK2D
{

    /// <summary>
    /// The Struct used to hold all of the data for each joint in the FABRIK joint chain.
    /// </summary>
    public struct FABRIK_JOINT_CONSTRAINT_DATA {
        public bool constraint_enabled;
        public float constraint_angle_min;
        public float constraint_angle_max;
        public bool constraint_angle_inverted;
        public bool constraint_angle_in_localscape;

        public FABRIK_JOINT_CONSTRAINT_DATA(bool is_enabled) {
            this.constraint_enabled = is_enabled;
            this.constraint_angle_min = 0;
            this.constraint_angle_max = Mathf.Tau;
            this.constraint_angle_inverted = false;
            this.constraint_angle_in_localscape = true;
        }
    }
    /// <summary>
    /// All of the joints in the FABRIK chain.
    /// </summary>
    public FABRIK_JOINT_CONSTRAINT_DATA[] fabrik_constraints = new FABRIK_JOINT_CONSTRAINT_DATA[0];

    // Needed for resetting the joint positions
    private Vector2[] local_positions = new Vector2[0];

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "FABRIK/joint_count") {
            int num = (int)value;

            FABRIK_JOINT[] new_array = new FABRIK_JOINT[num];
            FABRIK_JOINT_CONSTRAINT_DATA[] new_constraint_array = new FABRIK_JOINT_CONSTRAINT_DATA[num];
            Vector2[] new_local_pos_array = new Vector2[num];
            for (int i = 0; i < num; i++) {
                if (i < fabrik_joints.Length) {
                    new_array[i] = fabrik_joints[i];
                    new_constraint_array[i] = fabrik_constraints[i];
                    new_local_pos_array[i] = local_positions[i];
                } else {
                    new_array[i] = new FABRIK_JOINT(null);
                    new_constraint_array[i] = new FABRIK_JOINT_CONSTRAINT_DATA(false);
                    new_local_pos_array[i] = Vector2.Zero;
                }
            }
            fabrik_joints = new_array;
            fabrik_constraints = new_constraint_array;
            local_positions = new_local_pos_array;

            fabrik_transfroms = new Transform2D[num];
            for (int i = 0; i < fabrik_transfroms.Length; i++) {
                fabrik_transfroms[i] = new Transform2D();
            }

            NotifyPropertyListChanged();
            return true;
        }
        else if (property.ToString().StartsWith("FABRIK/joint/")) {
            String[] fabrik_data = property.ToString().Split("/");
            int joint_index = fabrik_data[2].ToInt();
            
            if (joint_index < 0 || joint_index > fabrik_joints.Length-1) {
                GD.PrintErr("ERROR - Cannot get FABRIK joint at index " + joint_index.ToString());
                return false;
            }

            if (fabrik_data[3] == "constraint_enabled") {
                FABRIK_JOINT_CONSTRAINT_DATA current_constraint = fabrik_constraints[joint_index];
                current_constraint.constraint_enabled = (bool)value;
                fabrik_constraints[joint_index] = current_constraint;
                NotifyPropertyListChanged();
                update_gizmo();
            }
            else if (fabrik_data[3] == "constraint_angle_min") {
                FABRIK_JOINT_CONSTRAINT_DATA current_constraint = fabrik_constraints[joint_index];
                current_constraint.constraint_angle_min = Mathf.DegToRad((float)value);
                fabrik_constraints[joint_index] = current_constraint;
                update_gizmo();
            }
            else if (fabrik_data[3] == "constraint_angle_max") {
                FABRIK_JOINT_CONSTRAINT_DATA current_constraint = fabrik_constraints[joint_index];
                current_constraint.constraint_angle_max = Mathf.DegToRad((float)value);
                fabrik_constraints[joint_index] = current_constraint;
                update_gizmo();
            }
            else if (fabrik_data[3] == "constraint_angle_inverted") {
                FABRIK_JOINT_CONSTRAINT_DATA current_constraint = fabrik_constraints[joint_index];
                current_constraint.constraint_angle_inverted = (bool)value;
                fabrik_constraints[joint_index] = current_constraint;
                update_gizmo();
            }
            else if (fabrik_data[3] == "constraint_angle_in_localscape") {
                FABRIK_JOINT_CONSTRAINT_DATA current_constraint = fabrik_constraints[joint_index];
                current_constraint.constraint_angle_in_localscape = (bool)value;
                fabrik_constraints[joint_index] = current_constraint;
                update_gizmo();
            }
            else {
                try {
                    return base._Set(property, value);
                } catch {
                    return false;
                }
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
        if (property.ToString().StartsWith("FABRIK/joint/")) {
            String[] fabrik_data = property.ToString().Split("/");
            int joint_index = fabrik_data[2].ToInt();

            if (fabrik_data[3] == "constraint_enabled") {
                return fabrik_constraints[joint_index].constraint_enabled;
            }
            else if (fabrik_data[3] == "constraint_angle_min") {
                return Mathf.RadToDeg(fabrik_constraints[joint_index].constraint_angle_min);
            }
            else if (fabrik_data[3] == "constraint_angle_max") {
                return Mathf.RadToDeg(fabrik_constraints[joint_index].constraint_angle_max);
            }
            else if (fabrik_data[3] == "constraint_angle_inverted") {
                return fabrik_constraints[joint_index].constraint_angle_inverted;
            }
            else if (fabrik_data[3] == "constraint_angle_in_localscape") {
                return fabrik_constraints[joint_index].constraint_angle_in_localscape;
            }
        }
        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = base._GetPropertyList();
        Godot.Collections.Dictionary tmp_dict;

        // The FABRIK Joints
        // ===================
        String fabrik_string = "FABRIK/joint/";
        for (int i = 0; i < fabrik_joints.Length; i++) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", fabrik_string + i.ToString() + "/constraint_enabled");
            tmp_dict.Add("type", (int)Variant.Type.Bool);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            if (fabrik_constraints[i].constraint_enabled == true) {
                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/constraint_angle_min");
                tmp_dict.Add("type", (int)Variant.Type.Float);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);

                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/constraint_angle_max");
                tmp_dict.Add("type", (int)Variant.Type.Float);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);

                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/constraint_angle_inverted");
                tmp_dict.Add("type", (int)Variant.Type.Bool);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);

                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/constraint_angle_in_localscape");
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
        for (int i = 0; i < fabrik_joints.Length; i++) {
            FABRIK_JOINT current_joint = fabrik_joints[i];
            if (current_joint.twisted_bone == null) {
                current_joint.twisted_bone = GetNodeOrNull<Twisted_Bone2D>(current_joint.path_to_twisted_bone);
                if (current_joint.twisted_bone == null) {
                    GD.PrintErr("Cannot find TwistedBone2D for joint: " + i.ToString() + ". ABORTING IK!");
                    return;
                }
                fabrik_joints[i] = current_joint;
            }
            fabrik_transfroms[i] = Twisted_2DFunctions.world_transform_to_global_pose(current_joint.twisted_bone.GlobalTransform, modifier_stack.skeleton);
            local_positions[i] = current_joint.twisted_bone.Position;
        }

        base._ExecuteModification(modifier_stack, delta);
    }

    public override void chain_backwards() {
        // Set the position of the final joint to the target position
        int final_bone_index = fabrik_joints.Length-1;
        
        chain_apply_single(final_bone_index, fabrik_constraints[final_bone_index].constraint_enabled, false);
        
        Transform2D final_joint_trans = fabrik_transfroms[final_bone_index];
        float final_joint_angle = final_joint_trans.Rotation + fabrik_joints[final_bone_index].twisted_bone.bone_angle;
        Vector2 final_bone_angle_vector = new Vector2(Mathf.Cos(final_joint_angle), Mathf.Sin(final_joint_angle));
        final_joint_trans.Origin = target_transform.Origin - (final_bone_angle_vector * fabrik_joints[final_bone_index].twisted_bone.bone_length);
        fabrik_transfroms[final_bone_index] = final_joint_trans;

        // For all other bones, move them towards the target
        for (int i = final_bone_index; i >= 1; i--) {

            chain_apply_single(i-1, fabrik_constraints[i].constraint_enabled, false);
            chain_apply_single(i, fabrik_constraints[i].constraint_enabled, false);

            Transform2D next_bone_trans = fabrik_transfroms[i];
            Transform2D current_bone_trans = fabrik_transfroms[i-1];

            float length = fabrik_joints[i-1].twisted_bone.bone_length / (next_bone_trans.Origin - current_bone_trans.Origin).Length();
            current_bone_trans.Origin = next_bone_trans.Origin.Lerp(current_bone_trans.Origin, length);

            fabrik_transfroms[i-1] = current_bone_trans;
        }
    }

    public override void chain_forwards() {
        chain_apply_single(0, fabrik_constraints[0].constraint_enabled, true);

        for (int i = 0; i < fabrik_joints.Length-1; i++) {
            chain_apply_single(i, fabrik_constraints[i].constraint_enabled, true);

            Transform2D current_bone_trans = fabrik_transfroms[i];
            Transform2D next_bone_trans = fabrik_transfroms[i+1];

            float length = fabrik_joints[i].twisted_bone.bone_length / (current_bone_trans.Origin - next_bone_trans.Origin).Length();
            next_bone_trans.Origin = current_bone_trans.Origin.Lerp(next_bone_trans.Origin, length);

            fabrik_transfroms[i+1] = next_bone_trans;

            chain_apply_single(i+1, fabrik_constraints[i+1].constraint_enabled, true);
        }
        chain_apply_single(fabrik_joints.Length-1, fabrik_constraints[fabrik_joints.Length-1].constraint_enabled, true);
    }

    public override void chain_apply() {
        // We do not need this function anymore!
        return;
    }

    private void chain_apply_single(int joint_index, bool do_constraints = true, bool set_to_local=true) {
        fabrik_joints[joint_index].twisted_bone.GlobalTransform = Twisted_2DFunctions.global_pose_to_world_transform(fabrik_transfroms[joint_index], fabrik_joints[joint_index].twisted_bone.get_skeleton());

        if (joint_index == fabrik_joints.Length-1) {
            if (fabrik_joints[joint_index].use_target_rotation == true) {
                fabrik_joints[joint_index].twisted_bone.Rotation = target_node.Rotation;
            } else {
                Vector2 bone_dir = fabrik_joints[joint_index].twisted_bone.GlobalPosition.DirectionTo(target_node.GlobalTransform.Origin);
                fabrik_joints[joint_index].twisted_bone.LookAt(fabrik_joints[joint_index].twisted_bone.GlobalPosition + bone_dir);
            }
        }
        else {
            Transform2D next_bone_trans = Twisted_2DFunctions.global_pose_to_world_transform(fabrik_transfroms[joint_index+1], fabrik_joints[joint_index].twisted_bone.get_skeleton());
            Vector2 bone_dir = fabrik_joints[joint_index].twisted_bone.GlobalPosition.DirectionTo(next_bone_trans.Origin);
            fabrik_joints[joint_index].twisted_bone.LookAt(fabrik_joints[joint_index].twisted_bone.GlobalPosition + bone_dir);
        }

        // Additional rotation
        fabrik_joints[joint_index].twisted_bone.Rotate(fabrik_joints[joint_index].additional_rotation);
        // Bone angle adjustment
        fabrik_joints[joint_index].twisted_bone.Rotate(-fabrik_joints[joint_index].twisted_bone.bone_angle);

        if (fabrik_constraints[joint_index].constraint_enabled == true) {
            fabrik_joints[joint_index].twisted_bone.Rotation = Twisted_2DFunctions.clamp_angle(
                fabrik_joints[joint_index].twisted_bone.Rotation, fabrik_constraints[joint_index].constraint_angle_min,
                fabrik_constraints[joint_index].constraint_angle_max, fabrik_constraints[joint_index].constraint_angle_inverted);
        }

        // Position
        if (set_to_local == true) {
            fabrik_joints[joint_index].twisted_bone.Position = local_positions[joint_index];
        }
        
        fabrik_transfroms[joint_index] = Twisted_2DFunctions.world_transform_to_global_pose(fabrik_joints[joint_index].twisted_bone.GlobalTransform, fabrik_joints[joint_index].twisted_bone.get_skeleton());
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

        for (int i = 0; i < fabrik_joints.Length; i++) {
            draw_angle_constraints(fabrik_joints[i].twisted_bone,
                fabrik_constraints[i].constraint_angle_min, fabrik_constraints[i].constraint_angle_max, fabrik_constraints[i].constraint_angle_inverted,
                fabrik_constraints[i].constraint_angle_in_localscape, fabrik_constraints[i].constraint_enabled);
        }
    }
}
