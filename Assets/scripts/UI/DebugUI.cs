using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour {
	public Text m_FpsText;

	public void SetFpsText( int fps , int minFps, int maxFps)
	{
		//m_FpsText.text = string.Format ("FPS: {0} , Min: {1}, Max: {2}", fps.ToString (), minFps.ToString(), maxFps.ToString());
		m_FpsText.text = string.Format ("FPS: {0}", fps.ToString ());
	}
}
