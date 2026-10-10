using System.Collections.Generic;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour _playerBehaviour;
    [HideInInspector] public List<Passive> _passives = new List<Passive>();

    private void Start()
    {
        FightSceneManager.Instance._onEnemyDeath.AddListener(OnEnemyDeath);
    }

    public void AddPassive(Passive passive)
    {
        _passives.Add(passive);
        passive.OnAddPassive(_playerBehaviour);
    }

    public void ReloadPassives()
    {
        foreach (Passive passive in _passives) 
        {
            passive.ReloadPassive(_playerBehaviour);
        }
    }

    public void OnEnemyDeath()
    {
        foreach (Passive passive in _passives)
        {
            passive.OnEnemyDeath(_playerBehaviour);
        }
    }
}
