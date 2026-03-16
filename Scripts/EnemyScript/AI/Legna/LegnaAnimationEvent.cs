using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class LegnaAnimationEvent : MonoBehaviour
    {
        public GameObject hitBox;
        private LegnaCharacter character;
        private Animator hitAim;
        void Start()
        {
            character = GetComponentInParent<LegnaCharacter>();
            hitAim = hitBox.GetComponent<Animator>();
        }

        public void AwakeFinish()
        {
            character.AW_endTrigger = true;
        }

        public void StunFinish()
        {
            character.ST_endTrigger = true;
        }

        public void JumpAttackAir()
        {
            character.JA_airTrigger = true;
        }
        
        public void JumpAttackFinish()
        {
            character.JA_finishTrigger = true;
        }

        public void NormalAttackFinish()
        {
            character.NA_finishTrigger = true;
        }

        public void TriggerExcalibur()
        {
            character.TriggerExcalibur();
        }

        public void TriggerSeriesExplosion1D()
        {
            character.TriggerSeriesDelayExplosion1D(5, 5f, .2f, .2f);
        }
        
        public void TriggerSeriesExplosion2D()
        {
            character.TriggerSeriesDelayExplosion2D(5, 5f, .2f, .2f);
        }

        public void TriggerSingleExplosion()
        {
            character.TriggerSingleDelayExplosion(.3f);
        }

        public void SpinAttackDashStart()
        {
            character.SA_dashTrigger = true;
        }

        public void SpinAttackFinish()
        {
            character.SA_finishTrigger = true;
        }

        public void DodgeStart()
        {
            character.Dodge_startTrigger = true;
        }
        
        public void DodgeFinish()
        {
            character.Dodge_finishTrigger = true;
        }

        public void GuardCounterPrepareEnd()
        {
            character.Guard_startTrigger = true;
        }

        public void ActivateShield()
        {
            character.shield.SetActive(true);
        }

        public void CounterStart()
        {
            character.Counter_startTrigger = true;
        }
        
        public void CounterEnd()
        {
            character.Counter_finishTrigger = true;
        }

        public void PhaseChangePrepareEnd()
        {
            character.PC_prepareEndTrigger = true;
        }
        
        public void PhaseChangeEnd()
        {
            character.PC_endTrggier = true;
        }

        public void TwinkleAttackPrepareEnd()
        {
            character.TA_prepareEnd = true;
        }
        
        public void TwinkleAttackEnd()
        {
            character.TA_endTrigger = true;
        }

        public void TriggerNA1HitBox()
        {
            hitAim.SetTrigger("CA1");
        }

        public void TriggerNA2HitBox()
        {
            hitAim.SetTrigger("CA2");
        }

        public void TriggerJAHitBox()
        {
            hitAim.SetTrigger("JA");
        }

        public void TriggerCounterHitBox()
        {
            hitAim.SetTrigger("CT");
        }

        public void TriggerSlash1HitBox()
        {
            hitAim.SetTrigger("SL1");
        }

        public void TriggerSlash2HitBox()
        {
            hitAim.SetTrigger("SL2");
        }

        public void TriggerSlash3HitBox()
        {
            hitAim.SetTrigger("SL3");
        }

        public void TriggerSlash4HitBox()
        {
            hitAim.SetTrigger("SL4");
        }
        
        public void TriggerSlash1EXHitBox()
        {
            hitAim.SetTrigger("SL1EX");
        }

        public void TriggerSlash2EXHitBox()
        {
            hitAim.SetTrigger("SL2EX");
        }

        public void TriggerSlash3EXHitBox()
        {
            hitAim.SetTrigger("SL3EX");
        }

        public void TriggerSlash4EXHitBox()
        {
            hitAim.SetTrigger("SL4EX");
        }

        public void TriggerShoot()
        {
            character.TriggerShoot();
        }

        public void TriggerThrowCross()
        {
            character.TriggerThrowCross();
        }

        public void CaliburEnd() {
            AudioManager.Instance.PlaySFX("Boss/LCaliburEnd");
        }
        
        public void LoopEnd() {
            AudioManager.Instance.PlaySFX("Boss/LoopEnd");
        }
        
        public void LFire() {
            AudioManager.Instance.PlaySFX("Boss/LFire");
        }

        public void LSAttack1() {
            AudioManager.Instance.PlaySFX("Boss/LSAttack1");
        }
        public void LSAttack2() {
            AudioManager.Instance.PlaySFX("Boss/LSAttack2");
        }
        public void LSAttack3() {
            AudioManager.Instance.PlaySFX("Boss/LSAttack3");
        }
        public void LSAttack4() {
            AudioManager.Instance.PlaySFX("Boss/LSAttack4");
        }

        public void LStun() {
            AudioManager.Instance.PlaySFX("Boss/LStun");
        }
        public void LThrowCross() {
            AudioManager.Instance.PlaySFX("Boss/LThrowCross");
        }
        
        public void L十字架1() {
            AudioManager.Instance.PlaySFX("Boss/L十字架1");
        }
        
        public void L十字架2() {
            AudioManager.Instance.PlaySFX("Boss/L十字架2");
        }
        
        public void L十字架跳劈And龙车End() {
            AudioManager.Instance.PlaySFX("Boss/L十字架跳劈&龙车End");
        }
        public void L闪现End() {
            AudioManager.Instance.PlaySFX("Boss/L闪现End");
        }
    }
}
