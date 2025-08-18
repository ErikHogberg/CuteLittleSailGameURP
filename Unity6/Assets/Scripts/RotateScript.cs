using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour {
	public Vector3 Axis = Vector3.up;
	public float Speed;
	public bool Local = true;
	public Rigidbody Rigidbody;

	void Update() {

		if (Rigidbody)
			return;

		if (Local)
			transform.localRotation *= Quaternion.AngleAxis(Speed * Time.deltaTime, Axis);
		else
			transform.rotation *= Quaternion.AngleAxis(Speed * Time.deltaTime, Axis);
	}

	private void FixedUpdate() {
		if (!Rigidbody)
			return;

		if (Local)
			//FiXME: not using local axis
			Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.AngleAxis(Speed * Time.deltaTime, Axis));
		else
            // FIXME: does not impart friction on collidiing objects
			Rigidbody.rotation *= Quaternion.AngleAxis(Speed * Time.deltaTime, Axis);
		// 	transform.rotation *= Quaternion.AngleAxis(Speed * Time.deltaTime, Axis);
	}
}
