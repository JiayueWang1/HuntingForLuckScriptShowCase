using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 20;
        public int GainHealthAmountFromAttack = 5;
        private Character character;
        private PlayerHealth playerHealth;
        private Rigidbody2D rb;
        private MeleeAttackManager meleeAttackManager;
        private Vector2 direction;
        private bool collided;
        private bool downwardStrike;

        private void Start()
        {
            character = GetComponentInParent<Character>();
            playerHealth = GetComponentInParent<PlayerHealth>();
            rb = GetComponentInParent<Rigidbody2D>();
            meleeAttackManager = GetComponentInParent<MeleeAttackManager>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if (collision.TryGetComponent(out IDamagebale damagebale)) {
                HandleCollision(damagebale);
            }
        }

        private void HandleCollision(IDamagebale objHealth)
        {
            if(objHealth.IsGiveUpwardForce() && character.input.DownHeld() && !character.isGrounded)
            {
                direction = Vector2.up;
                downwardStrike = true;
                collided = true;
            }
            if(character.input.UpHeld() && !character.isGrounded)
            {
                direction = Vector2.down;
                collided = true;
            }
            if ((!character.input.UpHeld() && character.isGrounded) || 
                (!character.input.UpHeld() && !character.input.DownHeld()))
            {
                if (character.isFacingLeft)
                {
                    direction = Vector2.right;
                }
                else
                {
                    direction = Vector2.left;
                }
                collided = true;
            }

            int damage = G.I.DamageCalculation(playerHealth.healthPoints, objHealth.ReturnHealthPoint(), G.I.playerHealth.meleeDamage,
                GameSystem.AttackSource.Player);
            playerHealth.GainHealthFromAttack(GainHealthAmountFromAttack);
            objHealth.TakeDamage(damage);
            if (playerHealth.luckSkill) {
                objHealth.TakeMidBarDamage(G.I.playerHealth.meleeMidBarDamageDuringLuckSkill);
            }
            int i = Random.Range(1, 5);
            AudioManager.Instance.PlaySFX("Player/Spear Stab (Flesh) "+i);
            StartCoroutine(NoLongerColliding());
        }

        private void HandleMovement()
        {
            if (collided)
            {
                if (downwardStrike)
                {
                    rb.AddForce(direction * meleeAttackManager.upwardsForce);
                }
                else
                {
                    rb.AddForce(direction * meleeAttackManager.defaultForce);
                }
            }
        }

        private IEnumerator NoLongerColliding()
        {
            yield return new WaitForSeconds(meleeAttackManager.movementTime);
            collided = false;
            downwardStrike = false;
        }
    }

}
