using UnityEngine;
using System.Collections;

public class MyClient : MonoBehaviour {
	public static MyClient Instance;

	void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		Debug.Log ("MyClient Init");
	}
}
