using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
	public GameObject[] gold;
	public GameObject[] silver;

	// Coins arrays: index0 is gold coin, index 1-10 are silver coins
	public Animator animatorGold1;
	public Animator animatorGold2;
	public Animator animatorGold3;
	public Animator animatorGold4;
	public Animator animatorGold5;
	public Animator animatorGold6;
	public Animator animatorGold7;
	public Animator animatorGold8;
	public Animator animatorGold9;
	public Animator animatorGold10;

	private GameLogic gameLogic;
	private int previousStam = 0;

	/// <summary>
	/// Updates the display of coins in the UI
	/// </summary>
	/// <param name="coinAmount">Coin amount</param>
	public void UpdateCoins (int newStamina)
	{
		int oldTens;
		int oldOnes;
		int newTens;
		int newOnes;
		int tempStam = previousStam;

		while (tempStam != newStamina)
		{
			oldTens = (int) Mathf.Floor (tempStam / 10);
			oldOnes = tempStam % 10;

			if (previousStam > newStamina) // Decrease the number of coins
			{
				newTens = (int) Mathf.Floor ((tempStam - 1) / 10);
				newOnes = (tempStam - 1) % 10;

				if (oldTens == newTens && newOnes != 0) // ie. old stamina was 37 and new stamina is 36
				{
					SetCoinActive (silver, oldOnes, false);
				}
				else if (oldTens - newTens == 1 && oldOnes == 0) // Old stamina was 40 and new stamina is 39
				{
					oldOnes += 10;
					SetCoinActive (silver, oldOnes, false);
				}
				else // Old stamina was 31 and new stamina is 30
				{
					SetCoinActive (silver, oldOnes, false);

					if (newTens > 0) // ie. if new stamina is not 00
					{
						gold [newTens - 1].SetActive (false);
						SwitchSilverActive (silver, true);
						// play gold coin to silver animation
					}
					else break;
				}
				tempStam--;
			}
			else if (previousStam < newStamina) // Increase the number of coins
			{
				newTens = (int) Mathf.Floor ((tempStam + 1) / 10);
				newOnes = (tempStam + 1) % 10;

				if (oldTens == newTens && oldOnes != 0) // ie. old stamina was 36 and new stamina is 37
				{
					SetCoinActive (silver, newOnes, true);
				}
				else if (oldTens - newTens == -1 && newOnes == 0) // Old stamina was 39 and new stamina is 40
				{
					newOnes += 10;
					SetCoinActive (silver, newOnes, true);
				}
				else // Old stamina was 30 and new stamina is 31
				{
					if (oldTens > 0)
					{
						SetCoinActive (gold, oldTens, true);
						SwitchSilverActive (silver, false);
						// play silver coin to gold animation
					}

					SetCoinActive (silver, newOnes, true);
				}
				tempStam++;
			}
		}
		previousStam = newStamina;
	}

	private void SwitchSilverActive (GameObject[] array, bool status)
	{
		for (int i = 0; i < 10; i++)
		{
			array[i].SetActive (status);
		}
	}

	private void SetCoinActive (GameObject[] tensArray, int index, bool active)
	{
		tensArray [index-1].SetActive (active);
		// play particle effect
		// play sound
	}
}

