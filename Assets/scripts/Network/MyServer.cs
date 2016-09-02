using UnityEngine;
using System.Collections;

public class MyServer : MonoBehaviour {
	public static MyServer Instance;

	void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		Debug.Log ("MyServer Init");
	}
}
