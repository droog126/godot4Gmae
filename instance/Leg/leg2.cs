using Godot;
using System;

public partial class leg2 : Node3D
{
	// Called when the node enters the scene tree for the first time.
	//[Export]
	public float moveSpeed = 3.0f;

	private Node3D leg;
	private Skeleton3D left_skeleton;
	private Skeleton3D right_skeleton;

	private Transform3D _skeletonTransfom;
	private int[] boneIds = new int[2];
	private MeshInstance3D body;

	private WalkingAnimation _walkingAnimation;

	private FootMovement _footMovement;

	private MeshInstance3D[] debugPos= new MeshInstance3D[2];
	public override void _Ready()
	{
		Test.run();

        body = GetNode<MeshInstance3D>("body");

        leg = GetNode<Node3D>("leg");
		left_skeleton = leg.GetNode<Skeleton3D>("leftSkeleton");
		right_skeleton = leg.GetNode<Skeleton3D>("rightSkeleton");
		// 0 为bottom 1 为top

		var leftPos = GetNode<MeshInstance3D>("leftFoot");
		var rightPos = GetNode<MeshInstance3D>("rightFoot");

		debugPos[0] = leftPos;
		debugPos[1] = rightPos;

        _walkingAnimation = new WalkingAnimation(MoveFoot,0.1f);

		_footMovement = new FootMovement();
		_footMovement.Initialize(GlobalPosition);
    }

    private void MoveFoot(int stepIndex)
    {

		_footMovement.StepFoot2(stepIndex % 2,Transform);

    }




	private Vector3 forword;
	private Vector3 targetForword;
	private float AngularSpeed = 20f;
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		var zDir = Input.GetAxis("move_up", "move_down");
		var xDir = Input.GetAxis("move_left", "move_right");



		if (xDir != 0 || zDir != 0)
		{
			_walkingAnimation.Process(delta);

            targetForword = new Vector3(xDir, 0, zDir);


            forword = forword.Lerp(targetForword, AngularSpeed * (float)delta);


			Transform = Transform.LookingAt(Transform.Origin + forword, Vector3.Up);

			GlobalPosition = GlobalPosition + forword * moveSpeed * (float)delta;


        }





        _footMovement.step();
		debugPos[0].GlobalPosition = _footMovement.foot[0];
        debugPos[1].GlobalPosition = _footMovement.foot[1];




        UpdateBone("left", 0, Transform.AffineInverse() * _footMovement.foot[0]);
        UpdateBone("left", 1, new Vector3(-1f, 4f, 0));

        UpdateBone("right", 0, Transform.AffineInverse() * _footMovement.foot[1]);
        UpdateBone("right", 1, new Vector3(1f, 4f, 0));



    }

	





    // ui
    public void UpdateBone(string dirType,int footId, Vector3 relativePos)
    {

		 relativePos = relativePos / 10f;

        if (dirType == "left")
		{
            left_skeleton.SetBonePosePosition(footId, relativePos);

			//需要c# 枚举 或者映射写法 kf kc  ku

		}
        if (dirType == "right")
		{
			right_skeleton.SetBonePosePosition(footId, relativePos);

            //GD.Print($"right:{footId} ,{relativePos}");

        }

    }


}
