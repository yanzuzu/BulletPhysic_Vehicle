using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputCtrl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	#if UNITY_EDITOR
	#else
		// @leo: switch to use TouchInputModule instead StandaloneInputModule in non-editor mode.
		//       because in 5.3.x the StandaloneInputModule sometimes will trigger PointerEnter after PointerExit.
		//		 this occur the GAS/BOOST/REVERSE/JUMP/BRAKE get stuck
		#if !UNITY_STANDALONE_OSX
		GameObject.Destroy(gameObject.GetComponent<StandaloneInputModule>());
		gameObject.AddComponent<TouchInputModule>();
		#endif
	#endif
	}
}
