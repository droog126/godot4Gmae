using Godot;
using System;

/// <summary>
/// A motor node that can rotate a series of Twisted_PhysicsBone2D nodes to look at a target.
/// Currently, is similar to the LookAt node and the Jiggle node, but using Godot physics rather than IK.
/// 
/// Hopefully, in the future, this will allow for dynamic, controllable joints! Very WIP right now though as
/// the results are not as controllable as the dyanmic joints in other game engines like Unity or Unreal engine
/// and I ideally want to make this plugin as powerful as those dynamic joints.
/// </summary>
[Tool]
public partial class Twisted_PhysicsBoneMotor2D : Node2D
{
    /// <summary>
    /// Data for each joint in the physics joint chain. Right now its rather sparse.
    /// </summary>
    public struct MOTOR_BONE2D {
        public NodePath twisted_physics_bone_path;
        public Twisted_PhysicsBone2D physics_bone;

        public MOTOR_BONE2D(NodePath path) {
            this.twisted_physics_bone_path = path;
            this.physics_bone = null;
        }
    }
    /// <summary>
    /// All of the Twisted_PhysicsBone2D bones/joints in the physics chain
    /// </summary>
    public MOTOR_BONE2D[] motor_joints = new MOTOR_BONE2D[0];

    /// <summary>
    /// A NodePath to the Node2D-based node that is used as the target position
    /// </summary>
    public NodePath path_to_target = null;
    /// <summary>
    /// A reference to the Node2D-based node that is used as the target position
    /// </summary>
    public Node2D target_node = null;

    /// <summary>
    /// The amount of strength the motor applies to the Twisted_PhysicsBone2D bones in the physics chain.
    /// </summary>
    public float motor_strength = 1.0f;
    /// <summary>
    /// If <c>true</c>, the motor will rotate the Twisted_PhysicsBone2D bones in the physics chain to look at the target.
    /// </summary>
    public bool motor_enabled = false;

    /// <summary>
    /// A NodePath to the Node2D-based node that is located at the end of the PhysicsBone2D chain
    /// (Needed to accurately rotate the chain - similar to CCDIK in that way)
    /// </summary>
    public NodePath path_to_tip = null;
    /// <summary>
    /// The Node2D-based node that is located at the end of the PhysicsBone2D chain
    /// </summary>
    public Node2D tip_node = null;

    private int joint_count = 0;


    public override void _Ready()
    {
        if (path_to_target != null) {
            target_node = GetNodeOrNull<Node2D>(path_to_target);
        }
        if (path_to_tip != null) {
            tip_node = GetNodeOrNull<Node2D>(path_to_tip);
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "Motor/target") {
            path_to_target = (NodePath)value;
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            return true;
        }
        else if (property == "Motor/tip") {
            path_to_tip = (NodePath)value;
            if (path_to_tip != null) {
                tip_node = GetNodeOrNull<Node2D>(path_to_tip);
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

            MOTOR_BONE2D[] new_array = new MOTOR_BONE2D[joint_count];
            for (int i = 0; i < joint_count; i++) {
                if (i < motor_joints.Length) {
                    new_array[i] = motor_joints[i];
                } else {
                    new_array[i] = new MOTOR_BONE2D(null);
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
            MOTOR_BONE2D current_joint = motor_joints[joint_index];

            if (motor_data[3] == "twisted_physics_bone") {
                current_joint.twisted_physics_bone_path = (NodePath)value;
                if (current_joint.twisted_physics_bone_path != null) {
                    current_joint.physics_bone = GetNodeOrNull<Twisted_PhysicsBone2D>(current_joint.twisted_physics_bone_path);
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
        else if (property == "Motor/tip") {
            return path_to_tip;
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

            if (motor_data[3] == "twisted_physics_bone" && joint_index < motor_joints.Length) {
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
        tmp_dict.Add("name", "Motor/tip");
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
    /// Rotates a Twisted_PhysicsBone2D node to look at the target using its angular velocity.
    /// Its very similar to the <c>look_at_target</c> function in Twisted_PhysicsBone2D.
    /// </summary>
    public void motor_joint_look_at_target(MOTOR_BONE2D motor_bone, Vector2 target_position, float look_at_strength) {
        Vector2 tip_to_target = tip_node.GlobalPosition.DirectionTo(target_node.GlobalPosition);
        Vector2 self_to_tip = motor_bone.physics_bone.GlobalPosition.DirectionTo(tip_node.GlobalPosition);
        self_to_tip = self_to_tip.Rotated(-motor_bone.physics_bone.twisted_bone_2d.bone_angle);

        float target_direction_delta = self_to_tip.Dot(tip_to_target) * look_at_strength;
        motor_bone.physics_bone.AngularVelocity = -target_direction_delta;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Engine.IsEditorHint() == true) {
            return;
        }
        if (IsInsideTree() == false) {
            return;
        }
        if (motor_joints.Length <= 0 || motor_enabled == false) {
            return;
        }
        if (target_node == null) {
            if (path_to_target != null) {
                target_node = GetNodeOrNull<Node2D>(path_to_target);
            }
            if (target_node == null) {
                GD.PrintErr("PhysicsMotor2D: ", Name, " - cannot find target node or target node is not set!");
                return;
            }
        }
        if (tip_node == null) {
            if (path_to_tip != null) {
                tip_node = GetNodeOrNull<Node2D>(path_to_tip);
            }
            if (tip_node == null) {
                GD.PrintErr("PhysicsMotor2D: ", Name, " - cannot find tip node or tip node is not set!");
                return;
            }
        }

        for (int i = 0; i < motor_joints.Length; i++) {
            MOTOR_BONE2D current_joint = motor_joints[i];

            // Try to get the joint if its missing
            if (current_joint.physics_bone == null) {
                if (current_joint.twisted_physics_bone_path != null) {
                    current_joint.physics_bone = GetNodeOrNull<Twisted_PhysicsBone2D>(current_joint.twisted_physics_bone_path);
                }
                motor_joints[i] = current_joint;
            }
            // If it is still missing, then just ignore!
            if (current_joint.physics_bone == null) {
                continue;
            }
            motor_joint_look_at_target(current_joint, target_node.GlobalPosition, motor_strength);
        }
    }
}