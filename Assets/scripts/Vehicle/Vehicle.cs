using UnityEngine;
using System.Collections;
using BulletUnity;

public class Vehicle : MonoBehaviour {
	#region private memeber
	private BRigidBody m_rigid;
	#endregion
	
	void Start()
	{
		m_rigid = GetComponent<BRigidBody> ();
		if (null == m_rigid) {
			Debug.LogError ("no Bullet rigidBody can get");
		}
	}
		
	void FixedUpdate()
	{
	}
}
