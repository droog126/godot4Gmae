using Godot;
using System;
public partial class man : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public double jumpStrength = 7;
	public JumpClass jumpClass;
	public Vector3 velocity;

	public override void _Ready()
	{
		jumpClass = new JumpClass(jumpCallback);
	}

	public override void _PhysicsProcess(double delta)
	{
		// xz
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

		//y
		jumpClass.Step(delta);

		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
		}
		else
		{
			jumpClass.Reset();
			Scale = new Vector3(1.25f, 0.75f, 1.25f);
		}

		GD.Print(velocity, Velocity);
		Velocity = Velocity.Lerp(velocity, (float)(delta * 10.0));
		Scale = Scale.Lerp(Vector3.One, (float)(delta * 5.0));
		MoveAndSlide();
	}

	public void jumpCallback()
	{
		velocity.Y = (float)jumpStrength;
		Scale = new Vector3(0.5f, 1.5f, 0.5f);
	}
}


public class JumpClass
{
	public bool jumpSingle = false;
	public bool jumpDouble = false;
	public Action jumpCallback;

	public JumpClass(Action jumpCallback)
	{
		this.jumpCallback = jumpCallback;
		jumpSingle = true;
		jumpDouble = true;
	}


	public void Step(double delta)
	{
		if (Input.IsActionJustPressed("space"))
		{

			if (jumpSingle || jumpDouble)
			{
				jumpCallback();
			}
			if (jumpDouble)
			{
				jumpDouble = false;
			}
			if (jumpSingle)
			{
				jumpSingle = false;
				jumpDouble = true;
			}
		}
	}
	public void Reset()
	{
		jumpSingle = true;
		jumpDouble = true;
	}


}