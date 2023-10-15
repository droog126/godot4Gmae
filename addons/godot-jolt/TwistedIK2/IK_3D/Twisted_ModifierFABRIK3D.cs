using Godot;
using System;

/// <summary>
/// A modifier that uses the Forwards And Backwards Inverse Kinematics (FABRIK) algorithm
/// to rotate a series of joints so the tip of the last joint touches the target position.
/// </summary>
[Tool]
public partial class Twisted_ModifierFABRIK3D : Twisted_Modifier3D
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
    /// The distance from the target required to stop the FABRIK chain from executing.
    /// </summary>
    public float chain_tolerance = 0.01f;
    /// <summary>
    /// The maximum amount of iterations the FABRIK chain can perform in a single solve.
    /// The higher the number, the better FABRIK may do when solving large/far chains, but more costly to performance.
    /// </summary>
    public int chain_max_iterations = 10;
    private Transform3D chain_origin_global_pose;

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
    /// The Struct used to hold all of the data for each joint in the FABRIK joint chain.
    /// </summary>
    public struct FABRIK_JOINT {
        public NodePath path_to_twisted_bone;
        public Twisted_Bone3D twisted_bone;
        /// <summary>
        /// The magnet position for this joint, used to push the joint in a given direction.
        /// This helps define how the joint bends when it needs to contract.
        /// 
        /// NOTE: The magnet position needs to be in bone global space, not world space (like a GlobalTransform from a node)
        /// You can convert a GlobalTransform from world space to bone global space using the <c>world_transform_to_global_pose</c>
        /// in Twisted_Skeleton3D. There should be an example in the Utility scripts for setting the magnet position via code.
        /// </summary>
        public Vector3 magnet_position;
        public Vector3 additional_rotation;
        /// <summary>
        /// If <c>true</c>, this Bone will use the target's rotation Basis instead of calculating its own.
        /// This only works for the last bone in the FABRIK chain!
        /// </summary>
        public bool use_target_basis;

        /// <summary>
        /// The basis-axis to use as the base reference when resetting the bone prior to performing the look-at.
        /// This should be left at Y_Basis for the majority of uses, but there are cases where you may want to
        /// start at another basis to get correct rotation results. If you are getting the incorrect rotation
        /// compared to what you expect, try changing the basis direction and see if it helps.
        /// </summary>
        public LOOKAT_BASIS_DIRECTION lookat_basis_direction = LOOKAT_BASIS_DIRECTION.Y_BASIS;

        public FABRIK_JOINT(NodePath path) {
            this.path_to_twisted_bone = path;
            this.twisted_bone = null;
            this.magnet_position = Vector3.Zero;
            this.additional_rotation = Vector3.Zero;
            this.use_target_basis = false;
        }
    }
    /// <summary>
    /// All of the joints in the FABRIK chain.
    /// </summary>
    public FABRIK_JOINT[] fabrik_joints = new FABRIK_JOINT[0];

    private Transform3D[] fabrik_transfroms = new Transform3D[0];
    private Twisted_Skeleton3D fabrik_skeleton = null;
    private Transform3D target_transform = new Transform3D();

    /// <summary>
    /// If <c>true</c>, FABRIK will use <c>look_at</c> to calculate the joint rotation.
    /// If <c>false</c>, FABRIK will use Quaternion-based rotation instead (does not allow for additional rotation, but does not twist the bones)
    /// </summary>
    public bool use_lookat = true;

    private int joint_count = 0;

    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node3D>(path_to_target);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "FABRIK/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node3D>(path_to_target);
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
        else if (property == "FABRIK/use_lookat") {
            use_lookat = (bool)value;
            NotifyPropertyListChanged();
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

            fabrik_transfroms = new Transform3D[joint_count];
            for (int i = 0; i < fabrik_transfroms.Length; i++) {
                fabrik_transfroms[i] = new Transform3D();
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
                    current_joint.twisted_bone = GetNodeOrNull<Twisted_Bone3D>(current_joint.path_to_twisted_bone);
                }
            }
            else if (fabrik_data[3] == "magnet_position") {
                current_joint.magnet_position = (Vector3)value;
            }
            else if (fabrik_data[3] == "additional_rotation") {
                Vector3 tmp = (Vector3)value;
                tmp.X = Mathf.DegToRad(tmp.X);
                tmp.Y = Mathf.DegToRad(tmp.Y);
                tmp.Z = Mathf.DegToRad(tmp.Z);
                current_joint.additional_rotation = tmp;
            }
            else if (fabrik_data[3] == "use_target_basis") {
                current_joint.use_target_basis = (bool)value;
            }
            else if (fabrik_data[3] == "basis_direction") {
                current_joint.lookat_basis_direction = (LOOKAT_BASIS_DIRECTION)(int)value;
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
        else if (property == "FABRIK/use_lookat") {
            return use_lookat;
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
                Vector3 tmp = fabrik_joints[joint_index].additional_rotation;
                tmp.X = Mathf.RadToDeg(tmp.X);
                tmp.Y = Mathf.RadToDeg(tmp.Y);
                tmp.Z = Mathf.RadToDeg(tmp.Z);
                return tmp;
            }
            else if (fabrik_data[3] == "use_target_basis") {
                return fabrik_joints[joint_index].use_target_basis;
            }
            else if (fabrik_data[3] == "basis_direction") {
                return (int)fabrik_joints[joint_index].lookat_basis_direction;
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
        tmp_dict.Add("name", "FABRIK/use_lookat");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
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
            tmp_dict.Add("type", (int)Variant.Type.Vector3);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);

            if (use_lookat == true) {
                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/additional_rotation");
                tmp_dict.Add("type", (int)Variant.Type.Vector3);
                tmp_dict.Add("hint", (int)PropertyHint.None);
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);

                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/basis_direction");
                tmp_dict.Add("type", (int)Variant.Type.Int);
                tmp_dict.Add("hint", (int)PropertyHint.Enum);
                tmp_dict.Add("hint_string", "X_Basis, Y_Basis, Z_Basis");
                tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
                list.Add(tmp_dict);
            }

            if (i == fabrik_joints.Length-1) {
                tmp_dict = new Godot.Collections.Dictionary();
                tmp_dict.Add("name", fabrik_string + i.ToString() + "/use_target_basis");
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

        if (target_node == null) {
            GD.PrintErr("Cannot execute FABRIK: No target found!");
            return;
        }
        if (fabrik_joints.Length <= 0) {
            GD.PrintErr("Cannot execute FABRIK: No FABRIK joints found!");
            return;
        }

        fabrik_skeleton = modifier_stack.twisted_skeleton;
        if (fabrik_skeleton == null) {
            GD.PrintErr("Cannot execute FABRIK: Modifier stack does not contain skeleton!");
            return;
        }
        target_transform = fabrik_skeleton.world_transform_to_global_pose(target_node.GlobalTransform);

        for (int i = 0; i < fabrik_joints.Length; i++) {
            FABRIK_JOINT current_joint = fabrik_joints[i];
            if (current_joint.twisted_bone == null) {
                current_joint.twisted_bone = GetNodeOrNull<Twisted_Bone3D>(current_joint.path_to_twisted_bone);
                if (current_joint.twisted_bone == null) {
                    GD.PrintErr("Cannot find TwistedBone3D for joint: " + i.ToString() + ". ABORTING IK!");
                    return;
                }
                fabrik_joints[i] = current_joint;
            }
            fabrik_transfroms[i] = fabrik_skeleton.world_transform_to_global_pose(current_joint.twisted_bone.GlobalTransform);
        }
        chain_origin_global_pose = fabrik_transfroms[0];

        int final_bone_index = fabrik_joints.Length-1;
        float target_distance = fabrik_transfroms[final_bone_index].Origin.DistanceTo(target_transform.Origin);
        int chain_iterations = 0;

        while (target_distance > chain_tolerance) {
            // Apply magnet positions (TODO: optimize this!)
            for (int i = 0; i < fabrik_joints.Length; i++) {
                fabrik_transfroms[i].Origin += fabrik_joints[i].magnet_position;
            }

            chain_backwards();
            chain_forwards();

            target_distance = fabrik_transfroms[final_bone_index].Origin.DistanceTo(target_transform.Origin);
            
            chain_iterations += 1;
            if (chain_iterations >= chain_max_iterations) {
                break;
            }
        }
        chain_apply(modifier_stack);
    }

    private void chain_backwards() {
        // Set the position of the final joint to the target position
        int final_bone_index = fabrik_joints.Length-1;
        Transform3D final_joint_trans = fabrik_transfroms[final_bone_index];
        float final_joint_length = fabrik_joints[final_bone_index].twisted_bone.get_global_pose_length();
        final_joint_trans.Origin = target_transform.Origin - (-final_joint_trans.Basis.Z.Normalized() * final_joint_length);
        fabrik_transfroms[final_bone_index] = final_joint_trans;

        // For all other bones, move them towards the target
        for (int i = final_bone_index; i >= 1; i--) {
            Transform3D next_bone_trans = fabrik_transfroms[i];
            Transform3D current_bone_trans = fabrik_transfroms[i-1];

            float length = fabrik_joints[i-1].twisted_bone.get_global_pose_length() / (next_bone_trans.Origin - current_bone_trans.Origin).Length();
            current_bone_trans.Origin = next_bone_trans.Origin.Lerp(current_bone_trans.Origin, length);

            fabrik_transfroms[i-1] = current_bone_trans;
        }
    }

    private void chain_forwards() {
        // Set the root at the initial position
        Transform3D root_trans = fabrik_transfroms[0];
        root_trans.Origin = chain_origin_global_pose.Origin;
        fabrik_transfroms[0] = root_trans;

        for (int i = 0; i < fabrik_joints.Length-1; i++) {
            Transform3D current_bone_trans = fabrik_transfroms[i];
            Transform3D next_bone_trans = fabrik_transfroms[i+1];

            float length = fabrik_joints[i].twisted_bone.get_global_pose_length() / (current_bone_trans.Origin - next_bone_trans.Origin).Length();
            next_bone_trans.Origin = current_bone_trans.Origin.Lerp(next_bone_trans.Origin, length);

            fabrik_transfroms[i+1] = next_bone_trans;
        }
    }

    private void chain_apply(Twisted_ModifierStack3D stack) {
        for (int i = 0; i < fabrik_joints.Length; i++) {
            Transform3D bone_trans = fabrik_transfroms[i];

            if (i == fabrik_joints.Length-1) {
                if (fabrik_joints[i].use_target_basis == true) {
                    bone_trans.Basis = target_transform.Basis.Orthonormalized();
                } else {
                    if (use_lookat == true) {
                        Vector3 bone_up_dir = fabrik_joints[i].twisted_bone.get_reset_bone_global_pose().Basis.Y;
                        if (fabrik_joints[i].lookat_basis_direction == LOOKAT_BASIS_DIRECTION.X_BASIS) {
                            bone_up_dir = fabrik_joints[i].twisted_bone.get_reset_bone_global_pose().Basis.X;
                        } else if (fabrik_joints[i].lookat_basis_direction == LOOKAT_BASIS_DIRECTION.Z_BASIS) {
                            bone_up_dir = fabrik_joints[i].twisted_bone.get_reset_bone_global_pose().Basis.Z;
                        }
                        bone_up_dir = bone_up_dir.Normalized();

                        Vector3 bone_dir = bone_trans.Origin.DirectionTo(target_transform.Origin);
                        bone_trans = bone_trans.LookingAt(bone_trans.Origin + bone_dir, bone_up_dir);
                    }
                    else {
                        bone_trans = Twisted_3DFunctions.quat_based_lookat(bone_trans, target_transform.Origin);
                    }
                }
            }
            else {
                if (use_lookat == true) {
                    Transform3D next_bone_trans = fabrik_transfroms[i+1];
                    
                    Vector3 bone_up_dir = fabrik_joints[i].twisted_bone.get_reset_bone_global_pose().Basis.Y;
                    if (fabrik_joints[i].lookat_basis_direction == LOOKAT_BASIS_DIRECTION.X_BASIS) {
                        bone_up_dir = fabrik_joints[i].twisted_bone.get_reset_bone_global_pose().Basis.X;
                    } else if (fabrik_joints[i].lookat_basis_direction == LOOKAT_BASIS_DIRECTION.Z_BASIS) {
                        bone_up_dir = fabrik_joints[i].twisted_bone.get_reset_bone_global_pose().Basis.Z;
                    }
                    bone_up_dir = bone_up_dir.Normalized();
                    
                    Vector3 bone_dir = bone_trans.Origin.DirectionTo(next_bone_trans.Origin);
                    bone_trans = bone_trans.LookingAt(bone_trans.Origin + bone_dir, bone_up_dir);
                }
                else {
                    Transform3D next_bone_trans = fabrik_transfroms[i+1];
                    bone_trans = Twisted_3DFunctions.quat_based_lookat(bone_trans, next_bone_trans.Origin);
                }
            }
            fabrik_transfroms[i] = bone_trans;
        }

        for (int i = 0; i < fabrik_joints.Length; i++) {
            // Get the transform
            fabrik_joints[i].twisted_bone.GlobalTransform = fabrik_skeleton.global_pose_to_world_transform(fabrik_transfroms[i]);

            // Apply additional rotation (only supported in LookAt currently)
            if (use_lookat == true) {
                fabrik_joints[i].twisted_bone.RotateObjectLocal(Vector3.Right, fabrik_joints[i].additional_rotation.X);
                fabrik_joints[i].twisted_bone.RotateObjectLocal(Vector3.Up, fabrik_joints[i].additional_rotation.Y);
                fabrik_joints[i].twisted_bone.RotateObjectLocal(Vector3.Forward, fabrik_joints[i].additional_rotation.Z);
            }

            // Keep the scale consistent with the global pose
            fabrik_joints[i].twisted_bone.Scale = fabrik_joints[i].twisted_bone.get_reset_bone_global_pose(false).Basis.Scale;

            if (force_bone_application == true) {
                fabrik_joints[i].twisted_bone.force_apply_transform();
            }
        }
    }
}