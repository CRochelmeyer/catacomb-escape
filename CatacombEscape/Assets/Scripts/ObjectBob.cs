// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class ObjectBob : MonoBehaviour
{
	private float timer = 0.0f;
	public float bobbingSpeed = 0.18f;
	public float bobbingAmount = 0.2f;
	public float midpoint = 0.0f;
	private float waveslice;
	public float horizontal;
	public float vertical;
	private float totalAxes;
	private float translateChange;

	[Header ("Axes to bob")]
	public bool X;
	public bool Y;
	public bool Z;

	void  FixedUpdate ()
	{
		waveslice = 0.0f;

		//horizontal = Input.GetAxis("Horizontal");
		horizontal = 1;
		vertical = 1;

		//vertical = Input.GetAxis("Vertical");

		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) 
		{
			timer = 0.0f;
		}
		else 
		{
			waveslice = Mathf.Sin(timer);
			timer = timer + bobbingSpeed;

			if (timer > Mathf.PI * 2) 
			{
				timer = timer - (Mathf.PI * 2);
			}
		}

		if (waveslice != 0) 
		{
			translateChange = waveslice * bobbingAmount;
			totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;

			// One Axis
			if (X == true && Y == false && Z == false)
			{
				transform.localPosition = new Vector3 (midpoint + translateChange, transform.localPosition.y, transform.localPosition.z);
			}

			if (X == false && Y == true && Z == false)
			{
				transform.localPosition = new Vector3 (transform.localPosition.x, midpoint + translateChange, transform.localPosition.z);
			}

			if (X == false && Y == false && Z == true)
			{
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, midpoint + translateChange);
			}

			// Two Axis
			if (X == true && Y == true && Z == false)
			{
				transform.localPosition = new Vector3 (midpoint + translateChange, midpoint + translateChange, transform.localPosition.z);
			}

			if (X == false && Y == true && Z == true)
			{
				transform.localPosition = new Vector3 (transform.localPosition.x, midpoint + translateChange, midpoint + translateChange);
			}

			if (X == true && Y == false && Z == true)
			{
				transform.localPosition = new Vector3(midpoint + translateChange, transform.localPosition.y, midpoint + translateChange);
			}

			// All Axis
			if (X == true && Y == true && Z == true)
			{
				transform.localPosition = new Vector3(midpoint + translateChange, midpoint + translateChange, midpoint + translateChange);
			}

			// Else No axis
			if (X == false && Y == false && Z == false)
			{
				transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
			}


		}

		else 
		{
			transform.localPosition = new Vector3 (midpoint, midpoint, midpoint);
		}
	}
}
