using Godot;
using System;

/// <summary>
/// A Rigidbody based Twisted_Bone3D node. Great for adding physics to your skeletons using Godot's physics engine.
/// Because its physics-based, it will react to other physics objects. Can be used with physics motors for semi-manual control.
///
/// IMPORTANT: Physics bones directly under a scaled Twisted_Skeleton3D node NO LONGER WORK properly in Godot 4.0 no
/// matter how you set the scale. Why this happens I am not sure, but keeping the physics nodes separate is fixes the issue.
/// </summary>
[Tool]
public partial class Twisted_PhysicsBone3D : RigidBody3D
{

    /// <summary>
    /// If <c>true</c>, the Twisted_Bone3D node will attempt to automatically find and store a parent Twisted_Skeleton3D node.
    /// </summary>
    public bool auto_get_twisted_skeleton = true;
    /// <summary>
    /// If <c>true</c>, the Twisted_Bone3D node will attempt to automatically calculate its length using child nodes.
    /// </summary>
    public bool auto_calcualte_bone_length = true;

    /// <summary>
    /// The NodePath to the Twisted_Skeleton3D that this Twisted_Bone3D uses. This property is only used if <c>auto_get_twisted_skeleton</c> is <c>false</c>.
    /// </summary>
    public NodePath path_to_twisted_skeleton;
    /// <summary>
    /// A reference to the Twisted_Skeleton3D node that this Twisted_Bone3D uses.
    /// (Will be <c>null</c> if the Twisted_Skeleton3D cannot be found or is not set)
    /// </summary>
    public Twisted_Skeleton3D twisted_skeleton3d;

    /// <summary>
    /// The name of the bone in the Skeleton3D node that this Twisted_Bone3D is attached to.
    /// </summary>
    public string bone_name = "";
    /// <summary>
    /// The ID of the bone in the Skeleton3D node that this Twisted_Bone3D is attached to.
    /// </summary>
    public int bone_id = -1;
    /// <summary>
    /// The length of the bone, from its origin to its tip/end.
    /// </summary>
    public float bone_length;
    /// <summary>
    /// If <c>true</c>, then this Twisted_Bone3D will apply its transform to the bone in the Skeleton3D
    /// </summary>
    public bool bone_apply_transform = false;
    /// <summary>
    /// The interpolation strength used when applying the Twisted_Bone3D's Transfrom to the bone.
    /// </summary>
    public float bone_apply_strength = 1.0f;
    /// <summary>
    /// If <c>true</c>, the Transform3D applied by this Twisted_Bone3D node to the bone in the skeleton will be persistent.
    /// </summary>
    public bool bone_apply_persistent = true;
    /// <summary>
    /// If <c>true</c>, the Twisted_Bone3D node will follow the bone's Transform3D.
    /// (If applying and following, the Twisted_Bone3D node will follow AFTER applying its Transform3D to the bone)
    /// </summary>
    public bool bone_follow_transform = true;

    private bool did_apply_override = false;
    private bool force_bone_apply_transform = false;
    private bool cache_bone_apply_transform = true;
    private bool cache_bone_apply_persistent = true;

    private bool has_ready_been_called = false;
    private double internal_delta = 0.0f;


    private bool simulate_physics = false;
    private bool _internal_simulate_physics = false;
    /// <summary>
    /// A Vector3 representing the offset from the origin of the bone to the center of the RigidBody3D.
    /// Rigidbody nodes use the origin of the RigidBody3D as the center of mass, but the center of mass in a bone
    /// is likely not at the joint connection. This offset allows you to position the RigidBody3D center of mass correctly.
    /// </summary>
    public Vector3 origin_offset = Vector3.Zero;
    /// <summary>
    /// A Vector3 representing the scale that the Skeleton3D has. This allows the Twisted_PhysicsBone3D to be scaled and still
    /// work correctly with Godot's physics engine. If you're Skeleton3D has a scale of 4, you will need to put (4, 4, 4) for it
    /// to work correctly. This works around the Godot scaled RigidBody3D limitation.
    /// </summary>
    public Vector3 scale_offset = Vector3.One;


    public bool rotate_to_desired_node = false;
    public NodePath desired_node_path;
    public Node3D desired_node;
    public bool desired_node_follow_no_joints = false;


    public override void _Ready()
    {
        has_ready_been_called = true;

        twisted_skeleton3d = null;
        if (auto_get_twisted_skeleton == true) {
            twisted_skeleton3d = get_twisted_skeleton();
        } else {
            if (path_to_twisted_skeleton != null) {
                twisted_skeleton3d = GetNodeOrNull<Twisted_Skeleton3D>(path_to_twisted_skeleton);
            }
        }

        if (rotate_to_desired_node == true) {
            if (desired_node_path != null) {
                desired_node = GetNodeOrNull<Node3D>(desired_node_path);
            }
            AxisLockAngularX = false;
            AxisLockAngularY = false;
            AxisLockAngularZ = false;
            if (desired_node_follow_no_joints == true) {
                AxisLockLinearX = false;
                AxisLockLinearY = false;
                AxisLockLinearZ = false;
            }
        }

        if (auto_calcualte_bone_length == true) {
            auto_calculate_bone_length();
        }

        _internal_simulate_physics = false;

    }

    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == NotificationEnterTree) {
            has_ready_been_called = true;
            if (auto_get_twisted_skeleton == true) {
                twisted_skeleton3d = get_twisted_skeleton();
            }
            if (auto_calcualte_bone_length == true) {
                auto_calculate_bone_length();
            }
            // Connect the post-process signal
            GetTree().Connect("physics_frame", new Callable(this,"on_physics_frame"));
        }
        else if (what == NotificationExitTree) {
            // Disconnect the post-process signal
            GetTree().Disconnect("physics_frame", new Callable(this,"on_physics_frame"));
        }
    }

    // Only needed for following a Node3D-based node
    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        if (rotate_to_desired_node == true) {
            if (desired_node == null && desired_node_path != null) {
                desired_node = GetNodeOrNull<Node3D>(desired_node_path);
            }

            if (desired_node_follow_no_joints == true) {

                Transform3D bone_trans = make_transform_relative_to_skeleton(desired_node.GlobalTransform);
                Vector3 position_delta = (bone_trans.Origin - GlobalTransform.Origin);
                if (position_delta.LengthSquared() != 0) {
                    state.LinearVelocity = (position_delta / state.Step);
                } else {
                    state.LinearVelocity = Vector3.Zero;
                }
            }

            Quaternion rotation_quat = Twisted_3DFunctions.quat_from_two_vectors(-GlobalTransform.Basis.Z.Normalized(), -desired_node.GlobalTransform.Basis.Z.Normalized()).Normalized();
            if (rotation_quat.IsNormalized() == false) {
                return;
            }
            if (rotation_quat.W != 0) {
               state.AngularVelocity = (rotation_quat.GetEuler() / state.Step);
            } else {
                state.AngularVelocity = Vector3.Zero;
            }
        }
        else {
            state.AngularVelocity = AngularVelocity;
            state.LinearVelocity = LinearVelocity;
        }
        base._IntegrateForces(state);
    }

    public override bool _Set(StringName property, Variant value)
    {
        // ===========================================
        // ===== AUTOMATION
        if (property == "settings/auto_get_twisted_skeleton") {
            auto_get_twisted_skeleton = (bool)value;
            if (auto_get_twisted_skeleton == true) {
                twisted_skeleton3d = get_twisted_skeleton();
            } else {
                if (path_to_twisted_skeleton != null) {
                    twisted_skeleton3d = GetNodeOrNull<Twisted_Skeleton3D>(path_to_twisted_skeleton);
                }
            }
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "settings/path_to_twisted_skeleton") {
            path_to_twisted_skeleton = (NodePath)value;

            if (auto_get_twisted_skeleton == false) {
                if (path_to_twisted_skeleton != null) {
                    twisted_skeleton3d = GetNodeOrNull<Twisted_Skeleton3D>(path_to_twisted_skeleton);
                }
            }
            return true;
        }
        else if (property == "settings/auto_calcualte_bone_length") {
            auto_calcualte_bone_length = (bool)value;

            if (auto_calcualte_bone_length == true) {
                auto_calculate_bone_length();
            }
            return true;
        }
        // ===========================================
        // ===== BONE DATA
        if (property == "bone_data/bone_name") {
            bone_name = (string)value;
            if (twisted_skeleton3d != null) {
                if (twisted_skeleton3d.current_skeleton != null) {
                    bone_id = twisted_skeleton3d.current_skeleton.FindBone(bone_name);
                }
            }
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "bone_data/bone_id") {
            bone_id = (int)value;

            if (bone_id <= -1) {
                bone_id = -1;
                NotifyPropertyListChanged();
                return true;
            }

            if (twisted_skeleton3d != null) {
                if (twisted_skeleton3d.current_skeleton != null) {
                    if (bone_id < twisted_skeleton3d.current_skeleton.GetBoneCount()-1) {
                        bone_name = twisted_skeleton3d.current_skeleton.GetBoneName(bone_id);
                    }
                    else {
                        bone_id = twisted_skeleton3d.current_skeleton.GetBoneCount()-1;
                    }
                }
            }
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "bone_data/bone_length") {
            bone_length = (float)value;
            return true;
        }
        else if (property == "bone_data/bone_apply_transform") {
            bone_apply_transform = (bool)value;
            return true;
        }
        else if (property == "bone_data/bone_apply_strength") {
            bone_apply_strength = (float)value;
            return true;
        }
        else if (property == "bone_data/bone_apply_persistent") {
            bone_apply_persistent = (bool)value;
            return true;
        }
        else if (property == "bone_data/bone_follow_transform") {
            bone_follow_transform = (bool)value;
            return true;
        }
        // ===========================================

        // ===========================================
        // PHYSICS
        else if (property == "physics/origin_offset") {
            origin_offset = (Vector3)value;
            return true;
        }
        else if (property == "physics/scale_offset") {
            scale_offset = (Vector3)value;
            return true;
        }
        else if (property == "physics/simulate_physics") {
            simulate_physics = (bool)value;

            if (Engine.IsEditorHint() == false) {
                set_simulate_physics(simulate_physics);
            }
            
            return true;
        }

        else if (property == "physics/rotate_to_desired_node") {
            rotate_to_desired_node = (bool)value;
            return true;
        }
        else if (property == "physics/desired_node_path") {
            desired_node_path = (NodePath)value;

            if (desired_node_path != null) {
                desired_node = GetNodeOrNull<Node3D>(desired_node_path);
            }
            return true;
        }
        else if (property == "physics/desired_node_follow_no_joints") {
            desired_node_follow_no_joints = (bool)value;
            return true;
        }
        // ===========================================

        try {
            return base._Set(property, value);
        } catch {
            return false;
        }
    }

    public override Variant _Get(StringName property)
    {
        // ===========================================
        // ===== AUTOMATION
        if (property == "settings/auto_get_twisted_skeleton") {
            return auto_get_twisted_skeleton;
        }
        else if (property == "settings/path_to_twisted_skeleton") {
            return path_to_twisted_skeleton;
        }
        else if (property == "settings/auto_calcualte_bone_length") {
            return auto_calcualte_bone_length;
        }
        // ===========================================
        // ===== BONE DATA
        if (property == "bone_data/bone_name") {
            return bone_name;
        }
        else if (property == "bone_data/bone_id") {
            return bone_id;
        }
        else if (property == "bone_data/bone_length") {
            return bone_length;
        }
        else if (property == "bone_data/bone_apply_transform") {
            return bone_apply_transform;
        }
        else if (property == "bone_data/bone_apply_strength") {
            return bone_apply_strength;
        }
        else if (property == "bone_data/bone_apply_persistent") {
            return bone_apply_persistent;
        }
        else if (property == "bone_data/bone_follow_transform") {
            return bone_follow_transform;
        }
        // ===========================================

        // ===========================================
        // PHYSICS
        else if (property == "physics/origin_offset") {
            return origin_offset;
        }
        else if (property == "physics/scale_offset") {
            return scale_offset;
        }
        else if (property == "physics/simulate_physics") {
            return simulate_physics;
        }

        else if (property == "physics/rotate_to_desired_node") {
            return rotate_to_desired_node;
        }
        else if (property == "physics/desired_node_path") {
            return desired_node_path;
        }
        else if (property == "physics/desired_node_follow_no_joints") {
            return desired_node_follow_no_joints;
        }
        // ===========================================

        return base._Get(property);
    }

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> list = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        Godot.Collections.Dictionary tmp_dict;

        // ===========================================
        // ===== AUTOMATION
        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "settings/auto_get_twisted_skeleton");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        if (auto_get_twisted_skeleton == false) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "settings/path_to_twisted_skeleton");
            tmp_dict.Add("type", (int)Variant.Type.NodePath);
            tmp_dict.Add("hint", (int)PropertyHint.ResourceType);
            tmp_dict.Add("hint_string", "Skeleton3D");
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);
        }

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "settings/auto_calcualte_bone_length");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        // ===========================================
        // ===== BONE DATA
        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_name");
        tmp_dict.Add("type", (int)Variant.Type.String);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_id");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_length");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_apply_transform");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_apply_strength");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.Range);
        tmp_dict.Add("hint_string", "0,1,0.01");
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_apply_persistent");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_follow_transform");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        tmp_dict.Add("default", true);
        list.Add(tmp_dict);
        // ===========================================

        // ===========================================
        // PHYSICS
        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "physics/origin_offset");
        tmp_dict.Add("type", (int)Variant.Type.Vector3);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        tmp_dict.Add("default", true);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "physics/scale_offset");
        tmp_dict.Add("type", (int)Variant.Type.Vector3);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        tmp_dict.Add("default", true);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "physics/simulate_physics");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        tmp_dict.Add("default", true);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "physics/rotate_to_desired_node");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        tmp_dict.Add("default", true);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "physics/desired_node_path");
        tmp_dict.Add("type", (int)Variant.Type.NodePath);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "physics/desired_node_follow_no_joints");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        tmp_dict.Add("default", true);
        list.Add(tmp_dict);
        // ===========================================

        return list;
    }

    // ===================================
    // Physics related

    /// <summary>
    /// Internally used to set whether the Twisted_PhysicsBone3D is simulating physics using Godot's physics engine.
    /// This is what allows the RigidBody3D aspect of this node to turn on and off.
    /// </summary>
    /// <param name="physics_state"></param>
    public void set_simulate_physics(bool physics_state) {
        if (physics_state == true) {
            start_physics_simulation();
        } else {
            stop_physics_simulation();
        }
    }

    /// <summary>
    /// Starts the physics simulation on the RigidBody3D aspect of this node.
    /// This function should NOT be used directly. Instead, please use <c>set_simulate_physics(true)</c> instead!
    /// </summary>
    public void start_physics_simulation() {
        if (_internal_simulate_physics == true) {
            return;
        }

        if (bone_follow_transform) {
            retrieve_transform_from_bone();
        }

        // Let the RigidBody3D execute its force integration
        PhysicsServer3D.BodySetForceIntegrationCallback(GetRid(), new Callable(this, "_direct_state_changed"));

        // Set the physics mode to the mode this RigidBody3D uses
        if (this.Freeze == false) {
            PhysicsServer3D.BodySetMode(GetRid(), PhysicsServer3D.BodyMode.Rigid);
        } else {
            if (this.FreezeMode == FreezeModeEnum.Kinematic) {
                PhysicsServer3D.BodySetMode(GetRid(), PhysicsServer3D.BodyMode.Kinematic);
            } else if (this.FreezeMode == FreezeModeEnum.Static) {
                PhysicsServer3D.BodySetMode(GetRid(), PhysicsServer3D.BodyMode.Static);
            }
        }
        
        // Apply collision layer(s) and mask(s)
        PhysicsServer3D.BodySetCollisionLayer(GetRid(), CollisionLayer);
        PhysicsServer3D.BodySetCollisionMask(GetRid(), CollisionMask);
        
        _internal_simulate_physics = true;
    }

    /// <summary>
    /// Stops the physics simulation on the RigidBody3D aspect of this node.
    /// This function should NOT be used directly. Instead, please use <c>set_simulate_physics(false)</c> instead!
    /// </summary>
    public void stop_physics_simulation() {
        if (_internal_simulate_physics == false) {
            return;
        }

        // Stop the RigidBody3D from executing its force integration
        PhysicsServer3D.BodySetForceIntegrationCallback(GetRid(), new Callable(this, ""));
        // Set the RigidBody3D mode to static, to avoid physics calculations internally
        PhysicsServer3D.BodySetMode(GetRid(), PhysicsServer3D.BodyMode.Static);

        // Remove the collision layer(s) and mask(s)
        PhysicsServer3D.BodySetCollisionLayer(GetRid(), 0);
        PhysicsServer3D.BodySetCollisionMask(GetRid(), 0);

        if (bone_follow_transform) {
            retrieve_transform_from_bone();
        }

        _internal_simulate_physics = false;
    }

    /// <summary>
    /// This is a fun little function that rotates a RigidBody3D-based node to look at a target position using Quaternion.
    /// It does this by setting the angular velocity to the difference in rotation multiplied by the <c>look_at_strength</c>.
    /// This function is the basis of how the physics bone motor currently works (as of when this was written).
    /// 
    /// If you are looking to control a set of bones, please use the <c>Twisted_PhysicsBoneMotor3D</c> node, as otherwise your
    /// bone chain will have wiggly movement issues!
    /// </summary>
    /// <param name="target_position">The target position, in global space, that you want to rotate the RigidBody3D to look at</param>
    /// <param name="look_at_strength">The amount of force you want to apply to the rotation</param>
    public void look_at_target(Vector3 target_position, float look_at_strength) {
        Vector3 self_to_target = GlobalTransform.Origin.DirectionTo(target_position);
        float self_to_target_length = GlobalTransform.Origin.DistanceTo(target_position);
        
        if (self_to_target.LengthSquared() == 0) {
            return;
        }

        Quaternion rotation_quat = Twisted_3DFunctions.quat_from_two_vectors(-GlobalTransform.Basis.Z.Normalized(), self_to_target).Normalized();
        if (rotation_quat.IsNormalized() == false) {
            return;
        }

        AngularVelocity = (rotation_quat.GetEuler() * self_to_target_length) * look_at_strength;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsInsideTree() == false) {
            return;
        }
        if (bone_id <= -1) {
            return;
        }
        internal_delta = delta;
    }
    
    /// <summary>
    /// Called at the end of each <c>_physics_process</c> call. Used to apply post-processing.
    /// </summary>
    public void on_physics_frame() {
        if (IsInsideTree() == false || bone_id == -1) {
            return;
        }
        _On_Post_Process(internal_delta);
    }

    /// <summary>
    /// Called after <c>_physics_process</c> has been called on the node. This function is directly responsible for
    /// applying the node's Transform3D to the bone.
    /// 
    /// This is also where the bone's Transform3D is applied to this node, if this bone is following the
    /// transform of the bone it's attached to.
    /// </summary>
    /// <param name="delta">The amount of time that has elapsed</param>
    public virtual void _On_Post_Process(double delta) {
        if (force_bone_apply_transform == true) {
            cache_bone_apply_transform = bone_apply_transform;
            cache_bone_apply_persistent = bone_apply_persistent;

            bone_apply_transform = true;
            bone_apply_persistent = true;
        }
        
        if (twisted_skeleton3d != null && twisted_skeleton3d.current_skeleton != null) {
            if (bone_apply_transform == true) {
                apply_transform_to_bone();
            }
            else if (did_apply_override == true) {
                twisted_skeleton3d.current_skeleton.SetBoneGlobalPoseOverride(bone_id, Transform3D.Identity, 0.0f, false);
                did_apply_override = false;
            }

            if (simulate_physics == false) {
                if (bone_follow_transform == true) {
                    retrieve_transform_from_bone();
                }
            }
        }

        if (force_bone_apply_transform == true) {
            bone_apply_transform = cache_bone_apply_transform;
            bone_apply_persistent = cache_bone_apply_persistent;
            force_bone_apply_transform = false;
        }
    }
    // ===================================

    /// <summary>
    /// If called, the node will apply its Transform3D to the bone regardless of the properties set.
    /// (Used in IK to apply the IK results regardless of the individual Twisted_Bone3D setting)
    /// </summary>
    public void force_apply_transform() {
        force_bone_apply_transform = true;
    }

    /// <summary>
    /// Takes the Transform3D of the Twisted_Bone3D node and applies it to the bone in the Skeleton3D node.
    /// Note: This function does NOT check to see if the node has a valid bone ID.
    /// </summary>
    public void apply_transform_to_bone() {
        if (twisted_skeleton3d == null) {
            return;
        }
        if (twisted_skeleton3d.current_skeleton == null) {
            return;
        }

        Transform3D transform_to_apply = GlobalTransform;

        // Adjust for the offset
        transform_to_apply.Origin -= transform_to_apply.Basis * origin_offset;

        // Account for different bone forward axes
        transform_to_apply.Basis = twisted_skeleton3d.negative_z_forward_to_bone_forward(transform_to_apply.Basis);
        // Convert to a global pose
        transform_to_apply = twisted_skeleton3d.world_transform_to_global_pose(transform_to_apply);

        // Adjust for the scale offset
        if (scale_offset.LengthSquared() != 0) {
            transform_to_apply.Basis = transform_to_apply.Basis.Scaled(scale_offset);
        }

        // Apply to the skeleton
        twisted_skeleton3d.current_skeleton.SetBoneGlobalPoseOverride(
            bone_id, transform_to_apply, bone_apply_strength, bone_apply_persistent);
        
        // Note that we applied an overide (so we can remove it if we decide not to override anymore).
        did_apply_override = true;
    }

    /// <summary>
    /// Gets the Transform3D of the bone and applies it to the Twisted_Bone3D node.
    /// Note: This function does NOT check to see if the node has a valid bone ID.
    /// </summary>
    public void retrieve_transform_from_bone() {
        if (twisted_skeleton3d == null) {
            return;
        }
        if (twisted_skeleton3d.current_skeleton == null) {
            return;
        }

        Transform3D bone_transform = twisted_skeleton3d.current_skeleton.GetBoneGlobalPose(bone_id);
        
        // Adjust for scale offset
        if (scale_offset.LengthSquared() != 0) {
            bone_transform.Basis = bone_transform.Basis.Scaled(scale_offset.Inverse());
        }

        // Convert to a world transform
        bone_transform = twisted_skeleton3d.global_pose_to_world_transform(bone_transform);
        
        // Convert from bone-forward to -Z forward
        bone_transform.Basis = twisted_skeleton3d.bone_forward_to_negative_z_forward(bone_transform.Basis);

        // Adjust for the offset
        bone_transform.Origin += bone_transform.Basis * origin_offset;

        // Apply to self
        GlobalTransform = bone_transform;

        // Adjust the Rigidbody forces (since we scaled things)
        LinearVelocity = LinearVelocity * scale_offset.Inverse();
        AngularVelocity = AngularVelocity * scale_offset.Inverse();
    }

    public Transform3D make_transform_relative_to_skeleton(Transform3D input_trans) {
        Transform3D return_trans = twisted_skeleton3d.world_transform_to_global_pose(input_trans);
        if (scale_offset.LengthSquared() != 0) {
            return_trans.Basis = return_trans.Basis.Scaled(scale_offset.Inverse());
        }
        return_trans = twisted_skeleton3d.global_pose_to_world_transform(return_trans);
        return_trans.Origin += return_trans.Basis * origin_offset;
        return return_trans;
    }

    /// <summary>
    /// Returns the Twisted_Skeleton3D node that this Twisted_Bone3D node uses.
    /// </summary>
    /// <returns>The Twisted_Skeleton3D node that this bone uses, or <c>null</c> if one cannot be found</returns>
    public Twisted_Skeleton3D get_twisted_skeleton() {
        if (has_ready_been_called == false) {
            return null;
        }

        if (twisted_skeleton3d == null) {
            Node parent = GetParent();
            if (parent is Twisted_Skeleton3D) {
                twisted_skeleton3d = parent as Twisted_Skeleton3D;
            } else if (parent is Twisted_Bone3D) {
                twisted_skeleton3d = (parent as Twisted_Bone3D).get_twisted_skeleton();
            } else if (parent is Twisted_PhysicsBone3D) {
                twisted_skeleton3d = (parent as Twisted_PhysicsBone3D).get_twisted_skeleton();
            } else {
                GD.PrintErr("ERROR - Twisted_Bone3D - Cannot find Twisted_Skeleton3D or parent Twisted_Bone3D");
                return null;
            }
        }
        return twisted_skeleton3d;
    }


    /// <summary>
    /// Attempts to calculate the length of the bone using children Twisted_Bone3D or Twisted_PhysicsBone3D nodes.
    /// If the child nodes are setup correctly, this function is fairly accurate.
    /// </summary>
    public void auto_calculate_bone_length() {
        if (has_ready_been_called == false) {
            return;
        }

        bool length_set = false;
        foreach (Node child in GetChildren()) {
            if (child is Twisted_Bone3D) {
                if (child.IsInsideTree() == false) {
                    return;
                }
                Twisted_Bone3D child_bone = (Twisted_Bone3D)child;
                bone_length = GlobalTransform.Origin.DistanceTo(child_bone.GlobalTransform.Origin);
                length_set = true;
                break;
            }
            else if (child is Twisted_PhysicsBone3D) {
                if (child.IsInsideTree() == false) {
                    return;
                }
                Twisted_PhysicsBone3D child_bone = (Twisted_PhysicsBone3D)child;
                bone_length = GlobalTransform.Origin.DistanceTo(child_bone.GlobalTransform.Origin);
                length_set = true;
                break;
            }
        }
        if (length_set == false) {
            // TODO: find a way to show it was not set...
            bone_length = -1.0f;
        }
    }
}