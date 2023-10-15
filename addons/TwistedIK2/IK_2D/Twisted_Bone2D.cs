using Godot;
using System;

[Tool]
public partial class Twisted_Bone2D : Bone2D
{

    /// <summary>
    /// If <c>true</c>, the Twisted_Bone2D node will attempt to automatically find and store a parent Twisted_Skeleton2D node.
    /// </summary>
    public bool auto_get_skeleton = true;
    /// <summary>
    /// If <c>true</c>, the Twisted_Bone2D node will attempt to automatically calculate its length using child nodes.
    /// </summary>
    public bool auto_calcualte_bone_length = true;

    public bool auto_calcualte_bone_angle = true;

    /// <summary>
    /// The NodePath to the Skeleton2D that this Twisted_Bone2D uses. This property is only used if <c>auto_get_skeleton</c> is <c>false</c>.
    /// </summary>
    public NodePath path_to_skeleton;
    /// <summary>
    /// A reference to the Skeleton2D node that this Twisted_Bone2D uses.
    /// (Will be <c>null</c> if the Skeleton2D cannot be found or is not set)
    /// </summary>
    public Skeleton2D skeleton2d;

    /// <summary>
    /// The length of the bone, from its origin to its tip/end.
    /// </summary>
    public float bone_length;

    /// <summary>
    /// The angle offset from the Bone2D node to the actual bone, as the Bone2D angle is partially based on position of child bones.
    /// This is needed to accurately point the bones in the correct direction in IK solvers.
    /// (or rather, so the end result can be properly offset)
    /// </summary>
    public float bone_angle = 0;

    private Transform2D cache_bone_transform = new Transform2D();
    private bool is_executing_ik = false;

    private bool has_ready_been_called = false;
    private double internal_delta = 0.0f;

    // Gizmo variables (for drawing the gizmos)
    public bool gizmo_can_draw_in_editor = true;
    public bool gizmo_can_draw_in_game = false;
    private Rid gizmo_canvas_rid;
    private bool gizmo_canvas_rid_valid = false;
    public Color gizmo_bone_normal_color = new Color(0.6f, 1.0f, 1.0f, 0.5f);
    public Color gizmo_bone_ik_color = new Color(1.0f, 1.0f, 0.6f, 0.5f);
    public int gizmo_bone_shape_index = 2;

    public override void _Ready()
    {
        has_ready_been_called = true;
        
        skeleton2d = null;
        if (auto_get_skeleton == true) {
            skeleton2d = get_skeleton();
        } else {
            if (path_to_skeleton != null) {
                skeleton2d = GetNodeOrNull<Skeleton2D>(path_to_skeleton);
            }
        }

        if (auto_calcualte_bone_length == true) {
            auto_calculate_bone_length();
        }
        if (auto_calcualte_bone_angle == true) {
            auto_calculate_bone_angle();
        }
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationEnterTree) {
            has_ready_been_called = true;
            if (auto_get_skeleton == true) {
                skeleton2d = get_skeleton();
            }
            if (auto_calcualte_bone_length == true) {
                auto_calculate_bone_length();
            }
            if (auto_calcualte_bone_angle == true) {
                auto_calculate_bone_angle();
            }

            cache_bone_transform = Transform;
            update_gizmo();
        }
        else if (what == NotificationExitTree) {
            if (gizmo_canvas_rid_valid == true) {
                RenderingServer.FreeRid(gizmo_canvas_rid);
                gizmo_canvas_rid_valid = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsInsideTree() == false) {
            return;
        }

        if (is_executing_ik == true) {
            is_executing_ik = false;
            Transform = cache_bone_transform;
            update_gizmo();
        } else {
            cache_bone_transform = Transform;
        }

        internal_delta = delta;
    }

    /// <summary>
    /// Tells the Bone2D that it is being operated on by IK. This prevents it from caching the transform,
    /// which makes it possible for the Bone2D to reset itself back to the last non-IK position when
    /// IK is no longer running.
    /// </summary>
    /// <param name="status">Whether IK is running or not. Defaults to <c>true</c></param>
    public void set_executing_ik(bool status=true) {
        is_executing_ik = status;
        update_gizmo();
    }

    public override bool _Set(StringName property, Variant value)
    {
        // ===========================================
        // ===== AUTOMATION
        if (property == "settings/auto_get_skeleton") {
            auto_get_skeleton = (bool)value;

            if (auto_get_skeleton == true) {
                skeleton2d = get_skeleton();
            }
            
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "settings/path_to_skeleton") {
            path_to_skeleton = (NodePath)value;
            return true;
        }
        else if (property == "settings/auto_calcualte_bone_length") {
            auto_calcualte_bone_length = (bool)value;

            if (auto_calcualte_bone_length == true) {
                auto_calculate_bone_length();
                update_gizmo();
            }
            return true;
        }
        else if (property == "settings/auto_calcualte_bone_angle") {
            auto_calcualte_bone_angle = (bool)value;

            if (auto_calcualte_bone_angle == true) {
                auto_calculate_bone_angle();
                update_gizmo();
            }
            return true;
        }

        // ===========================================
        // ===== Bone Data
        else if (property == "bone_data/bone_length") {
            bone_length = (float)value;
            update_gizmo();
            return true;
        }
        else if (property == "bone_data/bone_angle") {
            bone_angle = Mathf.DegToRad((float)value);
            update_gizmo();
            return true;
        }
        // ===========================================

        // ===========================================
        // ===== Gizmo Data
        else if (property == "gizmo/gizmo_can_draw_in_editor") {
            if (gizmo_canvas_rid_valid == true) {
                RenderingServer.FreeRid(gizmo_canvas_rid);
                gizmo_canvas_rid_valid = false;
            }
            gizmo_can_draw_in_editor = (bool)value;
            update_gizmo();
            return true;
        }
        else if (property == "gizmo/gizmo_can_draw_in_game") {
            if (gizmo_canvas_rid_valid == true) {
                RenderingServer.FreeRid(gizmo_canvas_rid);
                gizmo_canvas_rid_valid = false;
            }
            gizmo_can_draw_in_game = (bool)value;
            update_gizmo();
            return true;
        }
        else if (property == "gizmo/gizmo_bone_shape_index") {
            gizmo_bone_shape_index = (int)value;
            update_gizmo();
            return true;
        }
        else if (property == "gizmo/gizmo_bone_normal_color") {
            gizmo_bone_normal_color = (Color)value;
            update_gizmo();
            return true;
        }
        else if (property == "gizmo/gizmo_bone_ik_color") {
            gizmo_bone_ik_color = (Color)value;
            update_gizmo();
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
        if (property == "settings/auto_get_skeleton") {
            return auto_get_skeleton;
        }
        else if (property == "settings/path_to_skeleton") {
            return path_to_skeleton;
        }
        else if (property == "settings/auto_calcualte_bone_length") {
            return auto_calcualte_bone_length;
        }
        else if (property == "settings/auto_calcualte_bone_angle") {
            return auto_calcualte_bone_angle;
        }
        // ===========================================
        // ===== Bone Data
        else if (property == "bone_data/bone_length") {
            return bone_length;
        }
        else if (property == "bone_data/bone_angle") {
            return Mathf.RadToDeg(bone_angle);
        }
        // ===========================================
        // ===== Gizmo Data
        else if (property == "gizmo/gizmo_can_draw_in_editor") {
            return gizmo_can_draw_in_editor;
        }
        else if (property == "gizmo/gizmo_can_draw_in_game") {
            return gizmo_can_draw_in_game;
        }
        else if (property == "gizmo/gizmo_bone_shape_index") {
            return gizmo_bone_shape_index;
        }
        else if (property == "gizmo/gizmo_bone_normal_color") {
            return gizmo_bone_normal_color;
        }
        else if (property == "gizmo/gizmo_bone_ik_color") {
            return gizmo_bone_ik_color;
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
        tmp_dict.Add("name", "settings/auto_get_skeleton");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        if (auto_get_skeleton == false) {
            tmp_dict = new Godot.Collections.Dictionary();
            tmp_dict.Add("name", "settings/path_to_skeleton");
            tmp_dict.Add("type", (int)Variant.Type.NodePath);
            tmp_dict.Add("hint", (int)PropertyHint.ResourceType);
            tmp_dict.Add("hint_string", "Skeleton2D");
            tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
            list.Add(tmp_dict);
        }

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "settings/auto_calcualte_bone_length");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "settings/auto_calcualte_bone_angle");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        // ===========================================
        // ===== BONE DATA
        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_length");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "bone_data/bone_angle");
        tmp_dict.Add("type", (int)Variant.Type.Float);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);
        // ===========================================

        // ===== Gizmo Data
        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "gizmo/gizmo_can_draw_in_editor");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "gizmo/gizmo_can_draw_in_game");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "gizmo/gizmo_bone_shape_index");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.Enum);
        tmp_dict.Add("hint_string", "Simple Rectangle, Hollow Bone, Bone");
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "gizmo/gizmo_bone_normal_color");
        tmp_dict.Add("type", (int)Variant.Type.Color);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "gizmo/gizmo_bone_ik_color");
        tmp_dict.Add("type", (int)Variant.Type.Color);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        // ===========================================

        return list;
    }

    /// <summary>
    /// Returns the Skeleton2D node that this Twisted_Bone2D node uses.
    /// </summary>
    /// <returns>The Skeleton2D node that this bone uses, or <c>null</c> if one cannot be found</returns>
    public Skeleton2D get_skeleton() {
        if (has_ready_been_called == false) {
            return null;
        }

        if (skeleton2d == null) {
            Node parent = GetParent();
            if (parent is Skeleton2D) {
                skeleton2d = parent as Skeleton2D;
            } else if (parent is Twisted_Bone2D) {
                skeleton2d = (parent as Twisted_Bone2D).get_skeleton();
            } else {
                GD.PrintErr("ERROR - Twisted_Bone2D - Cannot find Twisted_Skeleton3D or parent Twisted_Bone3D");
                return null;
            }
        }
        return skeleton2d;
    }

    /// <summary>
    /// Attempts to calculate the length of the bone using children Twisted_Bone2D node.
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
            if (child is Twisted_Bone2D) {
                if (child.IsInsideTree() == false) {
                    return;
                }
                Twisted_Bone2D child_bone = (Twisted_Bone2D)child;
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
    /// Attempts to calculate the angle of the Bone2D node using children Twisted_Bone2D node.
    /// If the child nodes are setup correctly, this function is fairly accurate.
    /// This is needed for accurately pointing bones at target positions.
    /// </summary>
    public void auto_calculate_bone_angle() {
        if (has_ready_been_called == false) {
            return;
        }
        if (IsInsideTree() == false) {
            return;
        }

        bool angle_set = false;
        foreach (Node child in GetChildren()) {
            if (child is Twisted_Bone2D) {
                if (child.IsInsideTree() == false) {
                    return;
                }
                Twisted_Bone2D child_bone = (Twisted_Bone2D)child;

                Vector2 child_relative_pos = child_bone.Transform.Origin.Normalized();
                bone_angle = Mathf.Atan2(child_relative_pos.Y, child_relative_pos.X);
                angle_set = true;
                break;
            }
        }
        if (angle_set == false) {
            // TODO: find a way to show it was not set...
            bone_angle = 0f;
        }
    }

    /// <summary>
    /// Will redraw the gizmo if the settings are correct. Use this instead of the stock <c>update</c> function if you want
    /// the Bone2D to draw using the built-in modifier properties.
    /// </summary>
    public void update_gizmo()
    {
        if (Engine.IsEditorHint() == true) {
            if (gizmo_can_draw_in_editor == true) {
                _Draw();
            }
        }
        else {
            if (gizmo_can_draw_in_game == true) {
                _Draw();
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

        if (gizmo_canvas_rid_valid == false) {
            gizmo_canvas_rid = RenderingServer.CanvasItemCreate();
            RenderingServer.CanvasItemSetParent(gizmo_canvas_rid, GetCanvasItem());
            RenderingServer.CanvasItemSetZAsRelativeToParent(gizmo_canvas_rid, true);
            RenderingServer.CanvasItemSetZIndex(gizmo_canvas_rid, 100);
            gizmo_canvas_rid_valid = true;
        }

        // Draw the bones
        if (gizmo_canvas_rid_valid == true) {
            Vector2 bone_direction = new Vector2(Mathf.Cos(bone_angle), Mathf.Sin(bone_angle));
            Color color_to_use = gizmo_bone_normal_color;
            RenderingServer.CanvasItemClear(gizmo_canvas_rid);

            if (is_executing_ik == true) {
                color_to_use = gizmo_bone_ik_color;
            }

            // Simple rectangles
            if (gizmo_bone_shape_index == 0) {
                RenderingServer.CanvasItemAddLine(gizmo_canvas_rid,
                    Vector2.Zero, bone_direction * bone_length,
                    color_to_use, 6.0f, true);
            }
            // Hollow bones
            else if (gizmo_bone_shape_index == 1) {
                Vector2[] shape_points = new Vector2[4];
                Vector2 bone_direction_perp = new Vector2(bone_direction.Y, -bone_direction.X);
                shape_points[0] = Vector2.Zero;
                shape_points[1] = (bone_direction * bone_length * 0.2f) + (bone_direction_perp * bone_length * 0.1f);
                shape_points[2] = (bone_direction * bone_length);
                shape_points[3] = (bone_direction * bone_length * 0.2f) - (bone_direction_perp * bone_length * 0.1f);

                RenderingServer.CanvasItemAddLine(gizmo_canvas_rid, shape_points[0], shape_points[1], color_to_use, 3.0f, true);
                RenderingServer.CanvasItemAddLine(gizmo_canvas_rid, shape_points[1], shape_points[2], color_to_use, 3.0f, true);
                RenderingServer.CanvasItemAddLine(gizmo_canvas_rid, shape_points[0], shape_points[3], color_to_use, 3.0f, true);
                RenderingServer.CanvasItemAddLine(gizmo_canvas_rid, shape_points[3], shape_points[2], color_to_use, 3.0f, true);
            }
            else if (gizmo_bone_shape_index == 2) {
                Vector2[] shape_points = new Vector2[4];
                Vector2 bone_direction_perp = new Vector2(bone_direction.Y, -bone_direction.X);
                shape_points[0] = Vector2.Zero;
                shape_points[1] = (bone_direction * bone_length * 0.2f) + (bone_direction_perp * bone_length * 0.1f);
                shape_points[2] = (bone_direction * bone_length);
                shape_points[3] = (bone_direction * bone_length * 0.2f) - (bone_direction_perp * bone_length * 0.1f);

                Color[] shape_colors = new Color[4];
                shape_colors[0] = color_to_use;
                shape_colors[1] = color_to_use;
                shape_colors[2] = color_to_use;
                shape_colors[3] = color_to_use;

                RenderingServer.CanvasItemAddPolygon(gizmo_canvas_rid, shape_points, shape_colors,
                    null, RenderingServer.GetWhiteTexture());
            }
        }
    }

}