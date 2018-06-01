using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class LoginManager : MonoBehaviour
{
    public static Player player;

    public string url;

    #region UI

    public enum PromtState
    {
        login, register, none
    };

    public PromtState promtState;

    public GameObject loginPanel;
    public GameObject registerPanel;

    public InputField usernameInput;
    public InputField passwordInput;

    public InputField newUserInput;
    public InputField newPassInput;
    public InputField newNameInput;

    public GameObject promtPanel;
    public GameObject exitPromtPanel;
    public Text promtText;

    public void ExitPromt()
    {
        if (!exitPromtPanel.activeInHierarchy)
        {
            exitPromtPanel.SetActive(true);
        }
        else
        {
            exitPromtPanel.SetActive(false);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void PromtPanel(string announce, PromtState promtState)
    {
        if (!promtPanel.activeInHierarchy)
        {
            this.promtState = promtState;
            promtPanel.SetActive(true);
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            promtText.text = announce;
        }
    }

    public void ClosePromt()
    {
        if (promtState == PromtState.login)
        {
            promtPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (promtState == PromtState.register)
        {
            promtPanel.SetActive(false);
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
        else if (promtState == PromtState.none)
        {
            promtPanel.SetActive(false);
            SceneManager.LoadScene("Main");
        }
    }

    public void CloseRegister()
    {
        if (registerPanel.activeInHierarchy)
        {
            registerPanel.SetActive(false);
            loginPanel.SetActive(true);
        }
    }

    public void Login()
    {
        string loginRequest = url + "/login/" + usernameInput.text + "/" + passwordInput.text;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loginRequest);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        string responseBody = new StreamReader(stream).ReadToEnd();
        var p = JsonConvert.DeserializeObject<Player[]>(responseBody);

        if (p.Length == 0)
        {
            PromtPanel("Invalid username or password.", PromtState.login);
        }
        else if (p.Length > 0)
        {
            player = p[0];
            PromtPanel("Login Completed.", PromtState.none);
        }

    }

    public void Register()
    {
        string registRequest = url + "/user/add/user?username=" + newUserInput.text + "&password=" + newPassInput.text + "&playername=" + newNameInput.text;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(registRequest);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        string responseBody = new StreamReader(stream).ReadToEnd();
        JsonQualifier[] r = JsonConvert.DeserializeObject<JsonQualifier[]>(responseBody);

        if (!r[0].success || r.Length < 1 || r == null || response == null)
        {
            PromtPanel("Register Failed, Please Try Again", PromtState.register);
        }
        else if (r[0].success)
        {
            PromtPanel("Register Completed.", PromtState.register);
        }
    }

    public void RegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    #endregion

}
