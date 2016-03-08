using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {

    private CharacterController cc;
    
    void Awake()
    {
        Cursor.visible = false;
        Fader.FadeOUT = false;
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = (false);
        Debug.Log("Cursor invisible Start");
    }
	
	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.End))
        {
            Application.Quit();
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = (false);
        Debug.Log("Cursor invisible Update");

    }

    void LateUpdate()
    {
        CheckFall();
    }

    private void CheckFall()
    {
        if(this.transform.position.y < -300)
        {
            Application.LoadLevel("FB");
            PlayerPrefs.SetInt("Level", GameManager.LevelCount);
        }   
        
    }

    void OnTriggerEnter(Collider other)
    {        
        if(other.CompareTag("FadeOut"))
        {
            Fader.FadeOUT = true;            
        }
        if(other.CompareTag("Exit"))
        {
            GameManager.LevelCount+=1;
            LevelGeneratorTEST.LevelCount+=1;
            Application.LoadLevel("LevelGen");
            PlayerPrefs.SetInt("Level", GameManager.LevelCount);
        }

        //if (other.CompareTag("Exit"))
        //{
        //    Application.LoadLevel(Application.loadedLevel + 1);
        //}
    }
}
