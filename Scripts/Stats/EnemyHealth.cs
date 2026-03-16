using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class EnemyHealth : Health, IDamagebale
    {
        public bool giveUpwardForce = true;
        [SerializeField] protected int recoverAmount;
        [SerializeField] protected float recoverTime;
        private float recoverTimeCountdown;
        public void TakeMidBarDamage(int amount) {
            
        }

        public void TakeDamage(int amount) {
            DealDamage(amount);
        }

        public bool IsGiveUpwardForce() {
            return giveUpwardForce;
        }

        public override void DealDamage(int amount)
        {
            base.DealDamage(amount);
            if (healthPoints <= 0 )// && gameObject.GetComponent<EnemyCharacter>())
            {
                healthPoints = 0;
                gameObject.SetActive(false);
                Invoke("Revive", 5);
            }
        }
        
        //This revives the enemy quickly so you can test out certain features when building game; this method probably shouldn't exist in real game
        protected virtual void Revive()
        {
            gameObject.GetComponent<Health>().healthPoints += 100;
            gameObject.SetActive(true);
        }
        protected virtual void HandleRecovery()
        {
            if (character.isDead)
            {
                return;
            }
            if (hit)
            {
                recoverTimeCountdown = recoverTime;
            }
            else
            {
                if (recoverTimeCountdown > 0)
                {
                    recoverTimeCountdown -= Time.deltaTime;
                }
                else
                {
                    recoverTimeCountdown = recoverTime;
                    GainCurrentHealth(recoverAmount);
                }
            }
        }
        
        public virtual void GainCurrentHealth(int amount)
        {
            healthPoints += amount;
            if (healthPoints > maxHealthPoints)
            {
                healthPoints = maxHealthPoints;
            }
            G.UI.playerHealthState.playerHealth = healthPoints;
            G.UI.playerHealthState.MarkDirty();
        }

        public int ReturnHealthPoint() {
            return healthPoints;
        }
    }

}