using Godot;
using System;

/// <summary>
/// The possible execution that Twisted_Modifier2D nodes can use.
/// </summary>
public enum TWISTED_IK_MODIFIER_MODES_2D {
    _PROCESS,
    _PHYSICS_PROCESS,
}

/// <summary>
/// The base class for all IK operations. This provides a constant, streamlined interface that allows
/// for mixing and matching IK and other Twisted_Bone2D-based operations.
/// </summary>
[Tool]
public partial class Twisted_Modifier2D : Node2D
{
    
    /// <summary>
    /// If <c>true</c>, this modifier will execute when the Twisted_ModifierStack2D executes.
    /// </summary>
    public bool execution_enabled = true;

    /// <summary>
    /// The execution mode this modifier uses. Currently, this is mostly to give some modifiers optional physics features.
    /// </summary>
    public TWISTED_IK_MODIFIER_MODES_2D execution_mode;


    /// <summary>
    /// When <c>true</c>, the gizmo(s) will be drawn in the editor if the modifier has gizmo code.
    /// </summary>
    public bool gizmo_can_draw_in_editor = true;
    /// <summary>
    /// When <c>true</c>, the gizmo(s) will be drawn while the game is running if the modifier has gizmo code.
    /// </summary>
    public bool gizmo_can_draw_in_game = false;
    /// <summary>
    /// When <c>true</c>, the gizmo will draw every time its executed, which can be helpful for dialing in modifier settings.
    /// </summary>
    public bool gizmo_update_on_execution = false;

    public override void _Ready()
    {
        if (IsInsideTree() == true) {
            if (GetParentOrNull<Twisted_ModifierStack2D>() != null) {
                GetParentOrNull<Twisted_ModifierStack2D>().update_modification_children_array();
            }
        }
    }

    /// <summary>
    /// This is where the modifier is actually executed! Any code in this function will be called when the
    /// Twisted_ModifierStack2D is executing.
    /// 
    /// This function is a virtual stub and is intended to be overriden with Twisted_Bone2D functionality!
    /// </summary>
    /// <param name="modifier_stack"></param>
    /// <param name="delta"></param>
    public virtual void _ExecuteModification(Twisted_ModifierStack2D modifier_stack, double delta) {
        if (gizmo_update_on_execution == true) {
            update_gizmo();
        }

        return;
    }

    public override bool _Set(StringName property, Variant value)
    {
        if (property == "base_settings/execution_enabled") {
            execution_enabled = (bool)value;
            return true;
        }
        else if (property == "base_settings/execution_mode") {
            execution_mode = (TWISTED_IK_MODIFIER_MODES_2D)(int)value;
            NotifyPropertyListChanged();
            return true;
        }
        else if (property == "base_settings/gizmo_can_draw_in_editor") {
            gizmo_can_draw_in_editor = (bool)value;
            update_gizmo();
            return true;
        }
        else if (property == "base_settings/gizmo_can_draw_in_game") {
            gizmo_can_draw_in_game = (bool)value;
            update_gizmo();
            return true;
        }
        else if (property == "base_settings/gizmo_update_on_execution") {
            gizmo_update_on_execution = (bool)value;
            update_gizmo();
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
        if (property == "base_settings/execution_enabled") {
            return execution_enabled;
        }
        else if (property == "base_settings/execution_mode") {
            return (int)execution_mode;
        }
        else if (property == "base_settings/gizmo_can_draw_in_editor") {
            return gizmo_can_draw_in_editor;
        }
        else if (property == "base_settings/gizmo_can_draw_in_game") {
            return gizmo_can_draw_in_game;
        }
        else if (property == "base_settings/gizmo_update_on_execution") {
            return gizmo_update_on_execution;
        }

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

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "base_settings/execution_enabled");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "base_settings/execution_mode");
        tmp_dict.Add("type", (int)Variant.Type.Int);
        tmp_dict.Add("hint", (int)PropertyHint.Enum);
        tmp_dict.Add("hint_string", "_Process, _Physics_Process");
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "base_settings/gizmo_can_draw_in_editor");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "base_settings/gizmo_can_draw_in_game");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        tmp_dict = new Godot.Collections.Dictionary();
        tmp_dict.Add("name", "base_settings/gizmo_update_on_execution");
        tmp_dict.Add("type", (int)Variant.Type.Bool);
        tmp_dict.Add("hint", (int)PropertyHint.None);
        tmp_dict.Add("usage", (int)PropertyUsageFlags.Default);
        list.Add(tmp_dict);

        return list;
    }



    /// <summary>
    /// A helper for drawing angle constraints easily.
    /// </summary>
    /// <param name="twisted_bone"></param>
    /// <param name="constraint_angle_min"></param>
    /// <param name="constraint_angle_max"></param>
    /// <param name="constraint_angle_invert"></param>
    /// <param name="constraint_angle_in_localspace"></param>
    /// <param name="constraint_angle_enabled"></param>
    public void draw_angle_constraints(Twisted_Bone2D twisted_bone, float constraint_angle_min, float constraint_angle_max,
        bool constraint_angle_invert, bool constraint_angle_in_localspace, bool constraint_angle_enabled)
    {
        // TODO: make this work with negative scaled transforms! Currently it gets kinda wonky

        if (twisted_bone == null) {
            return;
        }

        float arc_angle_min = constraint_angle_min;
        float arc_angle_max = constraint_angle_max;
        if (arc_angle_min < 0) {
            arc_angle_min = (Mathf.Pi * 2.0f) + arc_angle_min;
        }
        if (arc_angle_max < 0) {
            arc_angle_max = (Mathf.Pi + 2.0f) + arc_angle_max;
        }
        if (arc_angle_min > arc_angle_max) {
            float tmp = arc_angle_min;
            arc_angle_min = arc_angle_max;
            arc_angle_max = tmp;
        }

        arc_angle_min += twisted_bone.bone_angle;
        arc_angle_max += twisted_bone.bone_angle;

        if (constraint_angle_enabled == true) {
            if (constraint_angle_in_localspace == true) {
                Node2D bone_parent = twisted_bone.GetParentOrNull<Node2D>();
                if (bone_parent != null) {
                    DrawSetTransform(GetGlobalTransform().AffineInverse().BasisXform(twisted_bone.GlobalPosition),
                        bone_parent.GlobalRotation - GlobalRotation, Vector2.One);
                }
                else {
                    DrawSetTransform(GetGlobalTransform().AffineInverse().BasisXform(twisted_bone.GlobalPosition), 0, Vector2.One);
                }
            }
            else {
                DrawSetTransform(GetGlobalTransform().AffineInverse().BasisXform(twisted_bone.GlobalPosition), 0, Vector2.One);
            }

            if (constraint_angle_invert == true) {
                DrawArc(Vector2.Zero, twisted_bone.bone_length, arc_angle_min + (Mathf.Pi * 2.0f), arc_angle_max, 32, twisted_bone.gizmo_bone_normal_color, 2.0f, true);
            }
            else {
                DrawArc(Vector2.Zero, twisted_bone.bone_length, arc_angle_min, arc_angle_max, 32, twisted_bone.gizmo_bone_normal_color, 2.0f, true);
            }

            DrawLine(Vector2.Zero, new Vector2(Mathf.Cos(arc_angle_min), Mathf.Sin(arc_angle_min)) * twisted_bone.bone_length, twisted_bone.gizmo_bone_normal_color, 2.0f, true);
            DrawLine(Vector2.Zero, new Vector2(Mathf.Cos(arc_angle_max), Mathf.Sin(arc_angle_max)) * twisted_bone.bone_length, twisted_bone.gizmo_bone_normal_color, 2.0f, true);
        }
        else {
            DrawSetTransform(GetGlobalTransform().AffineInverse().BasisXform(twisted_bone.GlobalPosition), 0, Vector2.One);
            DrawArc(Vector2.Zero, twisted_bone.bone_length, 0, Mathf.Pi * 2.0f, 32, twisted_bone.gizmo_bone_normal_color, 2.0f, true);
            DrawLine(Vector2.Zero, Vector2.Up * twisted_bone.bone_length, twisted_bone.gizmo_bone_normal_color, 2.0f, true);
        }
    }

    /// <summary>
    /// Will redraw the gizmo if the settings are correct. Use this instead of the stock <c>update</c> function if you want
    /// gizmos that draw using the built-in modifier properties.
    /// </summary>
    public void update_gizmo()
    {
        if (Engine.IsEditorHint() == true) {
            if (gizmo_can_draw_in_editor == true) {
                this._Draw();
            }
        }
        else {
            if (gizmo_can_draw_in_game == true) {
                this._Draw();
            }
        }
    }

}
