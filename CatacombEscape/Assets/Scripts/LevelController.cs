using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

    // // // // // //
    //  Variables  //
    // // // // // //
    private int level;
    private bool nextLevel = false;
    private GameLogic gameLogic;

    void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameLogic>();
    }

    public int GetLevel
    {
        get { return level; }
    }

    public bool SetNextLevel
    {
        set { nextLevel = value; }
    }

    /// <summary>
    /// Destroys GameObjects in current level.
    /// Generation of next level occurs by using Awake(). This also recreates a lot of other objects. Perhaps this can be optimised better?
    /// </summary>

    /*
	public void NextLevel()
    {
        int rand = Random.Range(0, gameLogic.lvlCompClips.Length);
        gameLogic.audioSource.PlayOneShot(gameLogic.lvlCompClips[rand], 0.5f);

        gameLogic.emptyhand = true;
        level++;

        //initilaise tileBoard list a new
        tileBoard = new Tile[6, 5];

        // Destroy handtiles
        handTiles = GameObject.FindGameObjectsWithTag("handDrag");
        for (int i = 0; i < handTiles.Length; i++)
        {
            Destroy(handTiles[i]);
        }

        // Destroy eventTiles and calculate red tiles avoided
        GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
        for (int i = 0; i < eventTiles.Length; i++)
        {
            redavoid += redtiles - redstep;
            Destroy(eventTiles[i]);
        }

        // Destroy grid tiles
        for (int i = 0; i < gridPanels.Length; i++)
        {
            gridPanels[i].GetComponent<Image>().sprite = null;
            gridPanels[i].GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
        }

        // Generate the next level.
        InitGame(level);
    }

    */
}
