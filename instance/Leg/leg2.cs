using Godot;
using System;

public partial class leg2 : Node3D
{
	// Called when the node enters the scene tree for the first time.
	//[Export]
	public float moveSpeed = 1.0f;

	private Skeleton3D _skeleton;
	private Transform3D _skeletonTransfom;
	private int[] boneNameArr = new int[2];
	private MeshInstance3D body;

	private WalkingAnimation _walkingAnimation;

	private FootMovement _footMovement;
	public override void _Ready()
	{
        _skeleton = GetNode<Skeleton3D>("modal/Skeleton3D");
		_skeletonTransfom = _skeleton.Transform;

        var left  = _skeleton.FindBone("Leg2.L");
		var right = _skeleton.FindBone("Leg2.R");
        boneNameArr[0] = left;
		boneNameArr[1] = right;
		body = GetNode<MeshInstance3D>("body");

        _walkingAnimation = new WalkingAnimation(MoveFoot,0.3f);

		_footMovement = new FootMovement();
		_footMovement.Initialize(GlobalPosition);
    }

    private void MoveFoot(int stepIndex)
    {
        GD.Print(stepIndex,"我移动了一段时间了，我的脚也要跟着动了");
		_skeleton.GlobalPosition = GlobalPosition + _skeletonTransfom.Origin;

		_footMovement.StepFoot(stepIndex % 2, GlobalPosition.X, GlobalPosition.Y, forword);

    }



    private double timeSum = 0;

	private Vector3 forword;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var zDir = Input.GetAxis("move_up","move_down");
		var xDir = Input.GetAxis("move_right", "move_left");

        if (xDir != 0 || zDir != 0){
            Translate(new Vector3(xDir, 0, -zDir) * moveSpeed * (float)delta);
            _walkingAnimation.Process(delta);
        }

		forword.X = xDir; forword.Y=zDir;

    }
}
