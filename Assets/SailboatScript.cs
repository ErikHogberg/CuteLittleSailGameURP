using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SailboatScript : MonoBehaviour {

	public GameObject Rudder;
	public GameObject Mainsail;
	public GameObject Foresail;

	[Space]
	public float RudderSpeed = 1;
	public float RudderAngleMul = 1;
	[Space]
	public float MainsailSpeed = 1;
	public float MainsailAngleMul = 1;
	[Space]
	public float ForesailSpeed = 1;
	public float ForesailAngleMul = 1;

	float rudder = 0;
	float rudderBuffer = 0;
	float mainsail = 0;
	float mainsailBuffer = 0;
	float foresail = 0;
	float foresailBuffer = 0;

	Vector3 foresailAxis;

	void Start() {
		foresailAxis = Foresail.transform.localRotation * Vector3.up;
	}

	void Update() {
		rudderBuffer = Mathf.MoveTowards(rudderBuffer, rudder, RudderSpeed * Time.deltaTime);
		mainsailBuffer = Mathf.MoveTowards(mainsailBuffer, mainsail, MainsailSpeed * Time.deltaTime);
		foresailBuffer = Mathf.MoveTowards(foresailBuffer, foresail, ForesailSpeed * Time.deltaTime);

		Rudder.transform.localRotation = Quaternion.AngleAxis(rudderBuffer * RudderAngleMul, Vector3.up);
		Mainsail.transform.localRotation = Quaternion.AngleAxis(mainsailBuffer * MainsailAngleMul, Vector3.forward);
		Foresail.transform.localRotation = Quaternion.AngleAxis(foresailBuffer * ForesailAngleMul, foresailAxis);

	}

	public void OnRudder(InputValue value) {
		rudder = value.Get<float>();
	}

	public void OnMainsail(InputValue value) {
		mainsail = value.Get<float>();
	}

	public void OnForesail(InputValue value) {
		foresail = value.Get<float>();
	}

}
