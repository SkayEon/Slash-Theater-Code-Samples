using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Bonus Hp")]
public class BonusHpPassive : Passive
{
    [Header("Bonus HP Passive Variables")]
    [SerializeField] private int _nbHPBonus = 10;

    public override void OnAddPassive(PlayerBehaviour player_behaviour)
    {
        base.OnAddPassive(player_behaviour);

        _playerBehaviour.AddMaxHealthPoints(_nbHPBonus);
    }
}
