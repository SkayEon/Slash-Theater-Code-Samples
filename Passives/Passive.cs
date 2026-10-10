using UnityEngine;
using UnityEngine.Localization;

public abstract class Passive : ScriptableObject
{
    [Header("Displayable variables")]
    public LocalizedString _name;
    public LocalizedString _description;
    public Sprite _icon;

    [Header("Rarity")]
    public Rarity _rarity;

    [Header("Uniqueness")]
    public bool _isUnique = false;

    protected PlayerBehaviour _playerBehaviour;

    public virtual void OnAddPassive(PlayerBehaviour player_behaviour)
    {
        _playerBehaviour = player_behaviour;
    }

    public virtual void ReloadPassive(PlayerBehaviour player_behaviour)
    {
        _playerBehaviour = player_behaviour;
    }

    public virtual void OnEnemyDeath(PlayerBehaviour player_behaviour)
    {
    }
}
