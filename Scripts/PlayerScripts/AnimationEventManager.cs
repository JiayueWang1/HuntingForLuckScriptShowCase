using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class AnimationEventManager : MonoBehaviour
    {
        private MeleeAttackManager _meleeAttack;

        private WeaponAttackManager _weaponAttackManager;
        // Start is called before the first frame update
        void Start()
        {
            _meleeAttack = GetComponentInParent<MeleeAttackManager>();
            _weaponAttackManager = GetComponentInParent<WeaponAttackManager>();
        }

        private void TriggerPlaceProjectile()
        {
            _weaponAttackManager.TriggerPlaceProjectile();
        }
        
        public void TriggerMeleeWeapon()
        {
            _meleeAttack.TriggerMeleeWeapon();
        }
        
        private void FinishMeleeAttack()
        {
            _meleeAttack.FinishMeleeAttack();
        }

        public void PlayerDodge() {
            AudioManager.Instance.PlaySFX("Player/PlayerDodge");
        }
        
        public void PAttack1() {
            AudioManager.Instance.PlaySFX("Player/PAttack1");
        }
        public void PAttack2() {
            AudioManager.Instance.PlaySFX("Player/PAttack2");
        }

        public void PFire() {
            AudioManager.Instance.PlaySFX("Player/PFire");
        }
        public void Land() {
            AudioManager.Instance.PlaySFX("Player/Land");
        }
        public void Jump() {
            AudioManager.Instance.PlaySFX("Player/Jump");
        }
    }
}