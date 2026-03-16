using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class GeneralEnemyTrigger : Managers
    {
        public int damageAmount = 20;
        public int GainHealthAmountFromAttack = 20;
        public bool teleportAfterHit;
        public bool disableColliderAfterHit;
        public Transform teleportPosition = null;
        public bool hasDirection;
        public float verticalDamageForce;
        public float horizontalDamageForce;
        
        [HideInInspector] public LegnaHealth legnaHealth;
        [HideInInspector] public bool alreadyHit;

        private void Start() {
            legnaHealth = FindObjectOfType<LegnaHealth>();
            alreadyHit = false;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (G.I.UIMain.currentTarget != legnaHealth) {
                legnaHealth.UpdateEnemyInformation();
            }
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            
            if (playerHealth)
            {
                if (hasDirection)
                {
                    if (gameObject.transform.position.x < G.I.player.transform.position.x)
                    {
                        playerHealth.left = false;
                    }
                    else
                    {
                        playerHealth.left = true;
                    }
                    
                }
                else
                {
                    playerHealth.left = !G.I.character.isFacingLeft;
                }
                if (teleportAfterHit)
                {
                    playerHealth.teleportAfterHit = true;
                    playerHealth.teleportPosition = teleportPosition;
                }
                else
                {
                    playerHealth.teleportAfterHit = false;
                }
                playerHealth.verticalDamageForce = verticalDamageForce;
                playerHealth.horizontalDamageForce = horizontalDamageForce;
                legnaHealth.GainHealthFromAttack(GainHealthAmountFromAttack);
                playerHealth.DealDamage(G.I.DamageCalculation( legnaHealth.healthPoints,playerHealth.healthPoints, damageAmount, GameSystem.AttackSource.Enemy));
                if (!G.I.character.isDashing)
                {
                    if (disableColliderAfterHit)
                    {
                        gameObject.GetComponent<Collider2D>().enabled = false;
                    }
                    alreadyHit = true; 
                }
            }
        }
    }
}
