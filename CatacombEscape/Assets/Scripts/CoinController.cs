using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
	public Animator animatorGold1;
	public GameObject[] gold1;

	public Animator animatorGold2;
	public GameObject[] gold2;

	public Animator animatorGold3;
	public GameObject[] gold3;

	public Animator animatorGold4;
	public GameObject[] gold4;

	public Animator animatorGold5;
	public GameObject[] gold5;

	public Animator animatorGold6;
	public GameObject[] gold6;

	private GameLogic gameLogic;

	// Use this for initialization
	void Start ()
	{
		gameLogic = GameObject.FindObjectOfType<GameLogic> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
