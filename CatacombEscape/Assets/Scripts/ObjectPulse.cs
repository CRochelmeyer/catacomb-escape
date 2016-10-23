using UnityEngine;
using System.Collections;

public class ObjectPulse : MonoBehaviour
{
	private float timer = 0.0f;
	public float pulsingSpeed = 0.18f;
	public float pulsingAmount = 0.2f;
	public float midpoint = 0.0f;
	private float waveslice;
	public float horizontal;
	public float vertical;
	private float totalAxes;
	private float scaleChange;

	[Header ("Axes to pulse along")]
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
			timer = timer + pulsingSpeed;

			if (timer > Mathf.PI * 2) 
			{
				timer = timer - (Mathf.PI * 2);
			}
		}

		if (waveslice != 0) 
		{
			scaleChange = waveslice * pulsingAmount;
			totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
			scaleChange = totalAxes * scaleChange;

			// One Axis
			if (X == true && Y == false && Z == false)
			{
				transform.localScale = new Vector3 (midpoint + scaleChange, transform.localScale.y, transform.localScale.z);
			}

			if (X == false && Y == true && Z == false)
			{
				transform.localScale = new Vector3 (transform.localScale.x, midpoint + scaleChange, transform.localScale.z);
			}

			if (X == false && Y == false && Z == true)
			{
				transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, midpoint + scaleChange);
			}

			// Two Axis
			if (X == true && Y == true && Z == false)
			{
				transform.localScale = new Vector3 (midpoint + scaleChange, midpoint + scaleChange, transform.localScale.z);
			}

			if (X == false && Y == true && Z == true)
			{
				transform.localScale = new Vector3 (transform.localScale.x, midpoint + scaleChange, midpoint + scaleChange);
			}

			if (X == true && Y == false && Z == true)
			{
				transform.localScale = new Vector3(midpoint + scaleChange, transform.localScale.y, midpoint + scaleChange);
			}

			// All Axis
			if (X == true && Y == true && Z == true)
			{
				transform.localScale = new Vector3(midpoint + scaleChange, midpoint + scaleChange, midpoint + scaleChange);
			}

			// Else No axis
			if (X == false && Y == false && Z == false)
			{
				transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}


		}

		else 
		{
			transform.localPosition = new Vector3 (midpoint, midpoint, midpoint);
		}
	}
}
