using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    public string playername { get; set; }
    public int score { get; set; }

    public Text playerNameText;
    public Text playerScoreText;

    public void AssignData()
    {
        playerNameText.text = playername;
        playerScoreText.text = score + "   points";
    }

}
