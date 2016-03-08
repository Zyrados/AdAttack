using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    //Wird vom LevelGen gesetzt
    public static float LevelTime;
    public static int AdsToSpawn;
    public static float AdSpeed;

    public Text TimeLeft;
    public Text AdCounter;
    public Text Level;
    public static int LevelCount = 1;

    //Zum Debuggen
    public Text AdsGespawnt;
    public Text ObjectsGespawnt;
    
    //AdSpawner greifen drauf zu
    public static int adsClosed;
    public static int adsToSpawn;
    public static float adSpeed;

    void Awake()
    {
        adsToSpawn = AdsToSpawn;
        adSpeed = AdSpeed;
        showLevel();
    }

    private void showLevel()
    {
        Level.text = LevelCount.ToString();
        AdsGespawnt.text = AdSpawner.adsHaveSpawned.ToString();
        ObjectsGespawnt.text = LevelGeneratorTEST.ObjectsHaveSpawned.ToString();
    }

    void Update () 
    {
        countTime();
        showClosedAds();
        Lose();
        //Debug.Log("Level" + LevelCount);
	}

    private void showClosedAds()
    {
        AdCounter.text = adsClosed.ToString();
    }

    private void Lose()
    {
        if(LevelTime < 0)
        {
            Application.LoadLevel("FB");
        }
    }

    private void countTime()
    {
        LevelTime -= Time.deltaTime;
        TimeLeft.text = LevelTime.ToString("0");
    }
}
