using UnityEngine;
using System.Collections;

public class ControllingCameraAspectScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			// Set the desired aspect ratio 10:16
			float targetAspect = 10.0f / 16.0f;

			// Determine the game window's current aspect ratio
			float windowAspect = (float) Screen.width / (float) Screen.height;

			// Current viewport height should be scaled by this amount
			float scaleHeight = windowAspect / targetAspect;

			// Obtain camera component so we can modify its viewport
			Camera camera = GetComponent <Camera> ();

			// If scaled height is less than current height, add letterbox
			if (scaleHeight < 1.0f)
			{
				Rect rect = camera.rect;

				rect.width = 1.0f;
				rect.height = scaleHeight;
				rect.x = 0;
				rect.y = (1.0f - scaleHeight); //  / 2.0f; Trying to get the display to sit right at the top

				camera.rect = rect;
			}else // Add pillarbox
			{
				float scaleWidth = 1.0f / scaleHeight;

				Rect rect = camera.rect;

				rect.width = 1.0f;
				rect.height = scaleWidth;
				rect.x = 0;
				rect.y = (1.0f - scaleWidth) / 2.0f;

				camera.rect = rect;
			}
		}
	}
}
