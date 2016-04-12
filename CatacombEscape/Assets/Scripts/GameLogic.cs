﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLogic : MonoBehaviour
{
    int interval = 1;
    float nextTime = 0;
    //sprite holders drag sprites via inspector
    public Sprite[] tileSprite;
    public Sprite[] gridSprite;
    public GameObject BtmPanel;
    public GameObject[] gridPanels;
    public GameObject[] handTiles;
    //initiate a static instance of gamelogic to be used globally...
    public static GameLogic instance = null;

    private int level = 1;
    public void TestPassing(string pImageId, float px, float py)
    {
        Debug.Log("image id " + pImageId + " x y " + px + " " + py);
    }

    public void GenerateHand()
    {
        //approaching hand generation via grabbing each individual UI element and updating the sprite image and render...didnt work out 13/04
        //actaullyworking just rendered tiny and behind default image too...13/04
        handTiles = GameObject.FindGameObjectsWithTag("handDefault");
        Debug.Log("handtiles length : " + handTiles.Length);
        BtmPanel = GameObject.FindGameObjectWithTag("bottomPanel");
         //check for null
         if (handTiles != null)
         {
             for (int i = 0; i < handTiles.Length; i++)
             {
                Debug.Log(handTiles[i]);
                //set background image as sprite using spriterenderer had sizing/scale issues...
                // handTiles[i].GetComponent<Image>().sprite = tileSprite[Random.Range(0, tileSprite.Length)] as Sprite;
                //clone object 
                //cloning object successful below but doesnt display properly and off propotions.
                //GameObject newObject = Instantiate(handTiles[i],handTiles[i].transform.position,handTiles[i].transform.localRotation) as GameObject;
                GameObject newObject = Instantiate(handTiles[i]);
                newObject.transform.localScale = handTiles[i].transform.localScale;
                newObject.transform.localPosition = handTiles[i].transform.localPosition;
                //set tag so handTiles above doesnt grab clones as well.
                newObject.tag = "handDrag";
                //assign new object correct parents
                newObject.transform.SetParent(BtmPanel.transform, false);
                //use handdefaults to instantiate objects with rng sprite below and add script....
                newObject.GetComponent<Image>().sprite = tileSprite[Random.Range(0, tileSprite.Length)] as Sprite;
                newObject.AddComponent<Draggable>();
                //above method with bool set to false solved instantiating flipped object....   
                //newObject.transform.parent = handTiles[i].transform.parent;
                
                Debug.Log(handTiles[i].transform.parent + "   ");
                //test[i] = GameObject.Instantiate(handTiles[i], handTiles[i].transform.position, Quaternion.identity);
                Debug.Log("trying to find component" + handTiles[i] + " :: " + i + " :::" + handTiles.Length + "l");
                 Debug.Log("tilesprite[] " + tileSprite[Random.Range(0, tileSprite.Length)]);
             }
         }
         //trying new method of grabbing BottomPanel panel and adding child comp...
       // BtmPanel = GameObject.FindGameObjectWithTag("bottomPanel");
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
        DontDestroyOnLoad(gameObject);
        InitGame();
    }
    //init game method
    void InitGame()
    {
        //setup board with rng sprites
        GenerateBoard(level);
    }

    public void GenerateBoard(int level)
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
        //do this every second
        /*if (Time.time >= nextTime)
        {
            Debug.Log("mouse x: " + Input.mousePosition.x + " mouse y: " + Input.mousePosition.y);
            nextTime += interval;
        }*/

    }
}

