#if TOOLS

using Godot;
using System;

[Tool]
public partial class Twisted_Editor_PluginRoot : EditorPlugin
{

    public Control dock_control = null;

    public override void _EnterTree() {

        EditorInterface editor_interface = GetEditorInterface();
        // Make sure the TwistedIK plugin is enabled
        if (editor_interface.IsPluginEnabled("TwistedIK2") == false) {
            // Print an error message and do nothing!
            GD.PrintErr("TwistedIK2 Editor Plugin: Cannot initialize plugin because TwistedIK2 plugin is not found!");
            return;
        }

        dock_control = (Control)GD.Load<PackedScene>("addons/TwistedIK2_EditorPlugin/scenes/Twisted_Editor_Dock.tscn").Instantiate();
        AddControlToBottomPanel(dock_control, "TwistedIK");

        GD.Print("TwistedIK2 Editor Plugin: Initialized successfully!");
    }

    public override void _ExitTree() {
        if (dock_control != null) {
            RemoveControlFromBottomPanel(dock_control);
            dock_control.Free();
        }
    }
}

#endif