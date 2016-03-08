using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class FBholder : MonoBehaviour {

    public GameObject UIFBIsLoggedIN;
    public GameObject UIFBNotLoggedIN;
    public GameObject UIFBAvatar;
    public GameObject UIFBUserName;
    public GameObject ScoreEntryPanel;
    public GameObject ScoreScrollList;

    private List<object> ScoresList = null;
    private int getLevel;
    private Dictionary<string, string> profile = null;

    void Awake()
    {
        FB.Init(SetInit, OnHideUnity);
        getLevel = PlayerPrefs.GetInt("Level");
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = (true);
        Debug.Log("Cursor visible Start");
    }
    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = (true);
        Debug.Log("Cursor visible Update");
    }

    private void SetInit()
    {
        Debug.Log("FB Init done");
        if(FB.IsLoggedIn)
        {
            DealWithFBMenus(true);
            Debug.Log("FB Logged in");
        }
        else
        {
            DealWithFBMenus(false);
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void FBLoggin()
    {
        FB.Login("email,publish_actions", AuthCallback);
    }

    void AuthCallback(FBResult result)
    {
        if(FB.IsLoggedIn)
        {
            DealWithFBMenus(true);
            Debug.Log("FB loggin worked");
            SetScores();
            QueryScores();
        }
        else
        {
            DealWithFBMenus(false);
            Debug.Log("FB login fail");
        }
    }

    void DealWithFBMenus(bool isLoggedIn)
    {
        if(isLoggedIn)
        {
            UIFBIsLoggedIN.SetActive(true);
            UIFBNotLoggedIN.SetActive(false);
            //get profile picture code
            FB.API(Util.GetPictureURL("me", 256, 256), Facebook.HttpMethod.GET, DealWithProfilePicture);
            FB.API("/me?fields=id,first_name", Facebook.HttpMethod.GET, DealWithUserName);
            //get username code
        }
        else
        {
            UIFBIsLoggedIN.SetActive(false);
            UIFBNotLoggedIN.SetActive(true);
        }

    }

    void DealWithUserName(FBResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Problem with getting user name");
            FB.API("/me?fields=id,first_name", Facebook.HttpMethod.GET, DealWithUserName);
            return;
        }

        profile = Util.DeserializeJSONProfile(result.Text);

        Text UserMsg = UIFBUserName.GetComponent<Text>();
        string name; 
        if(profile.TryGetValue("first_name",out name))
        {
            UserMsg.text = "Hello, " + name;
        }
       

    }

    void DealWithProfilePicture(FBResult result)
    {
        if(result.Error != null)
        {
            Debug.Log("Problem with getting profile picture");
            FB.API(Util.GetPictureURL("me", 256, 256), Facebook.HttpMethod.GET, DealWithProfilePicture);
            return;
        }

        Image UserAvatar = UIFBAvatar.GetComponent<Image>();
        UserAvatar.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 256, 256), new Vector2(0, 0));    


    }

    public void ShareWithFriends()
    {
        FB.Feed(
            linkCaption: "I'm playing this awesome game",
            picture: "https://scontent-fra3-1.xx.fbcdn.net/hphotos-xft1/t31.0-8/p960x960/10003736_1456422167988347_4108855832150958269_o.png",
            linkName: "Check out this game",
            link: "apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + (FB.IsLoggedIn ? FB.UserId : "guest")

            );
    }

    public void InviteFriends()
    {
        FB.AppRequest(
            message: "This game is awesome, join me,now!",
            title: "Invite your friends to join you"
            );
    }

    //All Scores API related things
    public void QueryScores()
    {
        FB.API("/app/scores?fields=score,user.limit(30)", Facebook.HttpMethod.GET, ScoresCallback);
    }

    public void SetScores()
    {
        var scoreData = new Dictionary<string, string>();
        //Level hier an Score übergeben
        scoreData["score"] = getLevel.ToString();
        FB.API("/me/scores", Facebook.HttpMethod.POST, delegate (FBResult result)
              {
                  Debug.Log("Score submit result: " + result.Text);
              }, scoreData);

    }

    private void ScoresCallback(FBResult result)
    {

        Debug.Log("Scores Callback: " + result.Text);
       
        ScoresList = Util.DeserializeScores(result.Text);

        foreach(Transform child in ScoreScrollList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //Erzeugt ein Panelobjekt für jeden Freund
        foreach (object score in ScoresList)
        {
            var entry = (Dictionary<string, object>)score;
            var user = (Dictionary<string, object>)entry["user"];
                      

            GameObject ScorePanel;
            ScorePanel = Instantiate(ScoreEntryPanel) as GameObject;
            ScorePanel.transform.parent = ScoreScrollList.transform;

            Transform ThisScoreName = ScorePanel.transform.Find("FriendName");
            Transform ThisScoreLevel= ScorePanel.transform.Find("FriendLevel");
            Text ScoreName = ThisScoreName.GetComponent<Text>();
            Text ScoreLevel = ThisScoreLevel.GetComponent<Text>();

            ScoreName.text = user["name"].ToString();
            ScoreLevel.text = "Level: " + entry["score"].ToString();

            Transform TheUserAvatar = ScorePanel.transform.Find("FriendAvatar");
            Image UserAvatar = TheUserAvatar.GetComponent<Image>();

            FB.API(Util.GetPictureURL(user["id"].ToString(), 256, 256), Facebook.HttpMethod.GET, delegate (FBResult pictureResult)
                {
                    if(pictureResult.Error != null) // if there was an error
                    {
                        Debug.Log(pictureResult.Error);
                    }
                    else // if everthing was right
                    {
                        UserAvatar.sprite = Sprite.Create(pictureResult.Texture, new Rect(0, 0, 256, 256), new Vector2(0, 0));
                    }
                });
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("Level", 1);
        GameManager.LevelCount = 1;
        LevelGenerator.LevelCount = 1;
        Application.LoadLevel("LevelGen");
    }
}

