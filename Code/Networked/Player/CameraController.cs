using Sandbox;

namespace Auralis;
public sealed class CameraController : Component
{
	[Property] public PlayerController Player { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Head { get; set; }
	[Property] public float Distance { get; set; } = 0f;

	[Property] public Vector2 VerticalLimits { get; set; } = new(-89.9f, 89.9f);
	[Property] public Vector2 HorizontalLimits { get; set; } = new(-45f, 45f);


	private CameraComponent _camera;

	private float _yawOffset = 0f;
	private float _pitch = 0f;

	/// <summary>
	/// Fetch components and get pitch
	/// </summary>
	protected override void OnAwake()
	{
		_camera = Components.Get<CameraComponent>();

		var headAngles = Head.WorldRotation.Angles();
		_pitch = headAngles.pitch;
	}

	/// <summary>
	/// Rotate camera based on Mouse delta X and Y, clamping both pitch and yaw based on exposed Limit params.
	/// </summary>
	protected override void OnUpdate()
	{
		_pitch += Input.MouseDelta.y * 0.1f;
		_yawOffset -= Input.MouseDelta.x * 0.1f;

		_pitch = _pitch.Clamp(VerticalLimits.x, VerticalLimits.y);
		_yawOffset = _yawOffset.Clamp(HorizontalLimits.x, HorizontalLimits.y);

		float bodyYaw = Body.WorldRotation.Angles().yaw;

		var headAngles = new Angles
		{
			pitch = _pitch,
			yaw = bodyYaw + _yawOffset,
			roll = 0f
		};

		Head.WorldRotation = headAngles.ToRotation();

		if ( _camera is null )
			return;

		var headPos = Head.WorldPosition;
		var cameraDir = headAngles.ToRotation().Backward;
		var desiredCamPos = headPos + cameraDir * Distance;

		var trace = Scene.Trace.Ray(headPos, desiredCamPos)
			.WithoutTags("player", "trigger")
			.Run();

		_camera.WorldPosition = trace.Hit ? trace.HitPosition + trace.Normal * 0.05f : desiredCamPos;
		_camera.WorldRotation = headAngles.ToRotation();
	}
}
