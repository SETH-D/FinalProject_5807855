using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class TopScoreManager : MonoBehaviour
{

    public string url;

    public GameObject playerScorePrefab;

    public Transform leaderPanel;

    #region UI

    public Text playerNameText;
    public Text scoreText;

    public void ExitToMain()
    {
        SceneManager.LoadScene("Main");
    }

    #endregion

    // Use this for initialization
    void Start () {
        playerNameText.text = LoginManager.player.playername;
        scoreText.text = LoginManager.player.score + " : SCORE";

        LoadTopTenPlayer();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadTopTenPlayer()
    {
        string RequestTopTen = url + "/top10users";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RequestTopTen);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        string responseBody = new StreamReader(stream).ReadToEnd();
        var p = JsonConvert.DeserializeObject<PlayerScore[]>(responseBody);

        for (int i = 0; i < p.Length; i++)
        {
            GameObject ps = Instantiate(playerScorePrefab, leaderPanel);
            ps.GetComponent<PlayerScore>().playername = p[i].playername;
            ps.GetComponent<PlayerScore>().score= p[i].score;
            ps.GetComponent<PlayerScore>().AssignData();
        }
    }
}
