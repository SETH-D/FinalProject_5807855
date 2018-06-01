using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class MainManager : MonoBehaviour
{

    public string url;

    #region UI

    public Text playerNameText;
    public Text scoreText;
    public Text promtText;

    public Button leaderButton;

    public GameObject promtPanel;
    public GameObject exitPromt;
    public GameObject loadPromt;

    public void ExitPromt()
    {
        if (!exitPromt.activeInHierarchy)
        {
            promtPanel.SetActive(true);
            exitPromt.SetActive(true);
        }
        else
        {
            promtPanel.SetActive(false);
            exitPromt.SetActive(false);
        }
    }

    public void LoadPromt()
    {
        if (!loadPromt.activeInHierarchy)
        {
            promtPanel.SetActive(true);
            loadPromt.SetActive(true);
        }
        else
        {
            promtPanel.SetActive(false);
            loadPromt.SetActive(false);
        }
    }

    public void Play()
    {
        GameplayManager.enterState = GameplayManager.EnterState.New;
        SceneManager.LoadScene("gameplay");
    }

    public void Load()
    {
        GameplayManager.enterState = GameplayManager.EnterState.Load;
        SceneManager.LoadScene("gameplay");
    }

    public void TopScore()
    {
        SceneManager.LoadScene("top");
    }

    public void Exit()
    {
        Application.Quit();
    }

    #endregion

    // Use this for initialization
    void Start () {
        playerNameText.text = LoginManager.player.playername;
        scoreText.text = LoginManager.player.score + " : SCORE";

        if (LoginManager.player.save == "")
        {
            leaderButton.interactable = false;
        }
        else
        {
            leaderButton.interactable = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
