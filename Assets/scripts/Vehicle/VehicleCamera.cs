using UnityEngine;
using System.Collections;

public class VehicleCamera : MonoBehaviour {
	public Transform target;
	public float dist = 5f;
	public float height = 30f;
	private Transform m_cameraTrans;

	void Start()
	{
		m_cameraTrans = this.gameObject.transform;
	}

	// Update is called once per frame
	void LateUpdate () {
		if (null == target)
			return;
		m_cameraTrans.position = target.position;
		m_cameraTrans.position -= target.forward * dist;
		m_cameraTrans.position = new Vector3 (m_cameraTrans.position.x, height, m_cameraTrans.position.z);

		m_cameraTrans.LookAt (target);
	}
}
