using Godot;
using System;

/// <summary>
/// A node that represents a Bone in a Skeleton3D using a 3D node.
/// This node is the backbone of all 3D IK and bone manipulation processes.
/// </summary>
[Tool]
public partial class Twisted_Bone3D : Node3D
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

        if (auto_calcualte_bone_length == true) {
            auto_calculate_bone_length();
        }
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
            GetTree().Connect("process_frame",new Callable(this,"on_idle_frame"));
        }
        else if (what == NotificationExitTree) {
            // Disconnect the post-process signal
            GetTree().Disconnect("process_frame",new Callable(this,"on_idle_frame"));
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        // ===========================================
        // ===== AUTOMATION
        if (property == "settings/auto_get_twisted_skeleton") {
            auto_get_twisted_skeleton = (bool)value;

            if (auto_get_twisted_skeleton == true) {
                twisted_skeleton3d = get_twisted_skeleton();
            }
            
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "settings/path_to_twisted_skeleton") {
            path_to_twisted_skeleton = (NodePath)value;
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
        
        try {
            return base._Get(property);
        } catch {
            return false;
        }
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
        tmp_dict.Add("hint", (int)PropertyHint.Range);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_follow_transform");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.Range);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        tmp_dict.Add("default", true);
        list.Add(tmp_dict);
        // ===========================================

        return list;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsInsideTree() == false) {
            return;
        }
        internal_delta = delta;
    }

    /// <summary>
    /// Called at the end of each <c>_process</c> call. Used to apply post-processing.
    /// </summary>
    public void on_idle_frame()
    {
        if (IsInsideTree() == false || bone_id == -1) {
            return;
        }
        _On_Post_Process(internal_delta);
    }

    /// <summary>
    /// Called after <c>_process</c> has been called on the node. This function is directly responsible for
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
                twisted_skeleton3d.current_skeleton.SetBoneGlobalPoseOverride(bone_id, new Transform3D(), 0.0f, false);
                did_apply_override = false;
            }

            if (bone_follow_transform == true) {
                retrieve_transform_from_bone();
            }
        }

        if (force_bone_apply_transform == true) {
            bone_apply_transform = cache_bone_apply_transform;
            bone_apply_persistent = cache_bone_apply_persistent;
            force_bone_apply_transform = false;
        }
    }

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
        // Account for different bone forward axes
        transform_to_apply.Basis = twisted_skeleton3d.negative_z_forward_to_bone_forward(transform_to_apply.Basis);
        // Convert to a global pose
        transform_to_apply = twisted_skeleton3d.world_transform_to_global_pose(transform_to_apply);
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
        GlobalTransform = get_bone_global_pose(true);
    }

    /// <summary>
    /// Gets the global pose without overrides Transform3D of the bone and returns it.
    /// Note: If there is no skeleton or TwistedSkeleton3D, it will just return it's own transform/global-transform.
    /// </summary>
    public Transform3D get_reset_bone_global_pose(bool convert_to_world_space=true) {
        if (twisted_skeleton3d.current_skeleton == null || twisted_skeleton3d == null) {
            if (convert_to_world_space == true) {
                return GlobalTransform;
            }
            return Transform;
        }

        Transform3D bone_transform = twisted_skeleton3d.current_skeleton.GetBoneGlobalPoseNoOverride(bone_id);
        if (convert_to_world_space == true) {
            bone_transform = twisted_skeleton3d.global_pose_to_world_transform(bone_transform);
            bone_transform.Basis = twisted_skeleton3d.bone_forward_to_negative_z_forward(bone_transform.Basis);
        }
        return bone_transform;
    }

    /// <summary>
    /// Gets the global pose transform of the bone linked to this Twisted_Bone3D and returns it.
    /// Optionally converts the transform to world-space from global-bone space
    /// Note: If there is no skeleton or TwistedSkeleton3D, it will just return it's own transform/global-transform.
    /// </summary>
    /// <param name="convert_to_world_space">If true, will convert the transform from global-pose space to world-space</param>
    /// <returns>The global pose transform</returns>
    public Transform3D get_bone_global_pose(bool convert_to_world_space=true) {
        if (twisted_skeleton3d.current_skeleton == null || twisted_skeleton3d == null) {
            if (convert_to_world_space == true) {
                return GlobalTransform;
            }
            return Transform;
        }

        Transform3D bone_transform = twisted_skeleton3d.current_skeleton.GetBoneGlobalPose(bone_id);
        if (convert_to_world_space == true) {
            bone_transform = twisted_skeleton3d.global_pose_to_world_transform(bone_transform);
            // Convert from bone-forward to -Z forward
            bone_transform.Basis = twisted_skeleton3d.bone_forward_to_negative_z_forward(bone_transform.Basis);
        }
        return bone_transform;
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
        if (IsInsideTree() == false) {
            return;
        }

        bool length_set = false;
        foreach (Node child in GetChildren()) {
            if (child is Twisted_Bone3D || child is Twisted_PhysicsBone3D) {
                if (child.IsInsideTree() == false) {
                    continue;
                }
                Node3D child_bone = (Node3D)child;
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

    /// <summary>
    /// Returns the bone length, but relative to the global pose transform rather than relative to the world transform.
    /// Use this function for bone length if you are operating in the global pose space and need the bone's length instead
    /// of directly using the bone_length variable!
    /// NOTE: will return the bone_length if there is no skeleton or TwistedSkeleton3D set.
    /// </summary>
    /// <param name="force_recalculate"></param>
    /// <returns></returns>
    public float get_global_pose_length() {
        if (twisted_skeleton3d.current_skeleton == null || twisted_skeleton3d == null) {
            return bone_length;
        }

        // Use the bone forward direction for scale. Assume X as forward by default/auto.
        TWISTED_BONE_FORWARD_DIRECTION forward = twisted_skeleton3d.get_bone_forward_direction();
        if (forward == TWISTED_BONE_FORWARD_DIRECTION.Y_FORWARD || forward == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Y_FORWARD) {
            return bone_length * (1 / twisted_skeleton3d.GlobalTransform.Basis.Scale.Y);
        }
        else if (forward == TWISTED_BONE_FORWARD_DIRECTION.Z_FORWARD || forward == TWISTED_BONE_FORWARD_DIRECTION.NEGATIVE_Z_FORWARD)
        {
            return bone_length * (1 / twisted_skeleton3d.GlobalTransform.Basis.Scale.Z);
        }
        else {
            return bone_length * (1 / twisted_skeleton3d.GlobalTransform.Basis.Scale.X);
        }
    }

}