using UnityEngine;
using System.Collections;
using System;

//public enum Direction { left, right, straight};

public class LevelGenerator : MonoBehaviour {

    public GameObject StartZone;
    public GameObject EndZone;
    public GameObject AdSpawner;

    public GameObject[] Objects;

    public float MaxPossibleRotation;



    public static int LevelCount = 1;
    private Vector3 startVector = new Vector3(0, 0, 0);
    private Vector3 endZoneVector = new Vector3(1.5f, -105, -10);
    private Vector3 lastVector;
    private Vector3 spawnerPos;
    private GameObject lastUsedObject;
    private GameObject lastRotation;
    private float ObjectZSize;
    private float LastObjectZSize;

    private float ObjectsHaveSpawned;
    private float LevelSize;

    void Awake()
    {        
        LevelSize = LevelCount * 2;
        SetStartZone();
        StartGenerateLevel();
        SetGameManager();        
    }

    private void SetGameManager()
    {
        GameManager.LevelTime = LevelSize * 7;
        GameManager.AdsToSpawn = Convert.ToInt32(LevelSize * 4);
        GameManager.AdSpeed = LevelSize * 0.25f;
    }

    private void StartGenerateLevel()
    {
        //Debug.Log("Last Vector: " + lastVector);
        //Debug.Log("Last ObjectZSize: " + LastObjectZSize);
       while (ObjectsHaveSpawned < LevelSize)
        {
            int ObjectToSpawn = UnityEngine.Random.Range(0, (Objects.Length));
            switch(GetDirection())
            {
                case Direction.left:
                    break;
                case Direction.right:
                    break;
                case Direction.straight:
                    break;
            }
            ObjectZSize = Objects[ObjectToSpawn].transform.localScale.z;
            Vector3 NewSpawnPos = CalculateRandomPos();
            //Debug.Log("New Position: " + NewSpawnPos);
            Instantiate(Objects[ObjectToSpawn], NewSpawnPos, Quaternion.identity);
            ObjectsHaveSpawned++;
            LastObjectZSize = ObjectZSize;
            lastVector = NewSpawnPos;            
        }
       if(ObjectsHaveSpawned == LevelSize)
        {
            Vector3 NewSpawnPos = CalculateRandomPos();
            Instantiate(EndZone, NewSpawnPos + endZoneVector, Quaternion.identity);
            Vector3 AdSpawnPos = CalculateAdSpawnPos();
            Instantiate(AdSpawner, AdSpawnPos, Quaternion.identity);
        }
    }

    private Direction GetDirection()
    {
        int randomizer = UnityEngine.Random.Range(0, 3);
        
        switch(randomizer)
        {
            case 0:
                return Direction.left;
                
            case 1:
                return Direction.right;
                
            case 2:
                return Direction.straight;               
        }
        return 0;
    }

    private Vector3 CalculateAdSpawnPos()
    {
        spawnerPos = lastVector / 2;
        return spawnerPos;
    }

    private Vector3 CalculateRandomPos()
    {
        float getLastObjectCenter = LastObjectZSize / 2;
        float getObjectCenter = ObjectZSize / 2;
        float mindistance = getLastObjectCenter + getObjectCenter;
        float maxYdistance = 2;

        int randomizer = UnityEngine.Random.Range(0, 3);
        Vector3 newPosition = Vector3.zero;
        switch(randomizer)
        {
            case 0:
                newPosition = lastVector + new Vector3(0, 0, UnityEngine.Random.Range(mindistance, mindistance + 8));
                
                break;
            case 1:
                newPosition = lastVector + new Vector3(0, UnityEngine.Random.Range(-2,maxYdistance), UnityEngine.Random.Range(mindistance, mindistance + 8));
                
                break;
            case 2:
                newPosition = lastVector + new Vector3(0, UnityEngine.Random.Range(-2, -maxYdistance), UnityEngine.Random.Range(mindistance, mindistance + 8));
             
                break;
                
        }
        //Debug.Log("~~~ last Object Center: " + getLastObjectCenter);
        //Debug.Log("~~~ Object Center: " + getObjectCenter);
        //Vector3 newPosition = lastVector + new Vector3(0, 0, UnityEngine.Random.Range(mindistance, mindistance+8));
        return newPosition;        
    }

    private void SetStartZone()
    {
        Instantiate(StartZone, startVector, Quaternion.identity);
        LastObjectZSize = 20;
    }
}
