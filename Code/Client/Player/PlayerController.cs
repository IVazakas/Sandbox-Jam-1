using Sandbox;
using System.Text;

namespace Auralis;
public sealed class PlayerController : Component
{
	[Property] public float GroundControl { get; set; } = 2.0f;

	[Property] public float AirControl { get; set; } = 0.1f; 
	[Property] public float MaxForce { get; set; } = 50f;
	[Property] public float Speed { get; set; } = 160f;
	[Property] public float BoostSpeed { get; set; } = 290f;
	[Property] public float CrouchSpeed { get; set; } = 90f;
	[Property] public float JumpForce { get; set; } = 400f;

	[Property] public GameObject CameraTarget { get; set; }
	[Property] public GameObject Head { get; set; }
	[Property] public GameObject Body { get; set; }

	[Property] public float MaxTurnSpeed = 2.5f;
	[Property] public float TurnAccel = 50f;
	[Property] public float TurnDecel = 10f;

	private Vector3 _wishVelocity = Vector3.Zero;
	private bool _isCrouching = false;
	private bool _isSprinting = false;

	private float turnVelocity = 0f;

	private CharacterController _controller;

	protected override void OnAwake()
	{
		_controller = Components.Get<CharacterController>();
	}


	protected override void OnUpdate()
	{
		_isCrouching = Input.Down("Crouch");
		_isSprinting = Input.Down("Sprint");

		Rotate();
	}

	protected override void OnFixedUpdate()
	{
		BuildWishVelocity();
		Move();
	}

	private void BuildWishVelocity()
	{
		_wishVelocity = 0;

		Rotation rot = Body.WorldRotation; // If set to head it will move in the rotation of the head, might be useful later for other mech types
		if (Input.Down("Forward")) 
			_wishVelocity += rot.Forward;
		if (Input.Down("Backward")) 
			_wishVelocity += rot.Backward;

		_wishVelocity = _wishVelocity.WithZ( 0 );

		if (!_wishVelocity.IsNearZeroLength)
			_wishVelocity = _wishVelocity.Normal;

		if (_isCrouching)
			_wishVelocity *= CrouchSpeed;
		else if (_isSprinting)
			_wishVelocity *= BoostSpeed;
		else
			_wishVelocity *= Speed;
	}

	private void Move()
	{
		Vector3 gravity = Scene.PhysicsWorld.Gravity; // Tldr basic movement stuff, just tell the controller to move differently based on ground or air (though air shouldnt be necessary)

		if(_controller.IsOnGround)
		{
			_controller.Velocity = _controller.Velocity.WithZ(0);
			_controller.Accelerate(_wishVelocity);
			_controller.ApplyFriction(GroundControl);
		}
		else
		{
			_controller.Velocity += gravity * Time.Delta * 0.5f;
			_controller.Accelerate(_wishVelocity.ClampLength(MaxForce));
			_controller.ApplyFriction(AirControl);
		}

		_controller.Move();

		if(!_controller.IsOnGround)
		{
			_controller.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			_controller.Velocity = _controller.Velocity.WithZ(0);
		}
	}

	private void Rotate()
	{
		if (Body is null) // Rotate the body based on the A and D keys to give it a "tank controls" effect
			return;

		float targetSpeed = 0f;

		if (Input.Down("left"))
			targetSpeed = MaxTurnSpeed;
		if (Input.Down("right"))
			targetSpeed = -MaxTurnSpeed;

		float lerpSpeed = (targetSpeed != 0f) ? TurnAccel : TurnDecel;

		turnVelocity = MathX.Lerp(turnVelocity, targetSpeed, Time.Delta * lerpSpeed);

		Body.WorldRotation *= Rotation.FromYaw(turnVelocity * Time.Delta);
	}

}
