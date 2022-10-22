using UnityEngine;

public class PlayerAnimationForwarder : MonoBehaviour
{
    private PlayerCombat _playerCombat;

    //Runs when the hold is finished
    public void RegisterHold()
    {
        if (_playerCombat == null)
        {
            _playerCombat = GetComponentInParent<PlayerCombat>();
        }

        _playerCombat.TriggerHold();
    }

    public void RegisterAttack()
    {
        if(_playerCombat == null)
        {
            _playerCombat = GetComponentInParent<PlayerCombat>();
        }

        _playerCombat.MeleeAttack();
        _playerCombat.RangedAttack();
        _playerCombat.FocusCastAttack();
    }
}
