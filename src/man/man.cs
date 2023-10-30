using Godot;
using System;

public partial class Man : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


	public override void _Ready()
	{
		GD.Print("?????");
	}

	public override void _PhysicsProcess(double delta)
	{

		GD.Print("??");
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("space") && IsOnFloor())
			velocity.Y = JumpVelocity;


		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}


