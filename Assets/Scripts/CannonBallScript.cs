using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallScript : MonoBehaviour {

	public GameObject GravityCenter;
	public float GravityAmount = 9.81f;
	public Vector3 WindDir = Vector3.up;

    public float DespawnTime = 10f;
	Rigidbody ballRb;

	public void SetInit(GameObject gravityCenter, Vector3 windDir, Vector3 velocity) {
		GravityCenter = gravityCenter;
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

	void FixedUpdate() {
		Vector3 gravityDir = (transform.position - GravityCenter.transform.position).normalized;

		ballRb.AddForce(gravityDir * GravityAmount, ForceMode.Acceleration); // gravity
	}

    private void Update() {
        DespawnTime -= Time.deltaTime;
        if(DespawnTime < 0){
            Destroy(gameObject);
        }
    }
}
