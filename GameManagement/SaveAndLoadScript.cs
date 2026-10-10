using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadScript : MonoBehaviour
{
    private static readonly string _saveFolderPath = Application.dataPath + "/Saves/";
    private static readonly string _saveFilePath = _saveFolderPath + "/savefile.json";
    private static readonly string _bossSaveFilePath = _saveFolderPath + "/bosssavefile.json";

    public void SaveGameData()
    {
        if(!Directory.Exists(_saveFolderPath))
        {
            Directory.CreateDirectory(_saveFolderPath);
        }

        //get spells names
        string upBSpellName = "";
        string downBSpellName = "";
        string sideBSpellName = "";
        string neutralBSpellName = "";

        if (GameManager.Instance._upBSpell != null)
        {
            upBSpellName = GameManager.Instance._upBSpell.name;
        }
        if (GameManager.Instance._downBSpell != null)
        {
            downBSpellName = GameManager.Instance._downBSpell.name;
        }
        if (GameManager.Instance._sideBSpell != null)
        {
            sideBSpellName = GameManager.Instance._sideBSpell.name;
        }
        if (GameManager.Instance._neutralBSpell != null)
        {
            neutralBSpellName = GameManager.Instance._neutralBSpell.name;
        }

        // get passive names
        List<string> passive_names = new List<string>();
        foreach (Passive passive in GameManager.Instance._playerPassives)
        {
            passive_names.Add(passive.name);
        }

        //save data to json
        SaveObject save_object = new SaveObject
        {
            playerHealth = GameManager.Instance._playerHealth,
            playerMaxHealth = GameManager.Instance._playerMaxHealth,
            
            upBSpellName = upBSpellName,
            downBSpellName = downBSpellName,
            sideBSpellName = sideBSpellName,
            neutralBSpellName = neutralBSpellName,

            passiveNames = passive_names,

            currentFloor = GameManager.Instance._currentFloor,
        };

        string json = JsonUtility.ToJson(save_object);

        File.WriteAllText(_saveFilePath, json);
    }

    public void LoadGameData()
    {
        LoadSave(_saveFilePath);
    }

    public void LoadSaveBoss()
    {
        LoadSave(_bossSaveFilePath);
    }

    private void LoadSave(string path_save)
    {
        if (File.Exists(path_save))
        {
            string json = File.ReadAllText(path_save);

            SaveObject save_object = JsonUtility.FromJson<SaveObject>(json);

            //Player data
            GameManager.Instance._playerHealth = save_object.playerHealth;
            GameManager.Instance._playerMaxHealth = save_object.playerMaxHealth;

            //attacks
            GameManager.Instance._upBSpell = GetAttackByName(save_object.upBSpellName);
            GameManager.Instance._downBSpell = GetAttackByName(save_object.downBSpellName);
            GameManager.Instance._sideBSpell = GetAttackByName(save_object.sideBSpellName);
            GameManager.Instance._neutralBSpell = GetAttackByName(save_object.neutralBSpellName);

            //passives
            GameManager.Instance._playerPassives.Clear();

            foreach (string passive_name in save_object.passiveNames)
            {
                Passive passive = GetPassiveByName(passive_name);
                if (passive != null)
                {
                    GameManager.Instance._playerPassives.Add(passive);
                }
            }

            //game data
            GameManager.Instance._currentFloor = save_object.currentFloor;
        }
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
        }
    }

    public bool DoesSaveFileExist()
    {
        return File.Exists(_saveFilePath);
    }

    private Attack GetAttackByName(string attack_name)
    {
        foreach (Attack attack in GameManager.Instance._allAttacks)
        {
            if (attack.name == attack_name)
            {
                return attack;
            }
        }
        return null;
    }

    private Passive GetPassiveByName(string passive_name)
    {
        foreach (Passive passive in GameManager.Instance._allPassives)
        {
            if (passive.name == passive_name)
            {
                return passive;
            }
        }
        return null;
    }

    private class SaveObject
    {
        public int playerHealth;
        public int playerMaxHealth;

        public string upBSpellName;
        public string downBSpellName;
        public string sideBSpellName;
        public string neutralBSpellName;

        public List<string> passiveNames;

        public int currentFloor;
    }
}
