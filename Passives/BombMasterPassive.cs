using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Bomb Master")]
public class BombMasterPassive : Passive
{
    [Header("Bomb Master Passive Variables")]
    [SerializeField] private int _bombBonusDamage = 1;
    [SerializeField] private int _bombBonusExplodeRange = 1;

    public override void OnAddPassive(PlayerBehaviour player_behaviour)
    {
        base.OnAddPassive(player_behaviour);

        _playerBehaviour._bombBonusDamage += _bombBonusDamage;
        _playerBehaviour._bombBonusExplodeRange += _bombBonusExplodeRange;
    }

    public override void ReloadPassive(PlayerBehaviour player_behaviour)
    {
        base.ReloadPassive(player_behaviour);

        _playerBehaviour._bombBonusDamage += _bombBonusDamage;
        _playerBehaviour._bombBonusExplodeRange += _bombBonusExplodeRange;
    }
}
