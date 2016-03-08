using UnityEngine;
using System.Collections;

public class AdSpawner : MonoBehaviour {

    public GameObject[] Ads;
   
    public float SpawnDelay;

    private float SpawnTimer = 0;
    public static int adsHaveSpawned;

    private int AdsToSpawn;
    private Vector3 SpawnPoint;
    private float SpawnRangeX;
    private float SpawnRangeY;
    private float SpawnRangeZ;

	void Start () 
    {
        AdsToSpawn = GameManager.adsToSpawn;
        SpawnPoint = this.transform.position;
        SpawnRangeX = GetComponent<BoxCollider>().size.x;
        SpawnRangeY = GetComponent<BoxCollider>().size.y;
        SpawnRangeZ = GetComponent<BoxCollider>().size.z;
	}
	
	
	void Update () 
    {
        startSpawn();        
	}

    private void startSpawn()
    {
        while(adsHaveSpawned < AdsToSpawn)
        {
            SpawnAd();
        }
    }

    private void SpawnAd()
    {
        Vector3 NewSpawnPos = CalculateRandomPos();
        int AdToSpawn = Random.Range(0,(Ads.Length));
        GameObject NewAd = Instantiate(Ads[AdToSpawn], NewSpawnPos, Quaternion.identity) as GameObject;
        adsHaveSpawned++;
    }

    private Vector3 CalculateRandomPos()
    {
        float x;
        float y;
        float z;

        x = Random.Range(-SpawnRangeX + this.transform.position.x, SpawnRangeX + this.transform.position.x);
        y = Random.Range(-SpawnRangeY + this.transform.position.y, SpawnRangeY + this.transform.position.y);
        z = Random.Range(-SpawnRangeZ + this.transform.position.z, SpawnRangeZ + this.transform.position.z);

        return new Vector3(x, y, z);

    }
}
