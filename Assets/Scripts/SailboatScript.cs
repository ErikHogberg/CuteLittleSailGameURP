using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SailboatScript : MonoBehaviour {

	public float GravityAmount = 9.81f;
	public AnimationCurve GravityDropoff;
	public float GravityMaxDistanceSqr = 9f;
	public Vector3 WindDir = Vector3.down;
	public float ThrustAmount = 1f;

	[Range(0, 1)]
	public float FrontRearForceRatio = .5f;
	public float VelocityCap = 1f;

	[Space]
	public GameObject Rudder;
	public GameObject Mainsail;
	public GameObject Foresail;
	public GameObject Cannon;
	public GameObject CannonBallSpawnPoint;
	public GameObject CannonBallObject;

	// TODO: draw projectile arc
	// IDEA: only on fire button down, fire on release?

	[Space]
	public float RudderSpeed = 1;
	public float RudderAngleMul = 1;
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

	[Space]
	public Vector3 WeightCenter;
	public Vector3 FrontForcePoint;
	public Vector3 RearForcePoint;

	[Space]
	public UnityEvent<float> RudderEvent;
	public UnityEvent<float> MainsailEvent;
	public UnityEvent<float> ForesailEvent;
	public UnityEvent<float> TurnCannonEvent;
	public UnityEvent<Vector3> WindDirEvent;

	Vector3 initPos;

	void Start() {
		foresailRot = Foresail.transform.localRotation;
		boatRb = GetComponent<Rigidbody>();
		boatRb.centerOfMass = WeightCenter;

		RudderEvent.Invoke(rudderValue);
		MainsailEvent.Invoke(mainsailValue);
		ForesailEvent.Invoke(foresailValue);
		TurnCannonEvent.Invoke(turnCannonValue);

		initPos = transform.position;
	}

	void Update() {
		rudderBuffer = Mathf.MoveTowards(rudderBuffer, rudderValue, RudderSpeed * Time.deltaTime);

		// mainsailBuffer = Mathf.MoveTowards(mainsailBuffer, mainsail, MainsailBufferSpeed * Time.deltaTime);
		if (Mathf.Abs(mainsailValue) > 0) {
			mainsailBuffer += mainsailValue * MainsailBufferSpeed * Time.deltaTime;
			mainsailBuffer = Mathf.Clamp01(mainsailBuffer);
			MainsailEvent.Invoke(mainsailBuffer);
		}

		// foresailBuffer = Mathf.MoveTowards(foresailBuffer, foresail, ForesailBufferSpeed * Time.deltaTime);
		if (Mathf.Abs(foresailValue) > 0) {
			foresailBuffer += foresailValue * ForesailBufferSpeed * Time.deltaTime;
			foresailBuffer = Mathf.Clamp01(foresailBuffer);
			ForesailEvent.Invoke(foresailBuffer);
		}

		Rudder.transform.localRotation = Quaternion.AngleAxis(rudderBuffer * RudderAngleMul, Vector3.down);
		Mainsail.transform.localRotation = Quaternion.AngleAxis(mainsailBuffer * MainsailAngleMul, Vector3.forward);
		Foresail.transform.localRotation = Quaternion.AngleAxis(foresailBuffer * ForesailAngleMul, foresailRot * Vector3.forward) * foresailRot;


		if (Mathf.Abs(turnCannonValue) > 0) {
			turnCannonBuffer += turnCannonValue * CannonBufferSpeed * Time.deltaTime;
			TurnCannonEvent.Invoke(turnCannonBuffer);
		}
		Cannon.transform.localRotation = Quaternion.AngleAxis(turnCannonBuffer * CannonAngleMul, Vector3.up);
	}


	private void FixedUpdate() {

		// Vector3 gravityDir = (transform.position - GravityCenter.transform.position).normalized;

		// TODO: support for multiple gravity sources
		// IDEA: make planets pull all rigidbodies in range instead of vice versa
		// IDEA: gravity drop-off as distance from planet increases
		// boatRb.AddForce(gravityDir * GravityAmount, ForceMode.Acceleration); // gravity

		// thrust
		// TODO: calculate forward speed based on mainsail angle compared to wind
		// TODO: make wind magnitude affect forward speed/force
		// TODO: mainsail headwind angle forward force percentage calculation
		// TODO: mainsail tailwind angle forward force percentage calculation
		// TODO: foresail headwind angle forward force percentage calculation
		// TODO: foresail tailwind angle forward force percentage calculation
		// TODO: automatically flip sails between port and starboard

		float forwardSpeed = ThrustAmount * Mathf.Clamp01(mainsailValue);
		boatRb.AddForceAtPosition(transform.forward * forwardSpeed * FrontRearForceRatio, transform.TransformPoint(FrontForcePoint), ForceMode.Force);
		boatRb.AddForceAtPosition(Rudder.transform.forward * forwardSpeed * (1f - FrontRearForceRatio), transform.TransformPoint(RearForcePoint), ForceMode.Force);

		if (boatRb.velocity.sqrMagnitude > VelocityCap * VelocityCap) {
			boatRb.velocity = boatRb.velocity.normalized * VelocityCap;
		}

		// Vector3 boatWindFacing = Vector3.ProjectOnPlane(WindDir, transform.up);
		float boatWindAngle = Vector3.SignedAngle(transform.forward, WindDir, transform.up);
		WindDirEvent.Invoke(new Vector3(0, 0, -boatWindAngle));

	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("WindZone") && other.TryGetComponent<WindZoneScript>(out WindZoneScript windZone)) {
			WindDir = windZone.WindDir;
		}
	}

	private void OnTriggerStay(Collider other) {

		// NOTE: cannot have different gravity per source
		if (other.CompareTag("GravitySource")) {
			Vector3 delta = transform.position - other.transform.position;
			Vector3 gravityDir = delta.normalized;
			float gravityPercent = GravityDropoff.Evaluate(delta.sqrMagnitude / GravityMaxDistanceSqr);
			boatRb.AddForce(gravityDir * GravityAmount * gravityPercent, ForceMode.Acceleration); // gravity
		}
	}

	private void OnCollisionEnter(Collision other) {
		// TODO: lose when hit by enemy cannonball
		// TODO: decide what happens on "game over" for a player
	}

	public void OnRudder(InputValue value) {
		rudderValue = value.Get<float>();
		RudderEvent.Invoke(rudderValue);
	}

	public void OnMainsail(InputValue value) {
		mainsailValue = value.Get<float>();
		// MainsailEvent.Invoke(mainsailValue);
	}

	public void OnForesail(InputValue value) {
		foresailValue = value.Get<float>();
		// ForesailEvent.Invoke(foresailValue);
	}

	public void OnTurnCannon(InputValue value) {
		turnCannonValue = value.Get<float>();
		// TurnCannonEvent.Invoke(turnCannonValue);
	}

	public void OnFire(InputValue value) {
		if (value.Get<float>() > 0f)
			Fire();
	}

	public void OnResetPress(InputValue value) {
		if (value.Get<float>() > 0f)
			ResetPress();
	}

	public void SetRudder(float value) {
		rudderValue = value;
	}
	public void SetMainsail(float value) {
		mainsailBuffer = value;
	}
	public void SetForesail(float value) {
		foresailBuffer = value;
	}
	public void SetTurnCannon(float value) {
		turnCannonBuffer = value;
	}

	public void Fire() {

		if (!Cannon || !CannonBallObject || !CannonBallSpawnPoint) {
			Debug.LogWarning("Cannon object(s) missing");
			return;
		}

		// IDEA: projectile buffer instead of creating and destroying every projectile
		GameObject cannonBall = Instantiate(CannonBallObject, CannonBallSpawnPoint.transform.position, Cannon.transform.rotation);
		if (cannonBall.TryGetComponent<CannonBallScript>(out CannonBallScript cannonBallScript)) {
			cannonBallScript.SetInit(WindDir, Cannon.transform.forward * CannonBallVelocity);
		} else {
			Debug.LogWarning("Spawned object is not a cannonball");
		}

	}

	public void ResetPress(){
		boatRb.velocity = Vector3.zero;
		transform.position = initPos;
	}

}
