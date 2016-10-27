using UnityEngine;
using System.Collections;

public class PanelFader : MonoBehaviour
{
	public Texture2D fadeTexture;		// The texture that will overlay the screen. This can be a black image or a loading graphic
	public float fadeSpeed = 0.8f;				// The fading speed

	private int drawDepth = -1000;		// The texture's order in the draw hierarchy: a low number means it renders on top
	private float alpha = 1.0f;			// The texture's alpha value between 0 and 1
	private int fadeDir = -1;			// The direction to fade: in = -1, or out = 1
	private bool fading = false;

	void OnEnable ()
	{
		fading = false;
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, 0);
	}

	void OnGUI ()
	{
		if (fading)
		{
			alpha += fadeDir * fadeSpeed * Time.deltaTime;

			alpha = Mathf.Clamp01 (alpha);

			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			GUI.depth = drawDepth;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeTexture);
		}
	}

	public float BeginFade (int direction)
	{
		fading = true;
		fadeDir = direction;
		return (fadeSpeed);
	}
}
