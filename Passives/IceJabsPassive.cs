using UnityEngine;

// Unique passive: duplicate acquisition is handled by the passive selection system.
[CreateAssetMenu(menuName = "Passives/Ice Jab")]
public class IceJabsPassive : Passive
{
    public override void OnAddPassive(PlayerBehaviour player_behaviour)
    {
        base.OnAddPassive(player_behaviour);

        _playerBehaviour._isJabIced = true;
    }

    public override void ReloadPassive(PlayerBehaviour player_behaviour)
    {
        base.ReloadPassive(player_behaviour);

        _playerBehaviour._isJabIced = true;
    }
}
