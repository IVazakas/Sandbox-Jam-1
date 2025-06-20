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


	private Vector3 _wishVelocity = Vector3.Zero;
	private bool _isCrouching = false;
	private bool _isSprinting = false;

	private CharacterController _controller;

	protected override void OnAwake()
	{
		_controller = Components.Get<CharacterController>();
	}


	protected override void OnUpdate()
	{

	}

	private void BuildWishVelocity()
	{
		_wishVelocity = 0;

		Rotation rot = Head.WorldRotation;
		if ( Input.Down( "Forward" ) ) _wishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) _wishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) _wishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) _wishVelocity += rot.Right;

		_wishVelocity = _wishVelocity.WithZ( 0 );
		if ( !_wishVelocity.IsNearZeroLength )
			_wishVelocity = _wishVelocity.Normal;

		if ( _isCrouching )
			_wishVelocity *= CrouchSpeed;
		else if ( _isSprinting )
			_wishVelocity *= BoostSpeed;
		else
			_wishVelocity *= Speed;
	}

	private void Move()
	{
		Vector3 gravity = Scene.PhysicsWorld.Gravity;

		if(_controller.IsOnGround)
		{
			_controller.Velocity = _controller.Velocity.WithZ( 0 );
		}
	}

}
