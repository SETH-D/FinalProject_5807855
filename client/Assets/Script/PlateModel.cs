using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateModel
{
    const string prefId = "plates_";

    string playerName;

    public PlatesMap platesMap = new PlatesMap();

    public PlateModel(string playerName)
    {
        this.playerName = playerName;
        LoadPlatesList();
    }

    public void ClearPlatesList()
    {
        var platesJson = PlayerPrefs.GetString(prefId + playerName, null);
        PlayerPrefs.SetString(prefId + playerName, "");
    }

    void LoadPlatesList()
    {
        var platesJson = PlayerPrefs.GetString(prefId + playerName, null);
        if (!string.IsNullOrEmpty(platesJson))
        {
            this.platesMap = JsonUtility.FromJson<PlatesMap>(platesJson);
        }
    }

    public void AddPlate(Plate plate)
    {
        platesMap.plates.Add(plate);
        this.SavePlatesList();
    }

    void SavePlatesList()
    {
        var platesJson = JsonUtility.ToJson(this.platesMap);
        PlayerPrefs.SetString(prefId + playerName, platesJson);
        LoginManager.player.save = PlayerPrefs.GetString(prefId + playerName, null);
    }

    public Plate GetPlatesByPosition(int position)
    {
        return platesMap.plates.Find(p => p.position == position);
    }

    [System.Serializable]
    public class Plate
    {
        public int position;
        public bool inActive;
        public int symbol;
    }

    [System.Serializable]
    public class PlatesMap
    {
        public List<Plate> plates = new List<Plate>();

        public bool IsFinish;

        public int time;

        public int clickCount;
    }
}
