using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class Twisted_Editor_Dock_Skeleton3D : Twisted_Editor_Dock_Base
{

    private Twisted_Skeleton3D selected_skeleton;

    private Button create_bones_button;

    public override void _Ready()
    {
        create_bones_button = GetNodeOrNull<Button>("MarginContainer/DockPanel/VBoxContainer/HBoxContainer/Create_Bones");
        if (create_bones_button != null) {
            create_bones_button.Connect("pressed",new Callable(this,"on_create_bones_pressed"));
        }
    }

    public override void on_new_refresh(Node selected_node) {
        base.on_new_refresh(selected_node);

        selected_skeleton = selected_node as Twisted_Skeleton3D;
        if (selected_skeleton != null) {
            Visible = true;
            // TODO: do initialization stuff here!
        }
        else {
            Visible = false;
        }
    }

    public void on_create_bones_pressed() {
        if (selected_skeleton == null) {
            return;
        }
        if (selected_skeleton.current_skeleton == null) {
            return;
        }

        selected_skeleton.update_skeleton3d_bone_data();
        Dictionary<int, SKELETON3D_BONE_DATA> bones_data = selected_skeleton.get_skeleton3d_bone_data();
        List<int> root_bones = selected_skeleton.get_skeleton3d_root_bones();

        List<int> bones_to_process = new List<int>();
        for (int i = 0; i < root_bones.Count; i++) {
            bones_to_process.Add(root_bones[i]);
        }
        Dictionary<int, Twisted_Bone3D> node_dictionary = new Dictionary<int, Twisted_Bone3D>();

        // Make the tree using the root bones!
        while (bones_to_process.Count > 0) {
            int current_bone = (int)bones_to_process[0];
            bones_to_process.RemoveAt(0);

            SKELETON3D_BONE_DATA bone_data = bones_data[current_bone];
            
            if (bone_data.parent_index == -1) {
                Twisted_Bone3D new_twisted_bone = new Twisted_Bone3D();
                // =============
                // Set the script (source: https://github.com/godotengine/godot/issues/31994)
                ulong object_id = new_twisted_bone.GetInstanceId();
                new_twisted_bone.SetScript(ResourceLoader.Load("addons/TwistedIK2/IK_3D/Twisted_Bone3D.cs"));
                new_twisted_bone = (Twisted_Bone3D)GodotObject.InstanceFromId(object_id);
                // =============

                new_twisted_bone.Name = selected_skeleton.current_skeleton.GetBoneName(current_bone);
                new_twisted_bone.bone_id = current_bone;
                new_twisted_bone.bone_name = new_twisted_bone.Name;
                new_twisted_bone.auto_calcualte_bone_length = false;

                selected_skeleton.AddChild(new_twisted_bone);

                new_twisted_bone.Owner = selected_skeleton.Owner;
                node_dictionary.Add(current_bone, new_twisted_bone);
            } else {
                Twisted_Bone3D parent_bone = node_dictionary[bone_data.parent_index];

                Twisted_Bone3D new_twisted_bone = new Twisted_Bone3D();
                // =============
                // Set the script (source: https://github.com/godotengine/godot/issues/31994)
                ulong object_id = new_twisted_bone.GetInstanceId();
                new_twisted_bone.SetScript(ResourceLoader.Load("addons/TwistedIK2/IK_3D/Twisted_Bone3D.cs"));
                new_twisted_bone = (Twisted_Bone3D)GodotObject.InstanceFromId(object_id);
                // =============
                new_twisted_bone.Name = selected_skeleton.current_skeleton.GetBoneName(current_bone);
                new_twisted_bone.bone_id = current_bone;
                new_twisted_bone.bone_name = new_twisted_bone.Name;
                 new_twisted_bone.auto_calcualte_bone_length = false;

                parent_bone.AddChild(new_twisted_bone);

                new_twisted_bone.Owner = parent_bone.Owner;
                node_dictionary.Add(current_bone, new_twisted_bone);
            }

            for (int i = 0; i < bone_data.children_indexes.Count; i++) {
                bones_to_process.Add(bone_data.children_indexes[i]);
            }
        }

        for (int i = 0; i < node_dictionary.Count; i++) {
            Twisted_Bone3D bone = node_dictionary[i];
            bone.auto_calcualte_bone_length = true;
            node_dictionary[i] = bone;
        }

        // TODO: tell the scene we have done stuff so we can save the nodes! Right now we have to do
        // something in the editor first to trigger a scene save.
    }
}
