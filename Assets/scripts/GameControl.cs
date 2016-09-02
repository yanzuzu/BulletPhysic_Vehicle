using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {
	public static GameControl Instance;

	void Awake()
	{
		Instance = this;
	}

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
}
