using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using VehicleDemo;
using BulletSharp.Math;

public class VehicleControl : MonoBehaviour {
	#region WORLD
	public BulletUnity.BPhysicsWorld World;

	private CollisionConfiguration CollisionConf;
	private CollisionDispatcher Dispatcher;
	private BroadphaseInterface Broadphase;
	private ConstraintSolver Solver;
	#endregion

	public BulletUnity.BRigidBody carRigid;

	#region Vehicle
	private CustomVehicle vehicle;

	const int rightIndex = 0;
	const int upIndex = 1;
	const int forwardIndex = 2;

	const float maxEngineForce = 2000.0f;//this should be engine/velocity dependent
	const float maxBreakingForce = 100.0f;

	float gEngineForce = 0.0f;
	float gBreakingForce = 0.0f;

	float gVehicleSteering = 0.0f;
	const float steeringIncrement = 1.0f;
	const float steeringClamp = 0.3f;
	public const float wheelRadius = 1.2f;
	public const float wheelWidth = 0.4f;
	const float wheelFriction = 1000;//BT_LARGE_FLOAT;
	const float suspensionStiffness = 20.0f;
	const float suspensionDamping = 2.3f;
	const float suspensionCompression = 4.4f;
	const float rollInfluence = 0.1f;//1.0f;

	const float suspensionRestLength = 0.6f;
	const float CUBE_HALF_EXTENTS = 1;

	BulletSharp.Math.Vector3 wheelDirectionCS0 = new BulletSharp.Math.Vector3(0, -1, 0);
	BulletSharp.Math.Vector3 wheelAxleCS = new BulletSharp.Math.Vector3(-1, 0, 0);

	#endregion
	// Use this for initialization
	void Start () {
		//InitWorld ();
		InitVehicle ();
	}
		
	private void InitWorld()
	{
//		CollisionConf = new DefaultCollisionConfiguration();
//		Dispatcher = new CollisionDispatcher(CollisionConf);
//		Solver = new SequentialImpulseConstraintSolver();
//
//		BulletSharp.Math.Vector3 worldMin = new BulletSharp.Math.Vector3(-10000, -10000, -10000);
//		BulletSharp.Math.Vector3 worldMax = new BulletSharp.Math.Vector3(10000, 10000, 10000);
//		Broadphase = new AxisSweep3(worldMin, worldMax);
//
//		World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf);
	}

	private void InitVehicle()
	{
		// create vehicle
		VehicleTuning tuning = new VehicleTuning();
		IVehicleRaycaster vehicleRayCaster = new DefaultVehicleRaycaster((DynamicsWorld) World.world);
		BulletSharp.RigidBody carChassis = (BulletSharp.RigidBody)carRigid.GetCollisionObject ();
		vehicle = new CustomVehicle(tuning, carChassis, vehicleRayCaster);

		carChassis.ActivationState = ActivationState.DisableDeactivation;
		World.AddAction(vehicle);


		const float connectionHeight = 1.2f;
		bool isFrontWheel = true;

		// choose coordinate system
		vehicle.SetCoordinateSystem(rightIndex, upIndex, forwardIndex);

		BulletSharp.Math.Vector3 connectionPointCS0 = new BulletSharp.Math.Vector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius);
		vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

		connectionPointCS0 = new BulletSharp.Math.Vector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius);
		vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

		isFrontWheel = false;
		connectionPointCS0 = new BulletSharp.Math.Vector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius);
		vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

		connectionPointCS0 = new BulletSharp.Math.Vector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius);
		vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);


		for (int i = 0; i < vehicle.NumWheels; i++)
		{
			WheelInfo wheel = vehicle.GetWheelInfo(i);
			wheel.SuspensionStiffness = suspensionStiffness;
			wheel.WheelsDampingRelaxation = suspensionDamping;
			wheel.WheelsDampingCompression = suspensionCompression;
			wheel.FrictionSlip = wheelFriction;
			wheel.RollInfluence = rollInfluence;
		}
//		Matrix vehicleTr;
//		vehicleTr = Matrix.Translation(0, -2, 0);
//		vehicle.RigidBody.WorldTransform = vehicleTr;
	}

	void FixedUpdate()
	{
		UpdateVehiclePhysic ();
	}

	// Update is called once per frame
	void Update () {
		HandleInput ();
		//UnityEngine.Debug.Log (carRigid.velocity);
	}


	private void UpdateVehiclePhysic()
	{
		gEngineForce *= (1.0f - Time.fixedDeltaTime);

		vehicle.ApplyEngineForce(gEngineForce, 2);
		vehicle.SetBrake(gBreakingForce, 2);
		vehicle.ApplyEngineForce(gEngineForce, 3);
		vehicle.SetBrake(gBreakingForce, 3);
		//UnityEngine.Debug.Log (gVehicleSteering);
		vehicle.SetSteeringValue(gVehicleSteering, 0);
		vehicle.SetSteeringValue(gVehicleSteering, 1);
	}

	private void HandleInput()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			gVehicleSteering += Time.deltaTime * steeringIncrement;
			if (gVehicleSteering > steeringClamp)
				gVehicleSteering = steeringClamp;
		}
		else if ((gVehicleSteering - float.Epsilon) > 0)
		{
			gVehicleSteering -= Time.deltaTime * steeringIncrement;
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			gVehicleSteering -= Time.deltaTime * steeringIncrement;
			if (gVehicleSteering < -steeringClamp)
				gVehicleSteering = -steeringClamp;
		}
		else if ((gVehicleSteering + float.Epsilon) < 0)
		{
			gVehicleSteering += Time.deltaTime * steeringIncrement;
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			gEngineForce = maxEngineForce;
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			gEngineForce = -maxEngineForce;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			gBreakingForce = maxBreakingForce;
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			gBreakingForce = 0;
		}
	}
}
