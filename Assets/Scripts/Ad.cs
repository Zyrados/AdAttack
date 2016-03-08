using UnityEngine;
using System.Collections;

public class Ad : MonoBehaviour {

    GameObject Player;
    public float Speed;
   
    
    
	void Start () 
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Speed = GameManager.adSpeed;
    }
		
	void Update () 
    {
        movement();
	}   

    private void movement()
    {
        transform.LookAt(Player.transform);
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);	
    }    
    
}
