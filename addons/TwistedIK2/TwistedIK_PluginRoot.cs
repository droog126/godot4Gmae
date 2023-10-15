#if TOOLS

using Godot;
using System;

[Tool]
public partial class TwistedIK_PluginRoot : EditorPlugin
{
    public override void _EnterTree() {

        Script node_script;
        Texture2D node_texture;

        // ==============================
        // ======= 3D NODES
        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_Skeleton3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_Skeleton3D.png");
        AddCustomType("Twisted_Skeleton3D", "Node3D", node_script, node_texture);
        
        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_Bone3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_Bone3D.png");
        AddCustomType("Twisted_Bone3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_Modifier3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_Modifier3D.png");
        AddCustomType("Twisted_Modifier3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierStack3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierStack3D.png");
        AddCustomType("Twisted_ModifierStack3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_PhysicsBone3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_PhysicsBone3D.png");
        AddCustomType("Twisted_PhysicsBone3D", "RigidBody3D", node_script, node_texture);

        // ======= 3D MODIFIERS
        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierLookAt3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierLookAt3D.png");
        AddCustomType("Twisted_ModifierLookAt3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierCCDIK3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierCCDIK3D.png");
        AddCustomType("Twisted_ModifierCCDIK3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierFABRIK3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierFABRIK3D.png");
        AddCustomType("Twisted_ModifierFABRIK3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierJiggle3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierJiggle3D.png");
        AddCustomType("Twisted_ModifierJiggle3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierTwoBoneIK3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierTwoBoneIK3D.png");
        AddCustomType("Twisted_ModifierTwoBoneIK3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierCurveIK3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierCurveIK3D.png");
        AddCustomType("Twisted_ModifierCurveIK3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierSliderJoint3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierSliderJoint3D.png");
        AddCustomType("Twisted_ModifierSliderJoint3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierDuoIK3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierDuoIK3D.png");
        AddCustomType("Twisted_ModifierDuoBoneIK3D", "Node3D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierVelvet3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_ModifierVelvet3D.png");
        AddCustomType("Twisted_ModifierVelvet3D", "Node3D", node_script, node_texture);

        // ======= 3D OTHER

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_3D/Twisted_PhysicsBoneMotor3D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_3D/Twisted_PhysicsBoneMotor3D.png");
        AddCustomType("Twisted_PhysicsBoneMotor3D", "Node3D", node_script, node_texture);

        // ======= 2D NODES
        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_Bone2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_Bone2D.png");
        AddCustomType("Twisted_Bone2D", "Bone2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_Modifier2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_Modifier2D.png");
        AddCustomType("Twisted_Modifier2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierStack2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierStack2D.png");
        AddCustomType("Twisted_ModifierStack2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_PhysicsBone2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_PhysicsBone2D.png");
        AddCustomType("Twisted_PhysicsBone2D", "RigidBody2D", node_script, node_texture);

        // ======= 2D MODIFIERS
        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierLookAt2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierLookAt2D.png");
        AddCustomType("Twisted_ModifierLookAt2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierCCDIK2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierCCDIK2D.png");
        AddCustomType("Twisted_ModifierCCDIK2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierFABRIK2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierFABRIK2D.png");
        AddCustomType("Twisted_ModifierFABRIK2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierJiggle2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierJiggle2D.png");
        AddCustomType("Twisted_ModifierJiggle2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierCurveIK2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierCurveIK2D.png");
        AddCustomType("Twisted_ModifierCurveIK2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierTwoBoneIK2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierTwoBoneIK2D.png");
        AddCustomType("Twisted_ModifierTwoBoneIK2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierConstrainedFABRIK2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierFABRIK2D.png");
        AddCustomType("Twisted_ModifierConstrainedFABRIK2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierSliderJoint2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierSliderJoint2D.png");
        AddCustomType("Twisted_ModifierSliderJoint2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierDuoIK2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierDuoIK2D.png");
        AddCustomType("Twisted_ModifierDuoBoneIK2D", "Node2D", node_script, node_texture);

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierVelvet2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_ModifierVelvet2D.png");
        AddCustomType("Twisted_ModifierVelvet2D", "Node2D", node_script, node_texture);

        // ======= 2D OTHER

        node_script = GD.Load<Script>("res://addons/TwistedIK2/IK_2D/Twisted_PhysicsBoneMotor2D.cs");
        node_texture = GD.Load<Texture2D>("res://addons/TwistedIK2/IK_2D/Twisted_PhysicsBoneMotor2D.png");
        AddCustomType("Twisted_PhysicsBoneMotor2D", "Node2D", node_script, node_texture);

        // ==============================

    }

    public override void _ExitTree() {
        RemoveCustomType("Twisted_Bone3D");
        RemoveCustomType("Twisted_Skeleton3D");
        RemoveCustomType("Twisted_Modifier3D");
        RemoveCustomType("Twisted_ModifierStack3D");
        RemoveCustomType("Twisted_PhysicsBone3D");

        RemoveCustomType("Twisted_ModifierLookAt3D");
        RemoveCustomType("Twisted_ModifierCCDIK3D");
        RemoveCustomType("Twisted_ModifierFABRIK3D");
        RemoveCustomType("Twisted_ModifierJiggle3D");
        RemoveCustomType("Twisted_ModifierTwoBoneIK3D");
        RemoveCustomType("Twisted_ModifierCurveIK3D");
        RemoveCustomType("Twisted_ModifierSliderJoint3D");
        RemoveCustomType("Twisted_ModifierDuoIK3D");
        RemoveCustomType("Twisted_ModifierVelvet3D");

        RemoveCustomType("Twisted_PhysicsBoneMotor3D");

        RemoveCustomType("Twisted_Bone2D");
        RemoveCustomType("Twisted_Modifier2D");
        RemoveCustomType("Twisted_ModifierStack2D");
        RemoveCustomType("Twisted_PhysicsBone2D");

        RemoveCustomType("Twisted_ModifierLookAt2D");
        RemoveCustomType("Twisted_ModifierCCDIK2D");
        RemoveCustomType("Twisted_ModifierFABRIK2D");
        RemoveCustomType("Twisted_ModifierJiggle2D");
        RemoveCustomType("Twisted_ModifierCurveIK2D");
        RemoveCustomType("Twisted_ModifierTwoBoneIK2D");
        RemoveCustomType("Twisted_ModifierConstrainedFABRIK2D");
        RemoveCustomType("Twisted_ModifierSliderJoint2D");
        RemoveCustomType("Twisted_ModifierDuoIK2D");
        RemoveCustomType("Twisted_ModifierVelvet2D");

        RemoveCustomType("Twisted_PhysicsBoneMotor2D");
    }
}

#endif