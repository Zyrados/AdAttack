using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

    public AudioSource MusicClip;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
	void Start () 
    {
        MusicClip.Play();
	}	
}
