using Godot;
using System;

public partial class leg3 : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public float moveSpeed = 2.5f;

    [Export]
    public Skeleton3D leftSkelton { get; set; }

    [Export]
    public Skeleton3D rightSeleton { get; set; }


    private Transform3D _skeletonTransfom;
	private int[] boneIds = new int[2];
	private MeshInstance3D body;

	private WalkingAnimation _walkingAnimation;

	private FootMovement _footMovement;
	private MeshInstance3D[] debugPos= new MeshInstance3D[2];


    // 0 为bottom 1 为top

    public override void _Ready()
	{
		Test.run();

        body = GetNode<MeshInstance3D>("body");

  

		var leftPos = GetNode<MeshInstance3D>("leftFoot");
		var rightPos = GetNode<MeshInstance3D>("rightFoot");

		debugPos[0] = leftPos;
		debugPos[1] = rightPos;

        // 更新时间
        _walkingAnimation = new WalkingAnimation(MoveFoot,0.2f);

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
    public override void _Process(double delta)
	{
		var zDir = Input.GetAxis("move_up", "move_down");
		var xDir = Input.GetAxis("move_left", "move_right");



		if (xDir != 0 || zDir != 0)
		{

            targetForword = new Vector3(xDir, 0, zDir);


            forword = forword.Lerp(targetForword, AngularSpeed * (float)delta);


			Transform = Transform.LookingAt(Transform.Origin + forword, Vector3.Up);

			GlobalPosition = GlobalPosition + forword * moveSpeed * (float)delta;

                    _walkingAnimation.Process(delta);

        }





        _footMovement.step();
		debugPos[0].GlobalPosition = _footMovement.foot[0];
        debugPos[1].GlobalPosition = _footMovement.foot[1];



        UpdateBoneV2(leftSkelton,"leftLegBottom", Transform.AffineInverse() * _footMovement.foot[0]);
        UpdateBoneV2(rightSeleton, "rightLegBottom", Transform.AffineInverse() * _footMovement.foot[1]);


    }



    public void UpdateBoneV2(Skeleton3D node,string boneName,Vector3 pos)
    {
        var  id = node.FindBone(boneName);
        if (id >= 0)
        {
            node.SetBonePosePosition(id, pos);
        }
    }



}
