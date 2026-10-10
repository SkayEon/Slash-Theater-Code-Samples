using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public bool _isGamePaused = false;

    [HideInInspector] public bool _shouldResetPlayerData = true;
    [HideInInspector] public bool _hasPlayerDataBeenInitialized = false;

    [HideInInspector] public Attack _upBSpell;
    [HideInInspector] public Attack _downBSpell;
    [HideInInspector] public Attack _sideBSpell;
    [HideInInspector] public Attack _neutralBSpell;

    [HideInInspector] public List<Passive> _playerPassives = new List<Passive>();

    [HideInInspector] public int _playerHealth;
    [HideInInspector] public int _playerMaxHealth;

    [HideInInspector] public LoadingSceneAnimationScript _loadingSceneAnimationScript;
    [HideInInspector] public bool _isInLoadingTime = false;

    [Header("Map Data")]
    public MapDataScript _firstLevelMapData;

    public MapDataScript[] _1stPoolMapsData;
    public MapDataScript[] _2ndPoolMapsData;
    public MapDataScript[] _3rdPoolMapsData;

    public MapDataScript _bossMapData;

    [HideInInspector] public MapDataScript _currentMapData;

    [HideInInspector] public int _currentFloor = 1;

    [Header("All Attacks")]
    public Attack[] _allAttacks;

    [Header("All Passives")]
    public Passive[] _allPassives;

    [Header("Save & Load")]
    public SaveAndLoadScript _saveAndLoad;
    [HideInInspector] public bool _hasLoadedSavedData = false;

    [Header("Other References")]
    public ControllerDisplayScript _controllerDisplay;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if (_currentMapData == null)
        {
            _currentMapData = _firstLevelMapData;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        StartCoroutine(WaitBeforeEndLoading());
    }

    private IEnumerator WaitBeforeEndLoading()
    {
        yield return new WaitForSeconds(1f);
        _isInLoadingTime = false;
    }

    // Scene loading
    public void LoadFirstLevel()
    {
        if (_saveAndLoad.DoesSaveFileExist())
        {
            _saveAndLoad.DeleteSaveFile();
        }

        _shouldResetPlayerData = false;
        _hasPlayerDataBeenInitialized = false;

        InputManager.Instance.SwitchToPlayerActionMap();
        _currentMapData = _firstLevelMapData;
        WaitForLoadScene("First Level");
    }

    public void LoadMapSceneWithSave()
    {
        if (_saveAndLoad.DoesSaveFileExist())
        {
            _saveAndLoad.LoadGameData();
            _hasLoadedSavedData = true;
            _shouldResetPlayerData = false;
            _hasPlayerDataBeenInitialized = true;
            LoadMapScene();
        }
    }

    public void CheckLoadNextMap()
    {
        if (_currentFloor <= 11)
        {
            LoadMapScene();
        }
        else
        {
            ResetData();
            LoadStartingMenuScene();
        }
    }

    public void LoadMapScene()
    {
        InputManager.Instance.SwitchToUIActionMap();
        WaitForLoadScene("Map Scene");
    }

    public void LoadNextMap(string map_name)
    {
        InputManager.Instance.SwitchToPlayerActionMap();
        WaitForLoadScene(map_name);
    }

    // Development tool: load a predefined save to test and balance the boss fight.
    public void LoadBossSave()
    {
        _saveAndLoad.LoadSaveBoss();
        _hasLoadedSavedData = true;
        _shouldResetPlayerData = false;
        _hasPlayerDataBeenInitialized = true;
        LoadMapScene();
    }

    public void LoadStartingMenuScene()
    {
        InputManager.Instance.SwitchToUIActionMap();
        WaitForLoadScene("Proto Menu");
    }

    private void WaitForLoadScene(string next_scene_name)
    {
        StartCoroutine(EndSceneThenLoadNextScene(next_scene_name));
    }

    // Loading scene animation script
    private IEnumerator EndSceneThenLoadNextScene(string next_scene_name)
    {
        EndSceneAnimation();
        _isInLoadingTime = true;
        yield return new WaitForSeconds(1f);

        SceneManager.LoadSceneAsync(next_scene_name);
    }

    private void EndSceneAnimation()
    {
        if (_loadingSceneAnimationScript != null)
        {
            _loadingSceneAnimationScript.StartCloseCurtainAnimation();
        }
    }

    public void ResetData()
    {
        _currentFloor = 1;
        _shouldResetPlayerData = false;
        _hasPlayerDataBeenInitialized = false;
        _saveAndLoad.DeleteSaveFile();
    }

    public void SaveGameData()
    {
        _saveAndLoad.SaveGameData();
    }

    public void LoadGameData()
    {
        _saveAndLoad.LoadGameData();
    }
}