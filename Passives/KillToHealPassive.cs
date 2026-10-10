using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Kill To Heal")]
public class KillToHealPassive : Passive
{
    [Header("Kill To Heal Passive Variables")]
    [SerializeField] private int _hpRegenAmount = 5;

    public override void OnEnemyDeath(PlayerBehaviour player_behaviour)
    {
        player_behaviour.HealHp(_hpRegenAmount);
    }
}
