using UnityEngine;
using System.Collections;
using InControl;

public class XBoxController : MonoBehaviour 
{
	public float Joystick;
	public float Fire1;

	private InputControl JoystickInput;
	private InputControl Fire1Input;
	private InputDevice inputDevice; 

	void Start () 
	{
		string[] joystickNames = Input.GetJoystickNames ();
		foreach(string joystickName in joystickNames)
		{
			Debug.Log(joystickName);
		}
	}

	void Update () 
	{
		inputDevice = InputManager.ActiveDevice;
		JoystickInput = inputDevice.LeftStickX;
		Fire1Input = inputDevice.Action1;

		Joystick = JoystickInput.Value;
		Fire1 = Fire1Input.Value;

		//Joystick = Input.GetAxis ("Horizontal");
		//Fire1 = Input.GetAxis ("Fire1");


		
		// Rotate target object with left stick.
		//transform.Rotate( Vector3.down,  500.0f * Time.deltaTime * inputDevice.LeftStickX, Space.World );
		//transform.Rotate( Vector3.right, 500.0f * Time.deltaTime * inputDevice.LeftStickY, Space.World );
	}
}
