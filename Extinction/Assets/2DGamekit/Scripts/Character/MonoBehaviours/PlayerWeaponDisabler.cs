using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class PlayerWeaponDisabler : MonoBehaviour
    {
        private PlayerInput inputs;

        private void OnEnable()
        {
            inputs = GetComponent<PlayerInput>();
            if (inputs)
            {
                inputs.DisableMeleeAttacking();
                inputs.DisableRangedAttacking();
            }
            enabled = false;
        }
    }
}
