using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum JOYSTICK_BTN_TYPE
{
	GAS,
	BACK,
	JUMP,
	BRAKE,
	BOOST,
	LEFT,
	RIGHT,
}

public class VehicleController : MonoBehaviour {
	public Vehicle m_vehicle;

	private float m_horizontal = 0;
	private float m_vertical = 0;
	private float m_steerIncrement = 3f;
	private float m_forceIncrement = 3f;
	private Dictionary<JOYSTICK_BTN_TYPE,bool> m_joyStickState = new Dictionary<JOYSTICK_BTN_TYPE, bool> ();

	void Start()
	{
		foreach( JOYSTICK_BTN_TYPE val in System.Enum.GetValues(typeof(JOYSTICK_BTN_TYPE)))
		{
			m_joyStickState [val] = false;
		}
	}

	void Update () {
		UpdateJoySitckState ();
		m_vehicle.SetInput( m_horizontal, m_vertical );
	}
		

	#region JOYSTICK
	public void OnJoystickEnter(string btnType)
	{
		OnJoyStickStateChange (btnType, true);
	}

	public void OnJoystickExit(string btnType)
	{
		OnJoyStickStateChange (btnType, false);
	}

	private void OnJoyStickStateChange( string btnType, bool state )
	{
		JOYSTICK_BTN_TYPE type = (JOYSTICK_BTN_TYPE)System.Enum.Parse (typeof(JOYSTICK_BTN_TYPE), btnType);
		if (null == type) {
			Debug.LogError ("no the button type = " + btnType);
			Debug.Break ();
		}
		m_joyStickState [type] = state;
	}

	private void UpdateJoySitckState()
	{
		#if UNITY_EDITOR
			m_horizontal = Input.GetAxis ("Horizontal");
			m_vertical = Input.GetAxis ("Vertical");
		#else

		float m_forceIncrement = Time.deltaTime* m_steerIncrement;
		if (m_joyStickState [JOYSTICK_BTN_TYPE.GAS]) {
			m_vertical += m_forceIncrement;
			if (m_vertical > 1)
				m_vertical = 1;
		} else if ((m_vertical - float.Epsilon) > 0){
			m_vertical -= m_forceIncrement;
		} 

		if (m_joyStickState [JOYSTICK_BTN_TYPE.BACK]) {
			m_vertical -= m_forceIncrement;
			if (m_vertical < -1)
				m_vertical = -1;
		} else if ((m_vertical + float.Epsilon) < 0){
			m_vertical += m_forceIncrement;
		}  

		float steerChange = Time.deltaTime* m_steerIncrement;
		if (m_joyStickState [JOYSTICK_BTN_TYPE.RIGHT]) {
			m_horizontal += steerChange;
			if (m_horizontal > 1)
				m_horizontal = 1;
		} else if ((m_horizontal - float.Epsilon) > 0){
			m_horizontal -= steerChange;
		} 

		if (m_joyStickState [JOYSTICK_BTN_TYPE.LEFT]) {
			m_horizontal -= steerChange;
			if (m_horizontal < -1)
				m_horizontal = -1;
		} else if ((m_horizontal + float.Epsilon) < 0){
			m_horizontal += steerChange;
		}
		#endif

	}
	#endregion
}
