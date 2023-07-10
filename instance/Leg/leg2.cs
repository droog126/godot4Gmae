using Godot;
using System;

public partial class leg2 : Node3D
{
	// Called when the node enters the scene tree for the first time.
	//[Export]
	public float moveSpeed = 10.0f;

	private Node3D leg;
	private Skeleton3D left_skeleton;
	private Skeleton3D right_skeleton;

	private Transform3D _skeletonTransfom;
	private int[] boneIds = new int[2];
	private MeshInstance3D body;

	private WalkingAnimation _walkingAnimation;

	private FootMovement _footMovement;
	public override void _Ready()
	{

        body = GetNode<MeshInstance3D>("body");

        leg = GetNode<Node3D>("leg");
		left_skeleton = leg.GetNode<Skeleton3D>("leftSkeleton");
		right_skeleton = leg.GetNode<Skeleton3D>("rightSkeleton");
		// 0 为bottom 1 为top


      

        _walkingAnimation = new WalkingAnimation(MoveFoot,0.3f);

		_footMovement = new FootMovement();
		_footMovement.Initialize(GlobalPosition);
    }

    private void MoveFoot(int stepIndex)
    {
        GD.Print(stepIndex,"我移动了一段时间了，我的脚也要跟着动了");
		_footMovement.StepFoot(stepIndex % 2, GlobalPosition,forword);

    }




	private Vector3 forword;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var zDir = Input.GetAxis("move_up", "move_down");
		var xDir = Input.GetAxis("move_right", "move_left");

		if (xDir != 0 || zDir != 0)
		{
			Translate(new Vector3(xDir, 0, -zDir) * moveSpeed * (float)delta);
			_walkingAnimation.Process(delta);
		}

		forword.X = xDir; forword.Z = zDir;

		_footMovement.step();

		GD.Print($"{_footMovement.foot[0]},{_footMovement.foot[1]}");

		UpdateBone("left", 0, _footMovement.foot[0], GlobalPosition);
        UpdateBone("left", 1, GlobalPosition + new Vector3(-1f,4f,0), GlobalPosition);

        UpdateBone("right", 0, _footMovement.foot[1], GlobalPosition);
        UpdateBone("right", 1, GlobalPosition + new Vector3(1f, 4f, 0), GlobalPosition);
    }


    // ui
    public void UpdateBone(string dirType,int footId, Vector3 globalPos,Vector3 parentPos)
    {

		var relativePos = (globalPos - parentPos) / 10f;

        if (dirType == "left")
		{
            left_skeleton.SetBonePosePosition(footId, relativePos);








			GD.Print($"left:{footId} ,{relativePos}");
			//需要c# 枚举 或者映射写法 kf kc  ku


		}
        if (dirType == "right")
		{
			right_skeleton.SetBonePosePosition(footId, relativePos);

            GD.Print($"right:{footId} ,{relativePos}");

        }

    }


}
