using Godot;
using System;



[Tool]
public partial class 提取脚本 : Node3D
{
    private Node _nodeToExtract;


    private bool action = false;
    [Export]
    public bool 提取
    {
        get
        {
            return action;
        }
        set
        {
            editorAction();
            action = value;
        }
    }


    public override void _Ready()
    {
        //var node = new Node3D();
        //AddChild(node);
        //node.Owner = GetTree().EditedSceneRoot;
    }



    public void editorAction()
    {
        if (Engine.IsEditorHint())
        {
            GD.Print("编辑器脚本运行");
            extractNode("地图");
            extractNode("杂项");
            extractNode("人物");
            extractNode("demo");
            extractNode("newPeople");

        }
    }

    public  void extractNode(String name)
    {
        // https://www.reddit.com/r/godot/comments/kpgu3v/using_packed_scene_to_save_everything/
        //var c = new Control();
        //var s = ResourceLoader.Load<PackedScene>("res://blender模型/核心2.tscn").Instantiate();
        //c.Owner = a_root;
        //s.Owner = a_root;



        Node node = GetNode<Node>(name);
        //var root = new Node();
        //root.
        //root.AddChild(node);
        Own(node, node);
        GD.Print($"{name} {node}");

        PackedScene packedScene = new PackedScene();
        Error result = packedScene.Pack(node);
        if (result == Error.Ok)
        {
            ResourceSaver.Save(packedScene, $"res://blender提取/{name}.tscn");
            GD.Print("Node saved to file.");
        }
        else
        {
            GD.Print("Failed to pack the node.");
        }

    }

    public static void Own(Node node, Node newOwner)
    {
        node.Owner = newOwner;
        if (node.GetChildCount() > 0)
        {
            foreach (Node kid in node.GetChildren())
            {
                Own(kid, newOwner);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            //GD.Print("脚本运行中");
        }
    }
}
