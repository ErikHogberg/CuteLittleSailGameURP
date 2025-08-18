using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SailboatScript : MonoBehaviour
{

	public float GravityAmount = 9.81f;
	public AnimationCurve GravityDropoff;
	public float GravityMaxDistanceSqr = 9f;
	public Vector3 WindDir = Vector3.down;
	public float ThrustAmount = 1f;
	public float TurnAngularForce = 1f;
	public float MinThrustPercent = .01f;


	[Range(0, 1)]
	public float FrontRearForceRatio = .5f;
	public float VelocityCap = 1f;

	[Space]
	public Camera Cam;
	public GameObject Rudder;
	public GameObject Mainsail;
	// TODO: rename jib, or headsail?
	// IDEA: option to ignore foresail, to reduce difficulty?
	// IDEA: add horizontal wings for controlling pitch
	public GameObject Foresail;
	public GameObject Cannon;
	public GameObject CannonBallSpawnPoint;
	public GameObject CannonBallObject;

	// TODO: draw projectile arc
	// IDEA: only on fire button down, fire on release?

	[Space]
	public float NoGoAngleThreshold = 30f;
	public AnimationCurve MainsailBeatingCurve;
	public AnimationCurve MainsailReachingCurve;
	public AnimationCurve ForesailBeatingCurve;
	public AnimationCurve ForesailReachingCurve;

	[Space]
	public float RudderSpeed = 1;
	public float RudderAngleMul = 1;
	public ForceMode RudderForceMode = ForceMode.Force;
	[Space]
	public float MainsailSpeed = 1;
	public float MainsailBufferSpeed = 1;
	public float MainsailAngleMul = 1;
	[Space]
	public float ForesailSpeed = 1;
	public float ForesailBufferSpeed = 1;
	public float ForesailAngleMul = 1;

	[Space]
	public float CannonSpeed = 1;
	public float CannonBufferSpeed = 1;
	public float CannonAngleMul = 1;

	public float CannonBallVelocity = 1f;

	[Space]
	public bool UsePointer = true;
	public float RudderMaxPointerAngle = 45;

	float rudderValue = 0;
	float rudderBuffer = 0;
	float mainsailValue = 0;
	float mainsailBuffer = 0;
	float foresailValue = 0;
	float foresailBuffer = 0;
	float turnCannonValue = 0;
	float turnCannonBuffer = 0;

	Quaternion foresailRot;

	Rigidbody boatRb;

	// float angleBuffer = 0;
	bool SailsOn = true;

	[Space]
	public Vector3 WeightCenter;
	// public Vector3 FrontForcePoint;
	// public Vector3 RearForcePoint;
	[Space]
	public GameObject ResetObject;

	[Space]
	public UnityEvent<float> RudderEvent;
	public UnityEvent<float> MainsailEvent;
	public UnityEvent<float> ForesailEvent;
	public UnityEvent<float> TurnCannonEvent;
	public UnityEvent<Vector3> WindDirEvent;
	public UnityEvent<string> WindMagnitudeEvent;

	Vector3 initPos;
	Transform lastGravitySource = null;

	void Start()
	{
		if (!Cam) Cam = Camera.main;
		foresailRot = Foresail.transform.localRotation;
		boatRb = GetComponent<Rigidbody>();
		boatRb.centerOfMass = WeightCenter;

		RudderEvent.Invoke(rudderValue);
		MainsailEvent.Invoke(mainsailValue);
		ForesailEvent.Invoke(foresailValue);
		TurnCannonEvent.Invoke(turnCannonValue);

		if (ResetObject)
			initPos = transform.position - ResetObject.transform.position;
		else
			initPos = transform.position;
	}

	void Update()
	{

		// IDEA: option to set rudder using mouse, always angling to steer towards projected mouse position on plane
		rudderBuffer = Mathf.MoveTowards(rudderBuffer, rudderValue, RudderSpeed * Time.deltaTime);

		// mainsailBuffer = Mathf.MoveTowards(mainsailBuffer, mainsail, MainsailBufferSpeed * Time.deltaTime);
		if (Mathf.Abs(mainsailValue) > 0)
		{
			mainsailBuffer += mainsailValue * MainsailBufferSpeed * Time.deltaTime;
			mainsailBuffer = Mathf.Clamp01(mainsailBuffer);
			MainsailEvent.Invoke(mainsailBuffer);
		}

		// foresailBuffer = Mathf.MoveTowards(foresailBuffer, foresail, ForesailBufferSpeed * Time.deltaTime);
		if (Mathf.Abs(foresailValue) > 0)
		{
			foresailBuffer += foresailValue * ForesailBufferSpeed * Time.deltaTime;
			foresailBuffer = Mathf.Clamp01(foresailBuffer);
			ForesailEvent.Invoke(foresailBuffer);
		}

		Rudder.transform.localRotation = Quaternion.AngleAxis(rudderBuffer * RudderAngleMul, Vector3.down);
		Mainsail.transform.localRotation = Quaternion.AngleAxis(mainsailBuffer * MainsailAngleMul, Vector3.forward);
		Foresail.transform.localRotation = Quaternion.AngleAxis(foresailBuffer * ForesailAngleMul, foresailRot * Vector3.forward) * foresailRot;


		if (Mathf.Abs(turnCannonValue) > 0)
		{
			turnCannonBuffer += turnCannonValue * CannonBufferSpeed * Time.deltaTime;
			turnCannonBuffer = Mathf.Clamp(turnCannonBuffer, -1, 1);
			TurnCannonEvent.Invoke(turnCannonBuffer);
		}
		Cannon.transform.localRotation = Quaternion.AngleAxis(turnCannonBuffer * CannonAngleMul, Vector3.up);
	}

	bool noGravitySources = true;
	float mainsailPercent = 0f;

	bool pointerDown = false;

	private void FixedUpdate()
	{

		if (UsePointer)
		{
			if (Pointer.current != null)
			{
				bool steerToPointer = false;
				if (Pointer.current.IsPressed())
				{
					if (pointerDown)
					{
						steerToPointer = true;
						// Debug.Log("pressed pointer");
					}
					else
					{
						if (EventSystem.current != null
						&& !EventSystem.current.IsPointerOverGameObject())
						{
							pointerDown = true;
							steerToPointer = true;
						}
						else
						{
							// Debug.Log("pointer hit ui");

						}
					}
				}
				else
				{
					pointerDown = false;
				}

				if (steerToPointer)
				{
					// Debug.Log("steer to pointer true");
					Vector2 pointerPos = Pointer.current.position.ReadValue();
					Ray mousePickRay = Cam.ScreenPointToRay(Pointer.current.position.ReadValue());
					// Vector3 mousePos = Cam.ScreenToWorldPoint(new Vector3(pointerPos.x, pointerPos.y, Cam.nearClipPlane));
					Vector3 mouseDir = Cam.transform.forward;

					Plane boatPlane = new Plane(transform.up, transform.position);
					if (boatPlane.Raycast(mousePickRay, out float enter))
					{
						Vector3 planePos = mousePickRay.GetPoint(enter);
						float angle = Vector3.SignedAngle(transform.forward, planePos - transform.position, transform.up);
						rudderValue = Mathf.Abs(angle) / RudderMaxPointerAngle * Mathf.Sign(angle);
						RudderEvent.Invoke(rudderValue);
						// Debug.Log($"set rudder to {rudderValue} using pointer");
					}
				}
			}
		}

		if (SailsOn)
		{


			// thrust
			// TODO: calculate forward speed based on mainsail angle compared to wind
			Vector3 boatWindFacing = Vector3.ProjectOnPlane(WindDir, transform.up);
			float boatWindAngle = Vector3.SignedAngle(transform.forward, WindDir, transform.up);
			float boatWindAngleAbs = Mathf.Abs(boatWindAngle);
			float windMagnitude = boatWindFacing.magnitude;

			mainsailPercent = 0f;
			// TODO: account for mainsail angle
			float mainsailWindAngle = boatWindAngle - mainsailBuffer * MainsailAngleMul;
			float mainsailWindAngleAbs = Mathf.Abs(mainsailWindAngle);
			if (mainsailWindAngleAbs < 180f - NoGoAngleThreshold)
			{
				if (mainsailWindAngleAbs < 90f)
				{
					// mainsailPercent = MainsailReachingCurve.Evaluate((mainsailWindAngleAbs - 90f) / 90f);
					mainsailPercent = MainsailReachingCurve.Evaluate(mainsailWindAngleAbs / 90f);
				}
				else
				{
					// mainsailPercent = MainsailBeatingCurve.Evaluate((mainsailWindAngleAbs - NoGoAngleThreshold) / (90f - NoGoAngleThreshold));
					mainsailPercent = MainsailBeatingCurve.Evaluate(1f - ((mainsailWindAngleAbs - 90f) / (90f - NoGoAngleThreshold)));
				}
			}

			mainsailPercent = Mathf.Max(mainsailPercent, MinThrustPercent);

			// float foresailWindAngle = boatWindAngle + foresailBuffer * ForesailAngleMul;
			// float foresailWindAngleAbs = Mathf.Abs(foresailWindAngle);
			// if (foresailWindAngleAbs > NoGoAngleThreshold) {
			// 	if (foresailWindAngleAbs > 90f) {
			// 		ForesailPercent = ForesailReachingCurve.Evaluate((boatWindAngleAbs - 90f) / 90f);
			// 	} else {
			// 		ForesailPercent = ForesailBeatingCurve.Evaluate((boatWindAngleAbs - NoGoAngleThreshold) / (90f - NoGoAngleThreshold));
			// 	}
			// }
			// IDEA: give no/less foresail force when directly downwind if both sails are turned to the same side, because the mainsail blocks the wind to the foresail

			// TODO: automatically flip sails between port and starboard

			float mainsailSpeed = ThrustAmount * mainsailPercent;
			//Mathf.Clamp01(mainsailValue);
			// float foresailSpeed = ThrustAmount * ForesailPercent;//Mathf.Clamp01(mainsailValue);

			// boatRb.AddForceAtPosition(transform.forward * foresailSpeed * FrontRearForceRatio, transform.TransformPoint(FrontForcePoint), ForceMode.Force);
			// boatRb.AddForceAtPosition(Rudder.transform.forward * mainsailSpeed * (1f - FrontRearForceRatio), transform.TransformPoint(RearForcePoint), ForceMode.Force);
			// boatRb.AddRelativeForce(Vector3.forward * windMagnitude * (foresailSpeed * FrontRearForceRatio + mainsailSpeed * (1f - FrontRearForceRatio)), ForceMode.Force);
			boatRb.AddRelativeForce(Vector3.forward * windMagnitude * mainsailSpeed, ForceMode.Force);

			boatRb.AddRelativeTorque(new Vector3(0, rudderBuffer * TurnAngularForce, 0), ForceMode.Force);

			if (boatRb.linearVelocity.sqrMagnitude > VelocityCap * VelocityCap)
			{
				boatRb.linearVelocity = boatRb.linearVelocity.normalized * VelocityCap;
			}

			WindDirEvent.Invoke(new Vector3(0, 0, -boatWindAngle));
			WindMagnitudeEvent.Invoke($"{windMagnitude.ToString("0.00")}, {mainsailPercent.ToString("0.00")}");
		}

		if (noGravitySources && lastGravitySource != null)
		{
			Vector3 delta = transform.position - lastGravitySource.position;
			Vector3 gravityDir = delta.normalized;
			float gravityPercent = GravityDropoff.Evaluate(delta.sqrMagnitude / GravityMaxDistanceSqr);
			boatRb.AddForce(gravityDir * GravityAmount * gravityPercent, ForceMode.Acceleration); // gravity
		}
		noGravitySources = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("WindZone") && other.TryGetComponent<WindZoneScript>(out WindZoneScript windZone))
		{
			WindDir = windZone.WindDir;
		}
	}

	private void OnTriggerStay(Collider other)
	{

		// NOTE: cannot have different gravity per source
		if (!boatRb || !other.CompareTag("GravitySource"))
		{
			return;
		}
		lastGravitySource = other.transform;
		noGravitySources = false;
		Vector3 delta = transform.position - other.transform.position;
		Vector3 gravityDir = delta.normalized;
		float gravityPercent = GravityDropoff.Evaluate(delta.sqrMagnitude / GravityMaxDistanceSqr);
		boatRb.AddForce(gravityDir * GravityAmount * gravityPercent, ForceMode.Acceleration); // gravity
	}

	private void OnCollisionEnter(Collision other)
	{
		// TODO: lose when hit by enemy cannonball
		// TODO: decide what happens on "game over" for a player
	}

	public void OnRudder(InputValue value)
	{
		rudderValue = value.Get<float>();
		RudderEvent.Invoke(rudderValue);
	}

	public void OnMainsail(InputValue value)
	{
		mainsailValue = value.Get<float>();
		// MainsailEvent.Invoke(mainsailValue);
	}

	public void OnForesail(InputValue value)
	{
		foresailValue = value.Get<float>();
		// ForesailEvent.Invoke(foresailValue);
	}

	public void OnTurnCannon(InputValue value)
	{
		turnCannonValue = value.Get<float>();
		// TurnCannonEvent.Invoke(turnCannonValue);
	}

	public void OnFire(InputValue value)
	{
		if (value.Get<float>() > 0f)
			Fire();
	}

	public void OnResetPress(InputValue value)
	{
		if (value.Get<float>() > 0f)
			ResetPress();
	}

	public void OnToggleSails(InputValue value)
	{
		if (value.Get<float>() > 0f)
			SailsOn = !SailsOn;
	}

	public void SetRudder(float value)
	{
		rudderValue = value;
	}
	public void SetMainsail(float value)
	{
		mainsailBuffer = value;
	}
	public void SetForesail(float value)
	{
		foresailBuffer = value;
	}
	public void SetTurnCannon(float value)
	{
		turnCannonBuffer = value;
	}

	public void Fire()
	{

		if (!Cannon || !CannonBallObject || !CannonBallSpawnPoint)
		{
			Debug.LogWarning("Cannon object(s) missing");
			return;
		}

		// IDEA: projectile buffer instead of creating and destroying every projectile
		// FIXME: cannonballs not spawning at spawnpoint under some conditions
		GameObject cannonBall = Instantiate(CannonBallObject, CannonBallSpawnPoint.transform.position, Cannon.transform.rotation, transform.parent);
		cannonBall.transform.position = CannonBallSpawnPoint.transform.position;
		if (cannonBall.TryGetComponent<CannonBallScript>(out CannonBallScript cannonBallScript))
		{
			cannonBallScript.SetInit(WindDir, Cannon.transform.forward * CannonBallVelocity);
		}
		else
		{
			Debug.LogWarning("Spawned object is not a cannonball");
		}

	}

	public void ResetPress()
	{
		boatRb.linearVelocity = Vector3.zero;
		if (ResetObject)
			transform.position = initPos + ResetObject.transform.position;
		else
			transform.position = initPos;
	}

}
