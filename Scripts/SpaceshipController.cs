using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour {

	public GameObject COM;
	[Range (0,0.2f)]
	public float inputDeadzone;

	public int maximumSpeed;
	public int maximumStrafeSpeed;

	public int strafeSpeed;
	public int speed;
	public int torqueStrength;
	public int tiltStrength;
	public float velocity;

	public bool tiltDumpers;
	public bool thrustersDumpers;

	public bool tiltDump;
	public bool thrustersDump;

	Rigidbody rb;

	int halfScreenWidth = Screen.width / 2;
	int halfScreenHeight = Screen.height / 2;

	//input
	float horizontalInput;
	float verticalInput;
	float torque;
	float horizontalTilt;
	float verticalTilt;
	float forwardInput;
	float mousePosXInput;
	float mousePosYInput;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.centerOfMass = COM.transform.position;
	}
	
	void FixedUpdate () {
		InputManager ();

		if (Input.GetKeyDown(KeyCode.Space)) {
			thrustersDumpers = !thrustersDumpers;
			Debug.Log ("Thrusters Dumpers: " + thrustersDumpers);
		}

		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			tiltDumpers = !tiltDumpers;
			Debug.Log ("Tilt Dumpers: " + tiltDumpers);
		}
		if (thrustersDump == true) {
			rb.AddForce (-rb.velocity,ForceMode.Force);
		} 
		if (tiltDump == true) {
			rb.AddTorque (-rb.angularVelocity,ForceMode.Force);
		} 

		if (horizontalTilt != 0 || verticalTilt != 0 || Mathf.Abs (mousePosYInput) > inputDeadzone || Mathf.Abs (mousePosXInput) > inputDeadzone) {
			//Tilt (mousePosXInput, mousePosYInput, verticalTilt, horizontalTilt);
			tiltDump = false;
		} else {
			tiltDump = tiltDumpers;
		}

		if (torque != 0 ) {
			Torque (torque);
			tiltDump = false;
		} else {
			tiltDump = tiltDumpers;
		}

		if (forwardInput !=0 || horizontalInput !=0 || verticalInput != 0) {
			Movement (horizontalInput, verticalInput, forwardInput);
			thrustersDump = false;
		} else {
			thrustersDump = thrustersDumpers;
		}
		ValidateSpeed ();
	}


	void ValidateSpeed(){
		velocity = rb.velocity.magnitude;
		if (rb.velocity.x > maximumStrafeSpeed) {
			rb.velocity = new Vector3(maximumStrafeSpeed,rb.velocity.y, rb.velocity.z);
		}else if (rb.velocity.x < -maximumStrafeSpeed) {
			rb.velocity = new Vector3(-maximumStrafeSpeed,rb.velocity.y, rb.velocity.z);
		}
		if (rb.velocity.y > maximumStrafeSpeed) {
			rb.velocity = new Vector3(rb.velocity.x,maximumStrafeSpeed, rb.velocity.z);
		}else if (rb.velocity.y < -maximumStrafeSpeed) {
			rb.velocity = new Vector3(rb.velocity.x,-maximumStrafeSpeed, rb.velocity.z);
		}
		if (rb.velocity.magnitude > maximumSpeed) {
			rb.velocity = rb.velocity.normalized * maximumSpeed;
			Debug.Log ("Maximum speed reached");
		}
	}

	float CalculateAngleBetweenInputAndVelocity(Vector3 input, Vector3 velocity){
		float angle = Vector3.Angle (input, velocity);
		return angle;
	}



	void InputManager(){
		horizontalInput = Input.GetAxis ("Horizontal");
		verticalInput = Input.GetAxis ("Vertical");
		torque = Input.GetAxis("Torque");
		horizontalTilt = Input.GetAxis ("HorizontalTilt");
		verticalTilt = Input.GetAxis ("VerticalTilt");
		forwardInput = Input.GetAxis ("Forward");

		Vector2 mouseInput = Input.mousePosition;
		mouseInput.x -= Screen.width/2;
		mouseInput.y -= Screen.height/2;

		mousePosXInput = mouseInput.x / (halfScreenWidth);
		mousePosYInput = mouseInput.y / (halfScreenHeight);
	}


	void Torque(float torque){
		if (Mathf.Abs (torque) > inputDeadzone) {
			rb.AddTorque (transform.forward * -torque * torqueStrength * Time.fixedDeltaTime, ForceMode.Force);
		}
	}


	void Tilt(float mousePosXInput, float mousePosYInput, float verticalTilt, float horizontalTilt){
		if (Mathf.Abs (mousePosXInput) > inputDeadzone) {
			rb.AddRelativeTorque (Vector3.up * mousePosXInput * tiltStrength * Time.fixedDeltaTime, ForceMode.Force);
		}
		if (Mathf.Abs (mousePosYInput) > inputDeadzone) {
			rb.AddRelativeTorque (Vector3.right * -mousePosYInput * tiltStrength * Time.fixedDeltaTime, ForceMode.Force);
		}

		if (Mathf.Abs (horizontalTilt) > inputDeadzone) {
			rb.AddRelativeTorque (Vector3.up * -horizontalTilt * tiltStrength * Time.fixedDeltaTime, ForceMode.Force);
		}
		if (Mathf.Abs (verticalTilt) > inputDeadzone) {
			rb.AddRelativeTorque (Vector3.right * verticalTilt * tiltStrength * Time.fixedDeltaTime, ForceMode.Force);
		}
	}


	void Movement(float horizontalInput, float verticalInput, float forwardInput){
		
		if (Mathf.Abs (horizontalInput) > inputDeadzone) {
			rb.AddRelativeForce (Vector3.right * horizontalInput * strafeSpeed * Time.fixedDeltaTime, ForceMode.Force);
		} 
		if (Mathf.Abs (verticalInput) > inputDeadzone) {
			rb.AddRelativeForce (Vector3.up * verticalInput * strafeSpeed* Time.fixedDeltaTime,ForceMode.Force);
		}
		if (Mathf.Abs (forwardInput) > inputDeadzone) {
			rb.AddRelativeForce (Vector3.forward * forwardInput * speed* Time.fixedDeltaTime,ForceMode.Force);
			if (CalculateAngleBetweenInputAndVelocity(transform.forward * forwardInput * speed* Time.fixedDeltaTime,rb.velocity) > 90) {
				rb.AddForce (-rb.velocity,ForceMode.Force);
				Debug.Log ("opposite force");
			}
		}
	}
}
