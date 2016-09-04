using UnityEngine;
using System.Collections;
using BulletUnity;

public class GameControl : MonoBehaviour {
	public static GameControl Instance;

	public DebugUI m_debugUI;

	#region FPS
	private float m_minFps = 10000f;
	private float m_maxFps = 0;
	private int m_fpsCount = 0;
	private float m_totalFps = 0;
	#endregion
	void Awake()
	{
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
		Instance = this;
	}


	void Update()
	{
		m_fpsCount++;
		float fps = 1f / Time.deltaTime;
		if (m_fpsCount >= 120) {
			if (fps < m_minFps)
				m_minFps = fps;
			if (fps > m_maxFps)
				m_maxFps = fps;
		}
		m_totalFps += fps;
		m_debugUI.SetFpsText ( (int)(m_totalFps / m_fpsCount), (int)m_minFps, (int)m_maxFps);
	}

	void FixedUpdate()
	{
		
	}

	#region Network
	public void ToBeServer()
	{
		NetworkMatchControl.Instance.CreateRoom (OnCreateRoom);
	}

	private void OnCreateRoom()
	{
		MyServer.Instance.Init ();
	}

	public void ToBeClient()
	{
		if (NetworkMatchControl.Instance.AllRooms.Count == 0) {
			Debug.LogError ("no Room can get!!");
			return;
		}

		NetworkMatchControl.Instance.AddRoom (NetworkMatchControl.Instance.AllRooms[0], OnAddRoomOK);
	}

	private void OnAddRoomOK()
	{
		MyClient.Instance.Init ();
	}
	#endregion
}
