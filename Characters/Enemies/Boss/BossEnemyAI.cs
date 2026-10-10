using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossPhase
{
    First,
    Second,
    Third
}

public class BossEnemyAI : EnemyAI
{
    [Header("Boss Enemy AI Variables")]
    public Animator _animator;

    [Header("Boss Enemy AI Timings")]
    public float _startIntroPhaseDuration = 0.5f;
    public float _introPhaseDuration = 2f;

    public float _betweenAttacksTime = 0.5f;
    public float _afterLastAttackTime = 1.5f;

    public float _prepareAttackDurationFirstPhase = 1f;
    public float _prepareAttackDurationSecondPhase = 0.7f;
    public float _prepareAttackDurationThirdPhase = 0.4f;

    public float _recupPhaseDurationFirstPhase = 5f;
    public float _recupPhaseDurationSecondPhase = 4f;
    public float _recupPhaseDurationThirdPhase = 3f;

    public float _changePhaseDuration = 3f;

    [Header("Change Phase Variable")]
    [HideInInspector] public BossPhase _bossPhase;
    public int _hpToStartSecondPhase = 50;
    public int _hpToStartThirdPhase = 30;

    [Header("Boss Attacks")]
    public BossAttackScript[] _bossAttacks;

    [HideInInspector] public List<BossAttackScript> _nextBossAttacks = new List<BossAttackScript>();

    [HideInInspector] public float _baseArmsAlphaColor;

    [HideInInspector] public Color32 _bossShowColor;
    [HideInInspector] public Color32 _bossHideColor;

    [Header("Boss Enemy AI Needed References")]
    public GameObject _bossScreamVfx;

    protected override void Start()
    {
        base.Start();

        _bossPhase = BossPhase.First;

        _bossShowColor = new Color32(255, 255, 255, 255);
        _bossHideColor = new Color32(180, 180, 180, 255);

        _baseArmsAlphaColor = _bossAttacks[0].GetComponent<SpriteRenderer>().color.a;

        _stateMachine.SetState(new BossEnemyIntroState(this));
    }

    public void GetArmor()
    {
        _enemyBehaviour._hasArmor = true;
    }

    public void LoseArmor()
    {
        _enemyBehaviour._hasArmor = false;
    }

    public void SpawnScreamVfx(float duration)
    {
        GameObject scream_vfx = Instantiate(_bossScreamVfx, transform.position, Quaternion.identity);
        StartCoroutine(DestroyScreamVfxInSeconds(scream_vfx, duration));
    }

    private IEnumerator DestroyScreamVfxInSeconds(GameObject scream_vfx, float duration) 
    { 
        yield return new WaitForSeconds(duration);
        Destroy(scream_vfx);
    }
}
