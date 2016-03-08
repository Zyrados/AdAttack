using UnityEngine;
using System.Collections;
using System;

public enum Direction { left, right, straight};

public class LevelGeneratorTEST : MonoBehaviour {

    public GameObject StartZone;
    public GameObject EndZone;
    public GameObject AdSpawner;

    public GameObject Plattform;
    public GameObject Mittelpunkt;

    public float MaxPossibleRotation;

    public Material[] Skyboxes;

    
    public int LEVELCOUNT;
    public static int LevelCount = 1;
    private Vector3 startVector = new Vector3(0, 0, 0);
    private Vector3 endZoneVector = new Vector3(1.5f, -125, -10);
    private Vector3 lastVector;
    private Vector3 spawnerPos;

    private Vector3 NewSpawnPos = Vector3.zero;
    private GameObject actualObject;
    private GameObject lastUsedObject;
    private GameObject lastRotation;

    private bool lastDirectionLeft;
    private bool lastDirectionRight;
    private bool lastDirectionStraight;

    private float ObjectZSize;
    private float ObjectXSize;
    private float LastObjectXSize;
    private float LastObjectZSize;

    public static float ObjectsHaveSpawned;
    private float LevelSize;

   
    void Awake()
    {
        ObjectsHaveSpawned = 0;
        //Später wieder entfernen
        LEVELCOUNT = LevelCount;
        //LevelCount = LEVELCOUNT;
        LevelSize = LEVELCOUNT * 2;
        SetStartZone();
        StartGenerateLevel();
        SetGameManager();
    }

    void Update()
    {
        //StartGenerateLevel();

    }

    private void SetGameManager()
    {
        GameManager.LevelTime = LevelSize * 5;
        GameManager.AdsToSpawn = Convert.ToInt32(LevelSize * 4);
        GameManager.AdSpeed = LevelSize * 0.05f;
    }

    private void StartGenerateLevel()
    {
        actualObject = Plattform;
        if (LevelCount <= 10)
        {
            RenderSettings.skybox = Skyboxes[0];
        }
        if (LevelCount > 10)
        {
            RenderSettings.skybox = Skyboxes[1];
        }
        if (LevelCount > 20)
        {
            RenderSettings.skybox = Skyboxes[2];
        }
        if (LevelCount > 30)
        {
            RenderSettings.skybox = Skyboxes[3];
        }
        if (LevelCount > 40)
        {
            RenderSettings.skybox = Skyboxes[4];
        }

        //Debug.Log("Last Vector: " + lastVector);
        //Debug.Log("Last ObjectZSize: " + LastObjectZSize);
        //Debug.Log("Last ObjectXSize: " + LastObjectXSize);
        while (ObjectsHaveSpawned < LevelSize)
        {
            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //Left:CalculateRandomPosLeft();
            //Right:CalculateRandomPosRight();
            //Straight: CalculateRandomPos();      
            switch (GetDirection())
                {
                    case Direction.left:
                    // Position anhand der X größe berechnen
                    //Debug.Log("links");
                    if(lastDirectionLeft)
                    {
                        actualObject = GeneratePlattformLeftOrRight();
                        NewSpawnPos = CalculateRandomPosLeft();
                        lastDirectionLeft = true;
                        lastDirectionRight = false;
                        lastDirectionStraight = false;
                    }
                    if(lastDirectionRight)
                    {
                        actualObject = GeneratePlattformLeftOrRight();
                        NewSpawnPos = CalculateRandomPos();
                        lastDirectionLeft = true;
                        lastDirectionRight = false;
                        lastDirectionStraight = false;
                    }
                    if(lastDirectionStraight)
                    {
                        actualObject = GeneratePlattformLeftOrRight();
                        NewSpawnPos = CalculateRandomPosLeft();
                        lastDirectionLeft = true;
                        lastDirectionRight = false;
                        lastDirectionStraight = false;
                    }
                    break;
                    case Direction.right:
                    //Debug.Log("rechts");
                    // Position anhand der X größe berechnen
                    if (lastDirectionLeft)
                    {
                        actualObject = GeneratePlattformLeftOrRight();
                        NewSpawnPos = CalculateRandomPos();
                        lastDirectionLeft = false;
                        lastDirectionRight = true;
                        lastDirectionStraight = false;
                    }
                    if (lastDirectionRight)
                    {
                        actualObject = GeneratePlattformLeftOrRight();
                        NewSpawnPos = CalculateRandomPosRight();
                        lastDirectionLeft = false;
                        lastDirectionRight = true;
                        lastDirectionStraight = false;
                    }
                    if (lastDirectionStraight)
                    {
                        actualObject = GeneratePlattformLeftOrRight();
                        NewSpawnPos = CalculateRandomPosRight();
                        lastDirectionLeft = false;
                        lastDirectionRight = true;
                        lastDirectionStraight = false;
                    }
                    break;

                    case Direction.straight:
                    //Debug.Log("gerade");
                    if(lastDirectionLeft)
                    {
                        actualObject = GeneratePlattformStraight();
                        NewSpawnPos = CalculateRandomPosLeft();
                        lastDirectionLeft = false;
                        lastDirectionRight = false;
                        lastDirectionStraight = true;
                    }
                    if(lastDirectionRight)
                    {
                        actualObject = GeneratePlattformStraight();
                        NewSpawnPos = CalculateRandomPosRight();
                        lastDirectionLeft = false;
                        lastDirectionRight = false;
                        lastDirectionStraight = true;
                    }
                    if(lastDirectionStraight)
                    {
                        actualObject = GeneratePlattformStraight();
                        NewSpawnPos = CalculateRandomPos();
                        lastDirectionLeft = false;
                        lastDirectionRight = false;
                        lastDirectionStraight = true;
                    }
                    break;
                }

                ObjectZSize = actualObject.transform.localScale.z;
                ObjectXSize = actualObject.transform.localScale.x;

               //Debug.Log("New Position: " + NewSpawnPos);
                Instantiate(actualObject, NewSpawnPos, Quaternion.identity);
            //Instantiate(Mittelpunkt, NewSpawnPos, Quaternion.identity);
            ObjectsHaveSpawned++;
                LastObjectZSize = ObjectZSize;
                LastObjectXSize = ObjectXSize;
                lastVector = NewSpawnPos;
            //Debug.Log("-------------------------------------------------");
            }
        //}
            if (ObjectsHaveSpawned == LevelSize)
            {
            Debug.Log("Alles gespawnt");
                Vector3 NewSpawnPos = CalculateRandomPos();
                Instantiate(EndZone, NewSpawnPos + endZoneVector, Quaternion.identity);
                Vector3 AdSpawnPos = CalculateAdSpawnPos();
                Instantiate(AdSpawner, AdSpawnPos, Quaternion.identity);
            }
        
    }

    private GameObject GeneratePlattformStraight()
    {
        actualObject.transform.localScale = new Vector3(5, 1, UnityEngine.Random.Range(5, 11));
        return actualObject;
    }

    private Vector3 CalculateRandomPosLeft()
    {
        float getLastObjectCenter = LastObjectXSize / 2;
        float getObjectCenter = ObjectXSize / 2;
        float mindistance = getLastObjectCenter + getObjectCenter;
        float maxYdistance = 2;

        int randomizer = UnityEngine.Random.Range(0, 3);

        Vector3 newPosition = Vector3.zero;
        switch (randomizer)
        {
            case 0:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, -mindistance - 11),0,LastObjectZSize / 2);

                break;
            case 1:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance - 11), UnityEngine.Random.Range(-2, maxYdistance), LastObjectZSize / 2);

                break;
            case 2:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance - 11), UnityEngine.Random.Range(-2, -maxYdistance), LastObjectZSize / 2);

                break;

        }
        //Debug.Log("~~~ last Object Center: " + getLastObjectCenter);
        //Debug.Log("~~~ Object Center: " + getObjectCenter);
        //Vector3 newPosition = lastVector + new Vector3(0, 0, UnityEngine.Random.Range(mindistance, mindistance+8));
        return newPosition;


    }
    private Vector3 CalculateRandomPosRight()
    {
        float getLastObjectCenter = LastObjectXSize / 2;
        float getObjectCenter = ObjectXSize / 2;
        float mindistance = getLastObjectCenter + getObjectCenter;
        float maxYdistance = 2;

        int randomizer = UnityEngine.Random.Range(0, 3);

        Vector3 newPosition = Vector3.zero;
        switch (randomizer)
        {
            case 0:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(mindistance, mindistance + 11), 0, LastObjectZSize / 2);

                break;
            case 1:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(mindistance, mindistance + 11), UnityEngine.Random.Range(-2, maxYdistance), LastObjectZSize / 2);

                break;
            case 2:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(mindistance, mindistance + 11), UnityEngine.Random.Range(-2, -maxYdistance), LastObjectZSize / 2);

                break;

        }
        //Debug.Log("~~~ last Object Center: " + getLastObjectCenter);
        //Debug.Log("~~~ Object Center: " + getObjectCenter);
        //Vector3 newPosition = lastVector + new Vector3(0, 0, UnityEngine.Random.Range(mindistance, mindistance+8));
        return newPosition;


    }

    private GameObject GeneratePlattformLeftOrRight()
    {
        actualObject.transform.localScale = new Vector3(UnityEngine.Random.Range(5, 11), 1, 5);
        return actualObject;
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

        int randomizer = UnityEngine.Random.Range(0, 6);
        Vector3 newPosition = Vector3.zero;
        switch(randomizer)
        {
            case 0:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance + 4), 0, UnityEngine.Random.Range(mindistance, mindistance + 8));
                
                break;
            case 1:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance + 4), UnityEngine.Random.Range(-2,maxYdistance), UnityEngine.Random.Range(mindistance, mindistance + 8));
                
                break;
            case 2:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance + 4), UnityEngine.Random.Range(-2, -maxYdistance), UnityEngine.Random.Range(mindistance, mindistance + 8));
             
                break;
            case 3:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance - 4), 0, UnityEngine.Random.Range(mindistance, mindistance + 8));

                break;
            case 4:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance - 4), UnityEngine.Random.Range(-2, maxYdistance), UnityEngine.Random.Range(mindistance, mindistance + 8));

                break;
            case 5:
                newPosition = lastVector + new Vector3(UnityEngine.Random.Range(-mindistance, mindistance - 4), UnityEngine.Random.Range(-2, -maxYdistance), UnityEngine.Random.Range(mindistance, mindistance + 8));

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
        LastObjectXSize = 5;
        lastDirectionLeft = false;
        lastDirectionRight = false;
        lastDirectionStraight = true;
    }
}
