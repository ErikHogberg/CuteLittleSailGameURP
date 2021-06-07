using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SailboatScript : MonoBehaviour {

	public GameObject GravityCenter;
	public float GravityAmount = 9.81f;
	public float ThrustAmount = 1f;

	[Range(0, 1)]
	public float FrontRearForceRatio = .5f;
	public float VelocityCap = 1f;

	[Space]
	public GameObject Rudder;
	public GameObject Mainsail;
	public GameObject Foresail;

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
	public bool Rotate = false;

	float rudderValue = 0;
	float rudderBuffer = 0;
	float mainsailValue = 0;
	float mainsailBuffer = 0;
	float foresailValue = 0;
	float foresailBuffer = 0;

	Quaternion foresailRot;

	Rigidbody boatRb;

	float angleBuffer = 0;

	[Space]
	public Vector3 WeightCenter;

	public Vector3 FrontForcePoint;
	public Vector3 RearForcePoint;

	void Start() {
		foresailRot = Foresail.transform.localRotation;
		boatRb = GetComponent<Rigidbody>();
		boatRb.centerOfMass = WeightCenter;
	}

	void Update() {
		rudderBuffer = Mathf.MoveTowards(rudderBuffer, rudderValue, RudderSpeed * Time.deltaTime);

		// mainsailBuffer = Mathf.MoveTowards(mainsailBuffer, mainsail, MainsailBufferSpeed * Time.deltaTime);
		mainsailBuffer += mainsailValue * MainsailBufferSpeed * Time.deltaTime;
		mainsailBuffer = Mathf.Clamp01(mainsailBuffer);

		// foresailBuffer = Mathf.MoveTowards(foresailBuffer, foresail, ForesailBufferSpeed * Time.deltaTime);
		foresailBuffer += foresailValue * ForesailBufferSpeed * Time.deltaTime;
		foresailBuffer = Mathf.Clamp01(foresailBuffer);

		Rudder.transform.localRotation = Quaternion.AngleAxis(rudderBuffer * RudderAngleMul, Vector3.down);
		Mainsail.transform.localRotation = Quaternion.AngleAxis(mainsailBuffer * MainsailAngleMul, Vector3.forward);
		Foresail.transform.localRotation = Quaternion.AngleAxis(foresailBuffer * ForesailAngleMul, foresailRot * Vector3.forward) * foresailRot;

		Vector3 gravityDir = (transform.position - GravityCenter.transform.position).normalized;

		boatRb.AddForce(gravityDir * GravityAmount, ForceMode.Acceleration); // gravity

		// thrust
		// TODO: calculate forward speed based on mainsail angle compared to wind
		// TODO: wind direction
		float forwardSpeed = ThrustAmount * Mathf.Clamp01(mainsailValue);
		boatRb.AddForceAtPosition(transform.forward * forwardSpeed * FrontRearForceRatio, transform.TransformPoint(FrontForcePoint), ForceMode.Force);
		boatRb.AddForceAtPosition(Rudder.transform.forward * forwardSpeed * (1f - FrontRearForceRatio), transform.TransformPoint(RearForcePoint), ForceMode.Force);

		if (boatRb.velocity.sqrMagnitude > VelocityCap * VelocityCap) {
			boatRb.velocity = boatRb.velocity.normalized * VelocityCap;
		}

	}

	public void OnRudder(InputValue value) {
		rudderValue = value.Get<float>();
	}

	public void OnMainsail(InputValue value) {
		mainsailValue = value.Get<float>();
	}

	public void OnForesail(InputValue value) {
		foresailValue = value.Get<float>();
	}

	public void SetRudder(float value) {
		rudderValue = value;
	}
	public void SetMainsail(float value) {
		mainsailValue = value;
	}
	public void SetForesail(float value) {
		foresailValue = value;
	}

}
