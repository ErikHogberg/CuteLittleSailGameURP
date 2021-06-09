using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallScript : MonoBehaviour {

	public float GravityAmount = 9.81f;
	public AnimationCurve GravityDropoff;
	public float GravityMaxDistanceSqr = 9f;
	public Vector3 WindDir = Vector3.up;

	public float DespawnTime = 10f;
	Rigidbody ballRb;

	// TODO: player ownership

	public void SetInit(Vector3 windDir, Vector3 velocity) {
		WindDir = windDir;
		if (!ballRb)
			ballRb = GetComponent<Rigidbody>();
		ballRb.velocity = velocity;
		// IDEA: ball spin
	}

	void Start() {
		if (!ballRb)
			ballRb = GetComponent<Rigidbody>();
	}

	private void Update() {
		DespawnTime -= Time.deltaTime;
		if (DespawnTime < 0)
			Destroy(gameObject);

	}

	private void OnTriggerStay(Collider other) {

		// NOTE: cannot have different gravity per source
		if (other.CompareTag("GravitySource")) {
			Vector3 delta = transform.position - other.transform.position;
			Vector3 gravityDir = delta.normalized;
			float gravityPercent = GravityDropoff.Evaluate(delta.sqrMagnitude / GravityMaxDistanceSqr);
			ballRb.AddForce(gravityDir * GravityAmount * gravityPercent, ForceMode.Acceleration); // gravity
		}
	}
}
