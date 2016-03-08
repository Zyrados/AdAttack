using UnityEngine;
using System.Collections;

public class Buttons : MonoBehaviour {

    public void OnClickStart()
    {
        Application.LoadLevel("LevelGen");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
