using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FightSceneManager : MonoBehaviour
{
    public static FightSceneManager Instance;

    [Header("Player Data")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerSpawnPosition; 
    [HideInInspector] public PlayerBehaviour _playerBehaviour;

    private bool _isPlayerSpawned = false;

    // Fight data
    private List<GameObject> _enemyList;
    [HideInInspector] public int _currentDamageReceived = 0;
    [HideInInspector] public int _currentDamageDealt = 0;

    [Header("Other References")]
    public HealthBarScript _playerHealthBarScript;
    public EnemyHealthBarScript _enemyHealthBarScript;
    public CDCanvasScript _cdCanvasScript;

    public CameraScript _cameraScript;
    [SerializeField] private RewardChoiceManager _upgradeChoiceManager;

    public DamageDisplayScript _damageDisplayCanvas;

    [Header("Scene Variables")]
    private bool _isTutoScene = false;
    [SerializeField] private float _timeOfVictoryAnimation = 2f;

    [HideInInspector] public bool _isFightEnded = false;

    [Header("All Attacks")]
    [Header("Common Attacks")]
    public Attack[] _neutralCommonAttacks;
    public Attack[] _fireCommonAttacks;
    public Attack[] _iceCommonAttacks;
    public Attack[] _thunderCommonAttacks;
    public Attack[] _windCommonAttacks;

    [Header("Rare Attacks")]
    public Attack[] _neutralRareAttacks;
    public Attack[] _fireRareAttacks;
    public Attack[] _iceRareAttacks;
    public Attack[] _thunderRareAttacks;
    public Attack[] _windRareAttacks;

    [Header("Legendary Attacks")]
    public Attack[] _neutralLegendaryAttacks;
    public Attack[] _fireLegendaryAttacks;
    public Attack[] _iceLegendaryAttacks;
    public Attack[] _thunderLegendaryAttacks;
    public Attack[] _windLegendaryAttacks;

    [Header("All Passives")]
    public Passive[] _commonPassives;
    public Passive[] _rarePassives;
    public Passive[] _legendaryPassives;

    [Header("Element References")]
    public ThunderBouncingBallScript _thunderBouncingBallPrefab;
    public GameObject _burnVfx;
    public GameObject _iceStackPrefab;
    public TornadoHoleScript _tornadoHolePrefab;
    public TornadoHoleScript _tornadoHoleSpikePrefab;

    [Header("Other Attacks References")]
    public GameObject _iceReflectPrefab;

    [Header("Element Icons")]
    public Sprite _neutralElementIcon;
    public Sprite _fireElementIcon;
    public Sprite _windElementIcon;
    public Sprite _thunderElementIcon;
    public Sprite _iceElementIcon;

    [Header("Feedback References")]
    public GameObject _enemyChargingAttackFeedback;

    public GameObject _hitVfx;
    public GameObject _parryVfx;

    public GameObject _neutralAuraVfx;
    public GameObject _fireAuraVfx;
    public GameObject _iceAuraVfx;
    public GameObject _thunderAuraVfx;
    public GameObject _windAuraVfx;

    [HideInInspector] public UnityEvent _onPlayerSpawned;
    [HideInInspector] public UnityEvent _onEnemyDeath;
    [HideInInspector] public UnityEvent _onEndFight;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (!_isPlayerSpawned)
        {
            _isPlayerSpawned = true;

            SpawnPlayer();
            _onPlayerSpawned.Invoke();
            StartFightSceneGameObjects();
            InputManager.Instance.SwitchToPlayerActionMap();
        }
    }

    // Start fight scene GameObjects
    private void SpawnPlayer()
    {
        GameObject player_object = Instantiate(_playerPrefab, new Vector3(_playerSpawnPosition.position.x, _playerSpawnPosition.position.y, 0), Quaternion.identity);
        _playerBehaviour = player_object.GetComponent<PlayerBehaviour>();

        if (GameManager.Instance._shouldResetPlayerData)
        {
            GameManager.Instance._shouldResetPlayerData = false;
        }
        else
        {
            if (GameManager.Instance._hasPlayerDataBeenInitialized)
            {
                _playerBehaviour.GetPlayerDataFromGameManager();
            }
            else
            {
                _playerBehaviour.SendPlayerDataToGameManager();
                GameManager.Instance._hasPlayerDataBeenInitialized = true;
            }
        }
    }

    private void StartFightSceneGameObjects()
    {
        InputManager.Instance.GetPlayerRef();

        _cameraScript.InitializeCamera();

        // Initialize enemies
        EnemyBehaviour[] enemies = FindObjectsByType<EnemyBehaviour>(FindObjectsSortMode.None);

        foreach (EnemyBehaviour enemy in enemies)
        {
            AddEnemyToList(enemy.gameObject);
            enemy.GetComponent<EnemyAI>().InitializeEnemy();
        }
    }

    private void AddEnemyToList(GameObject enemy)
    {
        if (_enemyList == null)
        {
            _enemyList = new List<GameObject>();
        }
        _enemyList.Add(enemy);
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (_enemyList != null)
        {
            _enemyList.Remove(enemy);

            if (_enemyList.Count == 0)
            {
                EndFight();
            }
        }
    }

    private void EndFight()
    {
        _onEndFight.Invoke();
        _isFightEnded = true;

        if (!_isTutoScene)
        {
            StartCoroutine(EndFightCoroutine());
        }
    }

    private IEnumerator EndFightCoroutine()
    {
        InputManager.Instance.SwitchToUIActionMap();
        _playerBehaviour.SendPlayerDataToGameManager();

        _playerBehaviour.PlayAnimation("Victory");

        yield return new WaitForSeconds(_timeOfVictoryAnimation);

        _upgradeChoiceManager.DisplayLevelClearCanvas();
    }

    public List<GameObject> GetEnemyList()
    {
        return _enemyList;
    }
}
