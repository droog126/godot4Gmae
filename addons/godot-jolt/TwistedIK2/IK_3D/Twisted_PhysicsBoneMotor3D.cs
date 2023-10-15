using Godot;
using System;

/// <summary>
/// A motor node that can rotate a series of Twisted_PhysicsBone3D nodes to look at a target.
/// Currently, is similar to the LookAt node and the Jiggle node, but using Godot physics rather than IK.
/// 
/// Hopefully, in the future, this will allow for dynamic, controllable joints! Very WIP right now though as
/// the results are not as controllable as the dyanmic joints in other game engines like Unity or Unreal engine
/// and I ideally want to make this plugin as powerful as those dynamic joints.
/// </summary>
[Tool]
public partial class Twisted_PhysicsBoneMotor3D : Node3D
{
    /// <summary>
    /// Data for each joint in the physics joint chain. Right now its rather sparse.
    /// </summary>
    public struct MOTOR_BONE3D {
        public NodePath twisted_physics_bone_path;
        public Twisted_PhysicsBone3D physics_bone;

        public MOTOR_BONE3D(NodePath path) {
            this.twisted_physics_bone_path = path;
            this.physics_bone = null;
        }
    }
    /// <summary>
    /// All of the Twisted_PhysicsBone3D bones/joints in the physics chain
    /// </summary>
    public MOTOR_BONE3D[] motor_joints = new MOTOR_BONE3D[0];

    /// <summary>
    /// A NodePath to the Node3D-based node that is used as the target position
    /// </summary>
    public NodePath path_to_target = null;
    /// <summary>
    /// A reference to the Node3D-based node that is used as the target position
    /// </summary>
    public Node3D target_node = null;

    /// <summary>
    /// The amount of strength the motor applies to the Twisted_PhysicsBone3D bones in the physics chain.
    /// </summary>
    public float motor_strength = 1.0f;
    /// <summary>
    /// If <c>true</c>, the motor will rotate the Twisted_PhysicsBone3D bones in the physics chain to look at the target.
    /// </summary>
    public bool motor_enabled = false;

    private int joint_count = 0;


    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node3D>(path_to_target);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "Motor/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node3D>(path_to_target);
            }
            return true;
        }
        else if (property == "Motor/strength") {
            motor_strength = (float)value;
            return true;
        }
        else if (property == "Motor/enabled") {
            motor_enabled = (bool)value;
            return true;
        }
        else if (property == "Motor/joint_count") {
            joint_count = (int)value;

            MOTOR_BONE3D[] new_array = new MOTOR_BONE3D[joint_count];
            for (int i = 0; i < joint_count; i++) {
                if (i < motor_joints.Length) {
                    new_array[i] = motor_joints[i];
                } else {
                    new_array[i] = new MOTOR_BONE3D(null);
                }
            }
            motor_joints = new_array;
            NotifyPropertyListChanged();
            return true;
        }


        else if (property.ToString().StartsWith("Motor/joint/")) {
            String[] motor_data = property.ToString().Split("/");
            int joint_index = motor_data[2].ToInt();
            
            if (joint_index < 0 || joint_index > motor_joints.Length-1) {
                GD.PrintErr("ERROR - Cannot get Physics Bone joint at index " + joint_index.ToString());
                return false;
            }
            MOTOR_BONE3D current_joint = motor_joints[joint_index];

            if (motor_data[3] == "twisted_physics_bone") {
                current_joint.twisted_physics_bone_path = (NodePath)value;
                if (current_joint.twisted_physics_bone_path != null) {
                    current_joint.physics_bone = GetNodeOrNull<Twisted_PhysicsBone3D>(current_joint.twisted_physics_bone_path);
                }
            }
            motor_joints[joint_index] = current_joint;
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
        if (property == "Motor/target") {
            return path_to_target;
        }
        else if (property == "Motor/strength") {
            return motor_strength;
        }
        else if (property == "Motor/enabled") {
            return motor_enabled;
        }
        else if (property == "Motor/joint_count") {
            return joint_count;
        }
        else if (property.ToString().StartsWith("Motor/joint/")) {
            String[] motor_data = property.ToString().Split("/");
            int joint_index = motor_data[2].ToInt();

            if (motor_data[3] == "twisted_physics_bone") {
                return motor_joints[joint_index].twisted_physics_bone_path;
            }
        }
        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        Godot.Collections.Dictionary tmp_dict;

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "Motor/target");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "Motor/strength");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.Range);
        tmp_dict.Add("hint_string", "0, 8, 0.01");
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "Motor/enabled");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "Motor/joint_count");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        // The Motor Joints
        // ===================
        String motor_string = "Motor/joint/";
        for (int i = 0; i < joint_count; i++) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", motor_string + i.ToString() + "/twisted_physics_bone");
            tmp_dict.Add("type", (int)Variant.Type.NodePath);
            tmp_dict.Add("hint", (int)PropertyHint.None);
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);
        }

        return list;
    }


    /// <summary>
    /// Rotates a Twisted_PhysicsBone3D node to look at the target using its angular velocity.
    /// Its very similar to the <c>look_at_target</c> function in Twisted_PhysicsBone3D, but it
    /// takes the angular velocity applied to the parent Twisted_PhysicsBone3D into account when solving.
    /// </summary>
    /// <param name="motor_bone">The bone that is being rotated</param>
    /// <param name="target_position">The position the bones are supposed to look at</param>
    /// <param name="accumulative_quat">The Quaternion that represents the rotation applied/delta calculated by the parent physics bone</param>
    /// <param name="look_at_strength">The amount of strength applied to the rotation</param>
    /// <returns>A Quaternion that represents the rotation applied/delta calculated and used for the angular velocity</returns>
    public Quaternion motor_joint_look_at_target(MOTOR_BONE3D motor_bone, Vector3 target_position, Quaternion accumulative_quat, float look_at_strength) {
        Vector3 self_to_target = motor_bone.physics_bone.GlobalTransform.Origin.DirectionTo(target_position);
        float self_to_target_length = motor_bone.physics_bone.GlobalTransform.Origin.DistanceTo(target_position);
        
        if (self_to_target.LengthSquared() == 0) {
            return accumulative_quat;
        }

        Vector3 forward_direction = -motor_bone.physics_bone.GlobalTransform.Basis.Z.Normalized();
        forward_direction = accumulative_quat.Normalized() * forward_direction;

        Quaternion rotation_quat = Twisted_3DFunctions.quat_from_two_vectors(forward_direction, self_to_target).Normalized();
        if (rotation_quat.IsNormalized() == false) {
            return accumulative_quat;
        }

        motor_bone.physics_bone.AngularVelocity = (rotation_quat.GetEuler() * self_to_target_length) * look_at_strength;
        return (accumulative_quat * rotation_quat);
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsInsideTree() == false) {
            return;
        }
        if (target_node == null || motor_joints.Length <= 0) {
            return;
        }
        if (motor_enabled == false) {
            return;
        }
        
        Quaternion accum_quat = Quaternion.Identity;
        for (int i = 0; i < motor_joints.Length; i++) {
            MOTOR_BONE3D current_joint = motor_joints[i];

            // Try to get the joint if its missing
            if (current_joint.physics_bone == null) {
                if (current_joint.twisted_physics_bone_path != null) {
                    current_joint.physics_bone = GetNodeOrNull<Twisted_PhysicsBone3D>(current_joint.twisted_physics_bone_path);
                }
                motor_joints[i] = current_joint;
            }
            // If it is still missing, then just ignore!
            if (current_joint.physics_bone == null) {
                continue;
            }

            accum_quat = motor_joint_look_at_target(current_joint, target_node.GlobalTransform.Origin, accum_quat, motor_strength);
        }
    }
}