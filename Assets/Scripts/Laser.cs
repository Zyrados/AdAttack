using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    public AudioSource CloseSound;

    public float LifeTime = 20;
    public ParticleSystem AdExplosion;
    void LateUpdate()
    {       
        LifeTime -= Time.deltaTime;
        if(LifeTime<0)
        {
            Destroy(this.gameObject);
        }
    }

	void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("X"))
        {            
            CloseSound.Play();           
            Destroy(this.gameObject,0.20f);
            Destroy(other.gameObject,0.10f);
            AdExplosion.Play();
            GameManager.adsClosed++;            
        }

        if(other.CompareTag("Bound"))
        {
            Destroy(this.gameObject);
        }
        
    }
}
