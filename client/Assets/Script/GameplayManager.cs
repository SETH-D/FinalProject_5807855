using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameplayManager : MonoBehaviour
{

    public enum EnterState
    {
        New = 0, Load
    };

    public enum PromtState
    {
        error, success
    };

    public PromtState promtState;

    public static EnterState enterState;

    public GameObject[] platePositions;
    public GameObject[] plateObjects;
    public GameObject platePrefab;

    public List<int> symbolInt;

    public Sprite[] symbolPlates;

    public PlateModel plateModel;

    public string url;

    public int gameTimer;
    public int score;
    public bool isFinish;

    public bool isHinting;

    #region UI
    public int pairs;
    public int clickCount;
    public Text pairText;
    public Text clickText;
    public Text scoreText;
    public Text timerText;
    public Text nameText;
    public Text promtText;
    public GameObject BlankBG;
    public GameObject WinText;
    public GameObject promtCanvas;
    public GameObject SavePromtPanel;
    public GameObject ExitPromtPanel;

    public Button SaveButton;
    #endregion

    void Awake()
    {
        if (enterState == EnterState.Load)
        {
            LoadGame();
        }
        else
        {

            plateModel = new PlateModel(LoginManager.player.playername);

            symbolInt = new List<int>()
            {

            1,1,2,2,3,3,4,4,5,5,6,6,7,7,8,8

            };

            AssignIcon();
        }

    }

    public void CancelQuit()
    {
        promtCanvas.SetActive(false);
        ExitPromtPanel.SetActive(false);
    }

    public void Quit()
    {
        if (ExitPromtPanel.activeInHierarchy)
        {
            SceneManager.LoadScene("Main");
        }
        else
        {
            promtCanvas.SetActive(true);
            ExitPromtPanel.SetActive(true);
        }
    }

    public void ClosePromt()
    {
        if (promtState == PromtState.error)
        {
            promtCanvas.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void PromtCanvas(string promtText)
    {
        promtCanvas.SetActive(true);
        SavePromtPanel.SetActive(true);
        this.promtText.text = promtText;
    }

    public void UpdateSave()
    {
        string s = PlayerPrefs.GetString("plates_" + LoginManager.player.playername, null);

        string updateRequest = url + "/user/updateprogress?save=" + s + "&username=" + LoginManager.player.username;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateRequest);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        string responseBody = new StreamReader(stream).ReadToEnd();
        JsonQualifier[] us = JsonConvert.DeserializeObject<JsonQualifier[]>(responseBody);

        Debug.Log(s);

        if (us[0].success)
        {
            promtState = PromtState.success;
            PromtCanvas("Save succeed.");
        }
        else
        {
            promtState = PromtState.error;
            PromtCanvas("Save failed.");
        }
    }

    public void SaveGame()
    {
        //plateModel.CleatPlatesList();

        for (int i = 0; i < plateObjects.Length; i++)
        {
            PlateModel.Plate p = new PlateModel.Plate();
            p.position = plateObjects[i].GetComponent<PlateProperties>().position;
            p.inActive = plateObjects[i].GetComponent<PlateProperties>().inActive;
            p.symbol = plateObjects[i].GetComponent<PlateProperties>().symbol;

            plateModel.AddPlate(p);
        }

        plateModel.platesMap.time = gameTimer;
        plateModel.platesMap.IsFinish = isFinish;
        plateModel.platesMap.clickCount = clickCount;

        UpdateSave();

    }

    public void LoadGame()
    {
        plateModel = new PlateModel(LoginManager.player.playername);

        string loginRequest = url + "/user/getprogress/" + LoginManager.player.username;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loginRequest);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        string responseBody = new StreamReader(stream).ReadToEnd();
        var py = JsonConvert.DeserializeObject<Player[]>(responseBody);

        LoginManager.player.save = py[0].save;

        PlayerPrefs.SetString("plates_" + LoginManager.player.playername, LoginManager.player.save);

        float pairFloat = 8f;

        for (int i = 0; i < plateObjects.Length; i++)
        {
            PlateModel.Plate p = plateModel.GetPlatesByPosition(i);

            plateObjects[i].GetComponent<PlateProperties>().inActive = p.inActive;
            plateObjects[i].GetComponent<PlateProperties>().symbol = p.symbol;

            plateObjects[i].transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = symbolPlates[p.symbol - 1];

            if (plateObjects[i].GetComponent<PlateProperties>().inActive)
            {
                plateObjects[i].GetComponent<PlateProperties>().plateState = PlateState.Open;
                plateObjects[i].GetComponent<Animator>().Play("PlateOpenFreeze");
                pairFloat -= 0.5f;
            }

        }

        pairs = (int)pairFloat;
        isFinish = false;

        clickCount = plateModel.platesMap.clickCount;
        gameTimer = plateModel.platesMap.time;
        Debug.Log(plateModel.platesMap.time);

        clickText.text = clickCount.ToString();
        pairText.text = pairs.ToString();
    }

    void Start()
    {
        nameText.text = LoginManager.player.playername;

        if (enterState == EnterState.New)
        {
            gameTimer = 0;
            score = 0;
        }

        isFinish = false;

        StartCoroutine(StartHint());
        StartCoroutine(Timer());
        StartCoroutine(CheckWinCondition());
    }

    IEnumerator StartHint()
    {
        yield return new WaitForSeconds(1f);
        isHinting = true;

        for (int i = 0; i < plateObjects.Length; i++)
        {
            if (!plateObjects[i].GetComponent<PlateProperties>().inActive)
                plateObjects[i].GetComponent<Animator>().Play("PlateOpenFlip");

        }

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < plateObjects.Length; i++)
        {
            if (!plateObjects[i].GetComponent<PlateProperties>().inActive)
                plateObjects[i].GetComponent<Animator>().Play("PlateCloseFlip");

        }

        yield return new WaitForSeconds(plateObjects[15].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        isHinting = false;
    }

    void FixedUpdate()
    {
        UpdateTimer();
    }

    IEnumerator CheckWinCondition()
    {
        while (true)
        {
            yield return new WaitUntil(() => pairs <= 0);

            StopCoroutine(Timer());
            isFinish = true;
            BlankBG.SetActive(true);
            WinText.SetActive(true);
            SaveButton.interactable = false;

            score += (int)(gameTimer * 100);

            scoreText.text = score.ToString();

            UpdateScore();

            break;
        }
    }

    public void UpdateScore()
    {
        if (score > LoginManager.player.score)
        {
            string updateRequest = url + "/user/updatescore?score=" + score.ToString() + "&username=" + LoginManager.player.username;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateRequest);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            LoginManager.player.score = score;
        }
    }

    public void UpdateTimer()
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(gameTimer);

        if (t.Seconds < 10 && t.Minutes < 10)
        {
            timerText.text = t.Minutes + "0 : 0" + t.Seconds;
        }
        else if (t.Seconds > 9 && t.Minutes < 10)
        {
            timerText.text = "0" + t.Minutes + " : " + t.Seconds;
        }
        else
        {
            timerText.text = t.Minutes + " : " + t.Seconds;
        }

    }

    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitWhile(() => isFinish);
            yield return new WaitWhile(() => isHinting);
            yield return new WaitForSecondsRealtime(1f);
            gameTimer += 1;
        }
    }

    public void UpdatePair(int count)
    {
        pairs += count;
        pairText.text = pairs.ToString();
    }

    public void ClickCount()
    {
        clickCount += 1;
        clickText.text = clickCount.ToString(); 
    }

    public void UpdateScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();
    }

    void PlateSpawning()
    {
        plateObjects = new GameObject[16];

        for (int i = 0; i < platePositions.Length; i++)
        {
            plateObjects[i] = Instantiate(platePrefab, platePositions[i].transform.position, Quaternion.identity);
            plateObjects[i].name = "Plate " + i;
            plateObjects[i].GetComponent<PlateProperties>().position = i;
            plateObjects[i].GetComponent<PlateProperties>().symbol = 0;
        }
    }

    void AssignIcon()
    {
        float floatPairs = 0f;
        foreach (GameObject p in plateObjects)
        {
            if (p.GetComponent<PlateProperties>().symbol == 0)
            {
                int randomNumber = Random.Range(0, symbolInt.Count);
                p.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = symbolPlates[symbolInt[randomNumber] - 1];
                p.transform.GetComponent<PlateProperties>().symbol = symbolInt[randomNumber];
                symbolInt.RemoveAt(randomNumber);
            }
            floatPairs += 0.5f;
        }
        pairs = (int)floatPairs;
    }
}


