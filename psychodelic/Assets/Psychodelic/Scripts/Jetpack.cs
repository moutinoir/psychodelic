using UnityEngine;
using System.Collections;

public class Jetpack : MonoBehaviour 
{
	// The maximum horizontal speed when moving
	public float maxAirSpeed = 10.0f;
	public float maxUpSpeed = 10.0f;
	public float accelLerp = 0.02f;
	
	[System.NonSerialized]
	public bool bApplyJetpack;
	
	private bool bHasJetpack, bCanJetpack, bIsUsingTrigger, useFuel, genFuel;
	private float jetpackEnergy, jetpackEnergyMax, jetpackEnergyCost, jetpackAmount, regenDelay, regenTime, useTime;
	private float JPVelocity, JPMaxSpeed, JPViewRotation, JPViewPitch, JPAxis;
	private Vector3 JPAccel, NewAccel;
	
	private Vector3 inputMoveDirection = Vector3.zero;
	private CharacterMotor motor;
	private FPScontroller cc;


	// Use this for initialization
	void Start ()
	{
		bHasJetpack = true;
		jetpackEnergy = 20;
		jetpackEnergyMax = 20;
		jetpackEnergyCost = 0;
		
		regenDelay = 5.0f;
		regenTime = 0.5f;
		useTime = 1.0f;
		
		useFuel = false;
		genFuel = false;
		
		motor = GetComponent<CharacterMotor>();
		
		if(!motor)
		{
			cc = GetComponent<FPScontroller>();   
		}
	}

	void Update ()
	{   
		JPAxis = Input.GetAxis ("JetpackTrigger");
		
		if(JPAxis > 0)
		{
			//print (JPAxis);
			bIsUsingTrigger = true;
			jetpackAmount = JPAxis * maxUpSpeed;
		}
		
		if(Input.GetButton("Jetpack") || JPAxis > 0)
		{
			if(bHasJetpack)
			{               
				StartJetpackBoost();
				
				if(jetpackEnergy > 5)
				{
					bApplyJetpack = true;
				}
			}
		}
		else if(bApplyJetpack)
		{
			bApplyJetpack = false;  
			useFuel = false;
			CancelInvoke("UseJetpackEnergy");
			
			if(!genFuel)
				InvokeRepeating("RegenJetpackEnergy", regenDelay, regenTime); 
		}
	}

	void StopJetpack()
	{
		bApplyJetpack   = false;
		bIsUsingTrigger = false;
		CancelInvoke("UseJetpackEnergy");
		InvokeRepeating("RegenJetpackEnergy", regenDelay, regenTime); 
	}
	
	void UseJetpackEnergy()
	{
		if(bApplyJetpack && jetpackEnergy > 0)
		{
			jetpackEnergy -= jetpackEnergyCost;
			
			if(!useFuel)
			{
				CancelInvoke("RegenJetpackEnergy");
				InvokeRepeating("UseJetpackEnergy", useTime, useTime); 
				useFuel = true;
			}
		}
		//print (string.Format("USE: {0} {1}", jetpackEnergy, useFuel));
	}
	
	void RegenJetpackEnergy()
	{
		if(jetpackEnergy < jetpackEnergyMax)
		{
			genFuel = true;
			
			jetpackEnergy += jetpackEnergyCost;
			//print (string.Format("GEN: {0} {1}", jetpackEnergy, useFuel));
		}
		else
		{
			genFuel = false;
			CancelInvoke("RegenJetpackEnergy");
			//print (string.Format("FULL: {0} {1}", jetpackEnergy, useFuel));
		}
		
		if(jetpackEnergy > 25)
		{
			bCanJetpack = true;
		}
	}
	
	void StartJetpackBoost()
	{   
		if(jetpackEnergy < 1)
		{
			print ("No Fuel");
			bCanJetpack = false;
			StopJetpack();
		}
		
		if(!useFuel)
		{
			UseJetpackEnergy();
			if(motor)
			{
				JPVelocity = motor.movement.velocity.y;
				NewAccel = motor.movement.velocity;
			}
			else
			{
				JPVelocity = cc.movement.velocity.y;
				NewAccel = cc.movement.velocity;
			}
			
		}
		
		/**
     *  JETPACK VELOCITY STUFF 
     */
		if(bApplyJetpack)
		{
			var directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			var JPOffset = 1 - (directionVector.z / 1.5f);
			
			if (directionVector != Vector3.zero)
			{
				// Get the length of the directon vector and then normalize it
				// Dividing by the length is cheaper than normalizing when we already have the length anyway
				var directionLength = directionVector.magnitude;
				directionVector = directionVector / directionLength;
				
				// Make sure the length is no bigger than 1
				directionLength = Mathf.Min(1, directionLength);
				
				// Make the input vector more sensitive towards the extremes and less sensitive in the middle
				// This makes it easier to control slow speeds when using analog sticks
				directionLength = directionLength * directionLength;
				
				// Multiply the normalized direction vector by the modified length
				directionVector = directionVector * directionLength;
			}
			// Apply the direction to the CharacterMotor
			JPAccel = Vector3.Scale(
				transform.rotation * directionVector, 
				new Vector3(maxAirSpeed,maxAirSpeed,maxAirSpeed));
			
			if(bIsUsingTrigger)
			{   
				JPVelocity = Mathf.Lerp(JPVelocity, jetpackAmount, 0.02f);
			}
			else
			{
				JPVelocity = Mathf.Lerp(JPVelocity, maxUpSpeed, 0.02f);
			}
			
			NewAccel = Vector3.Lerp(NewAccel, JPAccel, accelLerp);
			NewAccel.y = JPVelocity;
			
			//print (NewAccel);
			if(motor)
			{
				motor.SetVelocity(NewAccel);
			}
			else
			{
				cc.SetVelocity(NewAccel);   
			}
		}
	}

	void OnExternalVelocity(){}

}
