using Godot;
using System;
using System.Linq;
using GlobalSpace;

public partial class leg3 : CharacterBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public float moveSpeed = 2.5f;

    [Export]
    public Skeleton3D leftSkeleton { get; set; }

    [Export]
    public Skeleton3D rightSkeleton { get; set; }


    private Transform3D _skeletonTransfom;
	private int[] boneIds = new int[2];
	private MeshInstance3D body;

	private WalkingAnimation _walkingAnimation;

	private FootMovement _footMovement;
	private MeshInstance3D[] debugPos= new MeshInstance3D[2];
    private RayCast3D[] rayCast3Ds = new RayCast3D[2];



    // 0 为bottom 1 为top
    public override void _Ready()
	{
		Test1.run();

        body = GetNode<MeshInstance3D>("body");

  

        // 获取pos节点
		var leftPos = GetNode<MeshInstance3D>("leftFoot");
		var rightPos = GetNode<MeshInstance3D>("rightFoot");
		debugPos[0] = leftPos;
		debugPos[1] = rightPos;


        // 获取ray节点
        rayCast3Ds[0] = (RayCast3D)GetNode("leftRay");
        rayCast3Ds[1] = (RayCast3D)GetNode("rightRay");





        // 更新时间
        _walkingAnimation = new WalkingAnimation(MoveFoot,0.2f);
		_footMovement = new FootMovement();
		_footMovement.Initialize(GlobalPosition,rayCast3Ds);
    }

    private void MoveFoot(int stepIndex)
    {

		_footMovement.StepFoot2(Transform);

    }




	private Vector3 forword;
	private Vector3 targetForword;
	private float AngularSpeed = 20f;
    public override void _Process(double delta)
	{

		var zDir = Input.GetAxis("move_up", "move_down");
		var xDir = Input.GetAxis("move_left", "move_right");



        // 会影响性能
        var inputControl = Global.GetSingleClass<InputControl>("InputControl");



        if (inputControl.inputMode == InputMode.Game && (xDir != 0 || zDir != 0) )
		{

            GD.Print("???",inputControl.inputMode);
            targetForword = new Vector3(xDir, 0, zDir);


            forword = forword.Lerp(targetForword, AngularSpeed * (float)delta);


			Transform = Transform.LookingAt(Transform.Origin + forword, Vector3.Up);

			GlobalPosition = GlobalPosition + forword * moveSpeed * (float)delta;

            _walkingAnimation.Process(delta);

        }





        _footMovement.step();
		debugPos[0].Position = _footMovement.foot[0];
        debugPos[1].Position  = _footMovement.foot[1];



        UpdateBoneV2(leftSkeleton,"leftLegBottom", Transform.AffineInverse() * _footMovement.foot[0]);
        UpdateBoneV2(rightSkeleton, "rightLegBottom", Transform.AffineInverse() * _footMovement.foot[1]);


      
    }



    public override void _PhysicsProcess(double delta)
    {
        // 物理运行
        //getFootColiisionResult(delta);

        //isCanGoUp(delta);
        //MoveAndCollide(velocity);

        // 计算脚
        _footMovement.stepPhysics();
    }



    public void UpdateBoneV2(Skeleton3D node,string boneName,Vector3 pos)
    {
        var  id = node.FindBone(boneName);
        if (id >= 0)
        {
            node.SetBonePosePosition(id, pos);
        }
    }


    const float gravity = 9.8f;
    private Vector3 velocity = Vector3.Zero;


    public void getFootColiisionResult(double delta)
    {
        //var pos = Vector3.Zero;
   
        var leftResult = rayCast3Ds[0].GetCollider();
        if (leftResult == null)
        {
            debugPos[0].Translate(Vector3.Down * (float)delta * 0.1f);

        }
        else
        {
            var pos = rayCast3Ds[0].GetCollisionPoint();

            debugPos[0].Position += (pos - debugPos[0].Position) / 10;

        }


        var rightResult = rayCast3Ds[0].GetCollider();
        if (rightResult == null)
        {
            debugPos[1].Translate(Vector3.Down * (float)delta * 0.1f);
        }
        else
        {
            var pos = rayCast3Ds[1].GetCollisionPoint();

        }

        //var rightResult = rayCast3Ds[1].GetCollider();
        //if (leftResult!=null && rightResult!=null)
        //{
        //    pos += rayCast3Ds[0].GetCollisionPoint() / 2;
        //    pos += rayCast3Ds[1].GetCollisionPoint() / 2;
        //}

        //if (pos == Vector3.Zero)
        //{
        //    velocity += Vector3.Down * (float)gravity * (float)delta;
        //}
        //else
        //{
        //    velocity = Vector3.Zero;
        //}
    }


    public void isCanGoUp(double delta)
    {
        rayCast3Ds[0].Translate(Vector3.Up);
        rayCast3Ds[1].Translate(Vector3.Up);

        rayCast3Ds[0].ForceUpdateTransform();
        rayCast3Ds[1].ForceUpdateTransform();


        var pos = Vector3.Zero;

        var leftResult = rayCast3Ds[0].GetCollider();
        var rightResult = rayCast3Ds[1].GetCollider();
    


        if (leftResult != null && rightResult != null)
        {
            pos += rayCast3Ds[0].GetCollisionPoint() / 2;
            pos += rayCast3Ds[1].GetCollisionPoint() / 2;
        }

        if (pos == Vector3.Zero)
        {
            velocity += Vector3.Down * (float)gravity * (float)delta;
        }
        else
        {
            velocity = Vector3.Zero;
        }

        rayCast3Ds[0].Translate(Vector3.Down);
        rayCast3Ds[1].Translate(Vector3.Down);
    }




    //public void donw()
    //{
    //    const float gravity = 9.8f;


    //    // 计算重力
    //    Vector2 gravityForce = Vector2.down * gravity;

    //    // 运动学公式 - 更新速度  
    //    velocity += gravityForce * Time.deltaTime;

    //    // 更新位置
    //    position += velocity * Time.deltaTime;

    //    velocity


    //}

}
