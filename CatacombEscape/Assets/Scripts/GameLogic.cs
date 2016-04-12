using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLogic : MonoBehaviour
{
    //sprite holders drag sprites via inspector
    public Sprite[] tileSprite;
    public Sprite[] gridSprite;
    public Image[] gridPanels; 
    //initiate a static instance of gamelogic to be used globally...
    public static GameLogic instance = null;

    private int level = 1;
    public void TestPassing(string pImageId, float px, float py)
    {
        Debug.Log("image id " + pImageId + " x y " + px + " " + py);
    }

    public void GenerateHand()
    {
        Debug.Log("generatingHand");
    }
    //awake called behind start
    void Awake()
    {
        //if instance is null create instance of this GameLogic 
        if (instance == null)
        {
            instance = this;
        }
        //else instance is not null but not this GameLogic destroy 
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        InitGame();
    }
    //init game method
    void InitGame()
    {
        //setup board with rng sprites
        generateBoard(level);
    }

    public void generateBoard(int level)
    {
        //simple alg for #of green/red tiles based on level
        //using int to truncate .decimals
        int green = (int) (level * 1.5F);
        Debug.Log("green " + green);
        int red = (int)(level * .5);
        Debug.Log("red " + red);

    }
    // Use this for initialization
    void Start()
    {
        Debug.Log("GameLogic start)");

    }

    // Update is called once per frame
    void Update()
    {

    }
}

