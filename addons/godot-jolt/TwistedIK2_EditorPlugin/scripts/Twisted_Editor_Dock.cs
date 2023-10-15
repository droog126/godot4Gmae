using Godot;
using System;

[Tool]
public partial class Twisted_Editor_Dock : Control
{

	// Needed to get the selected node.
	public EditorInterface plugin_interface;

	public Button refresh_button;


	public Control docks_holder_node;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// ========================
		// Get the editor interface
		EditorPlugin tmp_plugin = new EditorPlugin();
		plugin_interface = tmp_plugin.GetEditorInterface();
		tmp_plugin.QueueFree();
		// ========================
	

		docks_holder_node = GetNodeOrNull<Control>("DockContainer");

		 // Tell all of the docks of our existance
		Godot.Collections.Array<Node> children_docks = docks_holder_node.GetChildren();
		for (int i = 0; i < children_docks.Count; i++) {
			Twisted_Editor_Dock_Base base_node = children_docks[i] as Twisted_Editor_Dock_Base;
			if (base_node != null) {
				base_node.dock_reference = this;
			}
			base_node.Visible = false;
		}

		refresh_button = GetNodeOrNull<Button>("RefreshButton");
		if (refresh_button != null) {
			refresh_button.Connect("pressed",new Callable(this,"on_refresh_pressed"));
		}
	}

	public void on_refresh_pressed() {
		if (plugin_interface != null) {
			Godot.Collections.Array<Node> selection = plugin_interface.GetSelection().GetSelectedNodes();
			if (selection.Count > 0) {
				// We only care about the first node
				Node selected_node = (Node)selection[0];

				// Tell all of the docks to update
				Godot.Collections.Array<Node> children_docks = docks_holder_node.GetChildren();
				for (int i = 0; i < children_docks.Count; i++) {
					Twisted_Editor_Dock_Base base_node = children_docks[i] as Twisted_Editor_Dock_Base;
					if (base_node != null) {
						base_node.on_new_refresh(selected_node);
					}
				}
			}
			else {
				// Tell all of the docks to update
				Godot.Collections.Array<Node> children_docks = docks_holder_node.GetChildren();
				for (int i = 0; i < children_docks.Count; i++) {
					Twisted_Editor_Dock_Base base_node = children_docks[i] as Twisted_Editor_Dock_Base;
					if (base_node != null) {
						base_node.on_new_refresh(null);
					}
				}
			}
		}
	}
}


[Tool]
public partial class Twisted_Editor_Dock_Base : Control {
	public Twisted_Editor_Dock dock_reference;

	public virtual void on_new_refresh(Node selected_node) {
		Visible = false;
	}
}
