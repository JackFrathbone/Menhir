using UnityEngine;

public class PlayerAnimationForwarder : MonoBehaviour
{
    private PlayerCombat _playerCombat;

    public void RegisterAttack()
    {
        if(_playerCombat == null)
        {
            _playerCombat = GetComponentInParent<PlayerCombat>();
        }

        _playerCombat.Attack();
    }
}
