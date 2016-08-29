using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    private bool faderRunning = false;

    /// <summary>
    /// Displays event panel UI. (Snakes and Scorpions)
    /// </summary>
    /// <param name="panel"></param>
	public void DisplayClickPanel(GameObject panel)
    {
        panel.SetActive(true);
        PlayerPrefs.SetString("Paused", "true");
        StartCoroutine(ClickToClose(panel));
    }

    /// <summary>
    /// Closes the pause panel UI.
    /// </summary>
    /// <param name="panel"></param>
    /// <returns></returns>
	public IEnumerator ClickToClose(GameObject panel)
    {
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }
        panel.SetActive(false);
        PlayerPrefs.SetString("Paused", "false");
    }

    /// <summary>
    /// Starts fade-in for stamina
    /// </summary>
    /// <param name="stamText"></param>
    /// <param name="newText"></param>
    /// <param name="time"></param>
    public void StartFade(Text stamText, string newText, float time)
    {
        // Prevent more than one instance
        if (faderRunning)
        {
            StopCoroutine("FadeStamPopup");
        }

        Color newColor = stamText.color;
        newColor.a = 1;
        stamText.color = newColor;
        stamText.text = newText;

        StartCoroutine(FadeStamPopup(stamText, time));
    }

    /// <summary>
    /// Fade in for stamina
    /// </summary>
    /// <param name="stamText"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator FadeStamPopup(Text stamText, float time)
    {
        faderRunning = true;
        float alpha = stamText.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            Color newColor = stamText.color;
            newColor.a = Mathf.Lerp(alpha, 0, t);
            stamText.color = newColor;
            yield return null;
        }
        faderRunning = false;
    }
}
