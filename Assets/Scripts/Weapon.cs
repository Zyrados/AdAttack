using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public GameObject LaserShoot;
    public float LaserSpeed = 50;
    public GameObject LaserSpawnPoint;
    public AudioSource LaserSound;

    public ParticleSystem muzzleflash;
	
    void Start () 
    {
        LaserSpawnPoint = GameObject.Find("LaserSpawnPoint");
        muzzleflash.Stop();	
	}
	
	
	void Update () 
    {
        Shoot();	
	}

    void Shoot()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
           GameObject Laser = Instantiate(LaserShoot, LaserSpawnPoint.transform.position, Quaternion.identity) as GameObject;
           Vector3 direction = transform.TransformDirection(Vector3.forward);
           Laser.GetComponent<Rigidbody>().AddForce(direction * LaserSpeed, ForceMode.VelocityChange);
            muzzleflash.Play();
           LaserSound.Play();
        }
    }
}
