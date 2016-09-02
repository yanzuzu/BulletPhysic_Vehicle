using System;
using System.Collections.Generic;
using System.Diagnostics;
using BulletSharp;
using BulletSharp.Math;
using BulletUnity;
using UnityEngine;

public class Test : MonoBehaviour, IAction{
	public BRigidBody rigidBody;
	private BulletSharp.RigidBody m_bulletRigid;

	void Start()
	{
		m_bulletRigid = (BulletSharp.RigidBody)rigidBody.GetCollisionObject ();
	}

	public void UpdateAction(CollisionWorld collisionWorld, float deltaTimeStep)
	{
		rigidBody.AddImpulse (new UnityEngine.Vector3 (0.05f, 0, 0));
		//m_bulletRigid.ApplyForce(new BulletSharp.Math.Vector3(100,0,0),BulletSharp.Math.Vector3.Zero);
	}

	public void DebugDraw(IDebugDraw debugDrawer)
	{
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			rigidBody.AddForce (new UnityEngine.Vector3 (1000, 0,0));
		}
	}
}
