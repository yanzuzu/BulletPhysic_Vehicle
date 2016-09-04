using UnityEngine;
using System.Collections;
using BulletUnity;

public class Vehicle : MonoBehaviour {
	#region private memeber
	// car physics calculations/ input stuff
	private Vector3 accel;
	public float throttle;
	private float deadZone = 0.001f;
	private Vector3 myRight;
	private Vector3 velo;
	private Vector3 flatVelo;
	private Vector3 relativeVelocity;
	private Vector3 dir;
	private Vector3 flatDir;
	private Vector3 carUp;
	private Transform carTransform;
	private BRigidBody carRigidbody;
	private Vector3 engineForce;

	private Vector3 turnVec;
	private Vector3 imp;
	private float rev;
	private float actualTurn;
	private float carMass;
	private Transform[] wheelTransform = new Transform[4];
	public float actualGrip;
	public float horizontal;
	private float maxSpeedToTurn = 0.2f;

	// the physical transforms for the car's wheels
	public Transform frontLeftWheel;
	public Transform frontRightWheel;
	public Transform rearLeftWheel;
	public Transform rearRightWheel;

	// these transform parents will allow wheels to turn for steering turn form acceleration turning
	public Transform LFWheelTransform;
	public Transform RFWheelTransform;

	// car physics adjustments
	public float power = 300f;
	public float maxSpeed = 50f;
	public float carGrip = 70f;
	public float turnSpeed = 3.0f;

	private float slideSpeed;
	public float mySpeed;

	private Vector3 carRight;
	private Vector3 carFwd;
	private Vector3 tempVEC;
	private Vector3 tempLocalEuler;

	private Vector3 rotationAmount;

	private float deviceAccelerometerSensitivity = 2f;

	public float turnVecValue = 100f;

	public float jumpForce = 2000f;
	#endregion
	
	void Start()
	{
		Initialize ();
	}

	void Initialize()
	{
		carTransform = transform;
		carRigidbody = GetComponent<BRigidBody> ();
		carUp = carTransform.up;
		carMass = carRigidbody.mass;
		carFwd = Vector3.forward;
		carRight = Vector3.right;

		setUpWheels ();
	}

	void Update()
	{
		//checkInput ();
		carPhysicsUpdate ();
	}

	void LateUpdate()
	{
		rotateVisualWheels ();
	}

	void setUpWheels()
	{
		if ((null == frontLeftWheel || null == frontRightWheel ||
		    null == rearLeftWheel || null == rearRightWheel)) {
			Debug.LogError ("one of the wheel is null");
			Debug.Break ();
		} else {
			wheelTransform [0] = frontLeftWheel;
			wheelTransform [1] = rearLeftWheel;
			wheelTransform [2] = frontRightWheel;
			wheelTransform [3] = rearRightWheel;
		}
	}

	void rotateVisualWheels()
	{
		tempLocalEuler = LFWheelTransform.localEulerAngles;
		tempLocalEuler.y = horizontal * 30;
		// front wheels visual rotation while steering the car
		LFWheelTransform.localEulerAngles = tempLocalEuler;

		tempLocalEuler = RFWheelTransform.localEulerAngles;
		tempLocalEuler.y = horizontal * 30;
		RFWheelTransform.localEulerAngles = tempLocalEuler;

		rotationAmount = carRight * (relativeVelocity.z * 1.6f * Time.deltaTime * Mathf.Rad2Deg);

		for (int i = 0; i < wheelTransform.Length; i++) {
			wheelTransform [i].Rotate (rotationAmount);
		}
	}

	public void SetInput(float pHorizontal, float pThrottle)
	{
		horizontal = pHorizontal;
		throttle = pThrottle;
	}

	void checkInput()
	{
		horizontal = Input.GetAxis ("Horizontal");
		throttle = Input.GetAxis ("Vertical");
	}

	void carPhysicsUpdate()
	{
		myRight = carTransform.right;
		// find our velocity
		velo = carRigidbody.velocity;

		tempVEC.x = velo.x;
		tempVEC.y = 0;
		tempVEC.z = velo.z;

		// figure out velocity without y movement - our flat velocity
		flatVelo = tempVEC;

		dir = transform.TransformDirection (carFwd);

		tempVEC.x = dir.x;
		tempVEC.y = 0;
		tempVEC.z = dir.z;

		flatDir = Vector3.Normalize (tempVEC);

		relativeVelocity = carTransform.InverseTransformDirection (flatVelo);

		slideSpeed = Vector3.Dot (myRight, flatVelo);

		mySpeed = flatVelo.magnitude;

		rev = Mathf.Sign (Vector3.Dot (flatVelo, flatDir));

		engineForce = (flatDir * (power * throttle) * carMass);

		actualTurn = horizontal;

		if (rev < 0.1f) {
			actualTurn =- actualTurn;
		}

		turnVec = (((carUp * turnSpeed) * actualTurn) * carMass) * turnVecValue;

		actualGrip = Mathf.Lerp (100, carGrip, mySpeed * 0.02f);
		imp = myRight * (-slideSpeed * carMass * actualGrip);
	}

	void slowVelocity()
	{
		carRigidbody.AddForce (-flatVelo * 0.8f);
	}

	void FixedUpdate()
	{
		if (mySpeed < maxSpeed) {
			carRigidbody.AddImpulse (engineForce * Time.fixedDeltaTime);
		}

		if (mySpeed > maxSpeedToTurn) {
			carRigidbody.AddTorque (turnVec * Time.fixedDeltaTime);
		} else if (mySpeed < maxSpeedToTurn) {
			return;
		}

		carRigidbody.AddImpulse (imp * Time.fixedDeltaTime);
	}

	public void Jump()
	{
		carRigidbody.AddImpulse (transform.up * jumpForce);
	}
		
}
