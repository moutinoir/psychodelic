using UnityEngine;
using System.Collections;
using InControl;

public class PGJetpack : MonoBehaviour 
{
	public float RollFactor = 3f;
	public float PitchFactor = 3f;
	public float YawFactor = 3f;
	public float UnrollFactor = 1f;
	public float UnpitchFactor = 1f;
	public float ThrustFactor = 1f;
	public float GravityFactor = 0.1f;
	public float MaxVelocityMagnitude = 20f;

	private InputDevice inputDevice; 
	private InputControl LeftMotor;
	private InputControl RightMotor;
	private InputControl Pitch;
	private InputControl Yaw;

	private CharacterController Controller = null;

	private Vector3 velocity;

	private void Awake ()
	{
		Controller = gameObject.GetComponent<CharacterController>();
	}

	private void Start()
	{

	}

	private void Update()
	{
		inputDevice = InputManager.ActiveDevice;

		LeftMotor = inputDevice.LeftTrigger;
		RightMotor = inputDevice.RightTrigger;
		Pitch = inputDevice.LeftStickY;
		Yaw = inputDevice.RightStickX;

		// roll
		transform.Rotate ((-LeftMotor.Value + RightMotor.Value) * RollFactor * Vector3.forward * Time.deltaTime);

		// pitch
		transform.Rotate (Pitch.Value * PitchFactor * Vector3.right * Time.deltaTime);

		// yaw
		transform.Rotate (Yaw.Value * YawFactor * Vector3.up * Time.deltaTime);

		// thrust
		velocity += transform.up * ThrustFactor * (LeftMotor.Value + RightMotor.Value) + Physics.gravity * GravityFactor;

		if (Controller.isGrounded)
		{
			velocity = Vector3.zero;

		}

		// unroll
		transform.RotateAround(transform.position, transform.forward, -Vector3.Dot (transform.right, Vector3.up) * UnrollFactor);

		// unpitch
		transform.RotateAround(transform.position, transform.right, Vector3.Dot (transform.forward, Vector3.up) * UnpitchFactor);


		velocity = Vector3.ClampMagnitude (velocity, MaxVelocityMagnitude);

		Controller.Move (velocity * Time.deltaTime);



	}
}
