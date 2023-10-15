using Godot;
using System;

/// <summary>
/// A modifier that uses the Forwards And Backwards Inverse Kinematics (FABRIK) algorithm
/// to rotate a series of joints so the tip of the last joint touches the target position.
/// </summary>
[Tool]
public partial class Twisted_ModifierFABRIK2D : Twisted_Modifier2D
{
    /// <summary>
    /// A NodePath to the Node2D-based node that is used as the target position
    /// </summary>
    public NodePath path_to_target = null;
    /// <summary>
    /// A reference to the Node2D-based node that is used as the target position
    /// </summary>
    public Node2D target_node = null;
    protected Transform2D target_transform;

    /// <summary>
    /// The distance from the target required to stop the FABRIK chain from executing.
    /// </summary>
    public float chain_tolerance = 2.0f;
    /// <summary>
    /// The maximum amount of iterations the FABRIK chain can perform in a single solve.
    /// The higher the number, the better FABRIK may do when solving large/far chains, but more costly to performance.
    /// </summary>
    public int chain_max_iterations = 10;
    protected Transform2D chain_origin_global_pose;


    /// <summary>
    /// The Struct used to hold all of the data for each joint in the FABRIK joint chain.
    /// </summary>
    public struct FABRIK_JOINT {
        public NodePath path_to_twisted_bone;
        public Twisted_Bone2D twisted_bone;
        /// <summary>
        /// The magnet position for this joint, used to push the joint in a given direction.
        /// This helps define how the joint bends when it needs to contract. This position is relative to the rotation of the bone.
        /// </summary>
        public Vector2 magnet_position;
        public float additional_rotation;
        /// <summary>
        /// If <c>true</c>, this Bone will use the target's rotation instead of calculating its own.
        /// This only works for the last bone in the FABRIK chain!
        /// </summary>
        public bool use_target_rotation;

        public FABRIK_JOINT(NodePath path) {
            this.path_to_twisted_bone = path;
            this.twisted_bone = null;
            this.magnet_position = Vector2.Zero;
            this.additional_rotation = 0;
            this.use_target_rotation = false;
        }
    }
    /// <summary>
    /// All of the joints in the FABRIK chain.
    /// </summary>
    public FABRIK_JOINT[] fabrik_joints = new FABRIK_JOINT[0];

    protected Transform2D[] fabrik_transfroms = new Transform2D[0];

    private int joint_count = 0;


    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node2D>(path_to_target);
        }

        for (int i = 0; i < fabrik_joints.Length; i++) {
            if (fabrik_joints[i].twisted_bone == null) {
                fabrik_joints[i].twisted_bone = GetNodeOrNull<Twisted_Bone2D>(fabrik_joints[i].path_to_twisted_bone);
            }
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "FABRIK/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            return true;
        }
        else if (property == "FABRIK/chain_tolerance") {
            chain_tolerance = (float)value;
            return true;
        }
        else if (property == "FABRIK/chain_max_iterations") {
            chain_max_iterations = (int)value;
            return true;
        }
        else if (property == "FABRIK/joint_count") {
            joint_count = (int)value;

            FABRIK_JOINT[] new_array = new FABRIK_JOINT[joint_count];
            for (int i = 0; i < joint_count; i++) {
                if (i < fabrik_joints.Length) {
                    new_array[i] = fabrik_joints[i];
                } else {
                    new_array[i] = new FABRIK_JOINT(null);
                }
            }
            fabrik_joints = new_array;

            fabrik_transfroms = new Transform2D[joint_count];
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
            FABRIK_JOINT current_joint = fabrik_joints[joint_index];

            if (fabrik_data[3] == "twisted_bone") {
                current_joint.path_to_twisted_bone = (NodePath)value;
                if (current_joint.path_to_twisted_bone != null) {
                    current_joint.twisted_bone = GetNodeOrNull<Twisted_Bone2D>(current_joint.path_to_twisted_bone);
                }
            }
            else if (fabrik_data[3] == "magnet_position") {
                current_joint.magnet_position = (Vector2)value;
            }
            else if (fabrik_data[3] == "additional_rotation") {
                current_joint.additional_rotation = Mathf.DegToRad((float)value);
            }
            else if (fabrik_data[3] == "use_target_rotation") {
                current_joint.use_target_rotation = (bool)value;
            }
            fabrik_joints[joint_index] = current_joint;
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
        if (property == "FABRIK/target") {
            return path_to_target;
        }
        else if (property == "FABRIK/chain_tolerance") {
            return chain_tolerance;
        }
        else if (property == "FABRIK/chain_max_iterations") {
            return chain_max_iterations;
        }
        else if (property == "FABRIK/joint_count") {
            return joint_count;
        }

        
        else if (property.ToString().StartsWith("FABRIK/joint/")) {
            String[] fabrik_data = property.ToString().Split("/");
            int joint_index = fabrik_data[2].ToInt();

            if (fabrik_data[3] == "twisted_bone") {
                return fabrik_joints[joint_index].path_to_twisted_bone;
            }
            else if (fabrik_data[3] == "magnet_position") {
                return fabrik_joints[joint_index].magnet_position;
            }
            else if (fabrik_data[3] == "additional_rotation") {
                return Mathf.RadToDeg(fabrik_joints[joint_index].additional_rotation);
            }
            else if (fabrik_data[3] == "use_target_rotation") {
                return fabrik_joints[joint_index].use_target_rotation;
            }
        }
        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = base._GetPropertyList();
        Godot.Collections.Dictionary tmp_dict;

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "FABRIK/target");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "FABRIK/chain_tolerance");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "FABRIK/chain_max_iterations");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "FABRIK/joint_count");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        // The FABRIK Joints
        // ===================
        String fabrik_string = "FABRIK/joint/";
        for (int i = 0; i < joint_count; i++) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", fabrik_string + i.ToString() + "/twisted_bone");
            tmp_dict.Add("type", (int)Variant.Type.NodePath);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", fabrik_string + i.ToString() + "/magnet_position");
            tmp_dict.Add("type", (int)Variant.Type.Vector2);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", fabrik_string + i.ToString() + "/additional_rotation");
            tmp_dict.Add("type", (int)Variant.Type.Float);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            if (i == fabrik_joints.Length-1) {
                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/use_target_rotation");
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

        if (target_node == null) {
            GD.PrintErr("Cannot execute FABRIK 2D: No target found!");
            return;
        }
        if (fabrik_joints.Length <= 0) {
            GD.PrintErr("Cannot execute FABRIK 2D: No FABRIK joints found!");
            return;
        }

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
        }
        chain_origin_global_pose = fabrik_transfroms[0];

        target_transform = Twisted_2DFunctions.world_transform_to_global_pose(target_node.GlobalTransform, modifier_stack.skeleton);

        int final_bone_index = fabrik_joints.Length-1;
        float target_distance = fabrik_transfroms[final_bone_index].Origin.DistanceTo(target_transform.Origin);
        int chain_iterations = 0;

        while (target_distance > chain_tolerance) {
            // Apply magnet positions and IK execution
            for (int i = 0; i < fabrik_joints.Length; i++) {
                fabrik_transfroms[i].Origin += fabrik_joints[i].magnet_position.Rotated(modifier_stack.skeleton.Rotation);
                fabrik_joints[i].twisted_bone.set_executing_ik(true);
            }

            chain_backwards();
            chain_forwards();
            chain_apply();

            target_distance = fabrik_transfroms[final_bone_index].Origin.DistanceTo(target_transform.Origin);
            
            chain_iterations += 1;
            if (chain_iterations >= chain_max_iterations) {
                break;
            }
        }
    }

    public virtual void chain_backwards() {
        // Set the position of the final joint to the target position
        int final_bone_index = fabrik_joints.Length-1;
        Transform2D final_joint_trans = fabrik_transfroms[final_bone_index];
        float final_joint_angle = final_joint_trans.Rotation + fabrik_joints[final_bone_index].twisted_bone.bone_angle;
        Vector2 final_bone_angle_vector = new Vector2(Mathf.Cos(final_joint_angle), Mathf.Sin(final_joint_angle));
        final_joint_trans.Origin = target_transform.Origin - (final_bone_angle_vector * fabrik_joints[final_bone_index].twisted_bone.bone_length);
        fabrik_transfroms[final_bone_index] = final_joint_trans;

        // For all other bones, move them towards the target
        for (int i = final_bone_index; i >= 1; i--) {
            Transform2D next_bone_trans = fabrik_transfroms[i];
            Transform2D current_bone_trans = fabrik_transfroms[i-1];

            float length = fabrik_joints[i-1].twisted_bone.bone_length / (next_bone_trans.Origin - current_bone_trans.Origin).Length();
            current_bone_trans.Origin = next_bone_trans.Origin.Lerp(current_bone_trans.Origin, length);

            fabrik_transfroms[i-1] = current_bone_trans;
        }
    }

    public virtual void chain_forwards() {
        // Set the root at the initial position
        Transform2D root_trans = fabrik_transfroms[0];
        root_trans.Origin = chain_origin_global_pose.Origin;
        fabrik_transfroms[0] = root_trans;

        for (int i = 0; i < fabrik_joints.Length-1; i++) {
            Transform2D current_bone_trans = fabrik_transfroms[i];
            Transform2D next_bone_trans = fabrik_transfroms[i+1];

            float length = fabrik_joints[i].twisted_bone.bone_length / (current_bone_trans.Origin - next_bone_trans.Origin).Length();
            next_bone_trans.Origin = current_bone_trans.Origin.Lerp(next_bone_trans.Origin, length);

            fabrik_transfroms[i+1] = next_bone_trans;
        }
    }

    public virtual void chain_apply() {
        for (int i = 0; i < fabrik_joints.Length; i++) {
            fabrik_joints[i].twisted_bone.GlobalTransform = Twisted_2DFunctions.global_pose_to_world_transform(fabrik_transfroms[i], fabrik_joints[i].twisted_bone.get_skeleton());

            if (i == fabrik_joints.Length-1) {
                if (fabrik_joints[i].use_target_rotation == true) {
                    fabrik_joints[i].twisted_bone.Rotation = target_node.Rotation;
                } else {
                    Vector2 bone_dir = fabrik_joints[i].twisted_bone.GlobalPosition.DirectionTo(target_node.GlobalTransform.Origin);
                    fabrik_joints[i].twisted_bone.LookAt(fabrik_joints[i].twisted_bone.GlobalPosition + bone_dir);
                }
            }
            else {
                Transform2D next_bone_trans = Twisted_2DFunctions.global_pose_to_world_transform(fabrik_transfroms[i+1], fabrik_joints[i].twisted_bone.get_skeleton());
                Vector2 bone_dir = fabrik_joints[i].twisted_bone.GlobalPosition.DirectionTo(next_bone_trans.Origin);
                fabrik_joints[i].twisted_bone.LookAt(fabrik_joints[i].twisted_bone.GlobalPosition + bone_dir);
            }

            // Additional rotation
            fabrik_joints[i].twisted_bone.Rotate(fabrik_joints[i].additional_rotation);
            // Bone angle adjustment
            fabrik_joints[i].twisted_bone.Rotate(-fabrik_joints[i].twisted_bone.bone_angle);
            
            fabrik_transfroms[i] = Twisted_2DFunctions.world_transform_to_global_pose(fabrik_joints[i].twisted_bone.GlobalTransform, fabrik_joints[i].twisted_bone.get_skeleton());
        }
    }
}