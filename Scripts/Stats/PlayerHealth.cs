using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MetroidvaniaTools
{
    //Health script specific to Player; it houses a lot of extra data that normally an Enemy wouldn't need
    public class PlayerHealth : Health {
        public static PlayerHealth Instance;

        private void Awake() {
            Instance = this;
        }

        //How long the time value needs to be adjusted to better visualize when the player is hit; this is an effects feature, not needed for actual gameplay
        [SerializeField] protected float slowDownTimeAmount;
        //How much the time value needs to be adjusted to better visualize when the player is hit; this is an effects feature, not needed for actual gameplay
        [SerializeField] protected float slowDownSpeed;
        [SerializeField] protected float forceApplyTime;
        [Header("Recover Part")]
        [SerializeField] protected int recoverAmount;
        [SerializeField] protected float recoverTimeAfterGainHealth;
        [SerializeField] protected float recoverTimeAfterHit;
        [SerializeField] protected float recoverInterval;

        //A reference to all the different sprites that make up the Player; this is used to make the Player slightly transparent when hit to visualize the Player received damage
        protected SpriteRenderer[] sprites;
        // protected Image deadScreenImage;
        // protected Text deadScreenText;
        protected float originalTimeScale;
        

        [HideInInspector] public bool teleportAfterHit;
        [HideInInspector] public Transform teleportPosition;
        [HideInInspector] public float verticalDamageForce;
        [HideInInspector] public float horizontalDamageForce;
        
        public int playerCurrentLuckValue;
        private bool applyForce;
        public float recoverTimeCountdown;
        private Rigidbody2D rb;
        private bool gainHealthFromAttack = false;
        public string playerName;
        
        [Header ("Luck Skill Part")]
        public bool luckSkill;
        public float BurningLuckTime;
        public float BurningReduceMaximumProportion;
        public int meleeMidBarDamageDuringLuckSkill = 20;
        public int minimumLuckBar = 10;
        [Header("Damage Part")] 
        public int meleeDamage = 20;
        public int rangeDamage = 20;
        public int rangeDamageToPlayerSelf = 5;
        public int rangeDamageToBossMidBarValue = 10;
        [Header("Luck Skill")] 
        public GameObject BurnLuck;
        public GameObject BurnLuckState;
        protected override void Initialization()
        {
            base.Initialization();
            sprites = GetComponentsInChildren<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            playerCurrentLuckValue = maxHealthPoints / 2;
            healthPoints = playerCurrentLuckValue;
            G.I.UIMain.UpdateCurrentPlayer(playerName, healthPoints, maxHealthPoints, playerCurrentLuckValue);
            // deadScreenImage = uiManager.deadScreen.GetComponent<Image>();
            // deadScreenText = uiManager.deadScreen.GetComponentInChildren<Text>();
        }

        protected virtual void Update()
        {
            character.isGettingHit = hit;
        }

        private void FixedUpdate()
        {
            HandleIFrames();
            HandleDamageMovement();
            HandleRecovery();
            HandleDeath();
        }

        public override void DealDamage(int amount)
        {
            if (!character.isDead)
            {
                if (hit || character.isDashing)
                {
                    return;
                }
                base.DealDamage(amount);
                CharacterGetHit();
                if (healthPoints <= 0)
                {
                    character.isDead = true;
                    healthPoints = 0;
                }
                
                originalTimeScale = Time.timeScale;
                hit = true;
                //Slows down time to the damage speed
                Time.timeScale = slowDownSpeed;
                applyForce = true;
                Invoke("Cancel", iFrameTime);
                Invoke("HitCancel", slowDownTimeAmount);
                Invoke("ForceCancel", forceApplyTime);
            }
            G.UI.playerHealthState.playerHealth = healthPoints;
            G.UI.playerHealthState.MarkDirty();
        }

        private void CharacterGetHit()
        {
            character.isGettingHit = true;
            character.isJumping = false;
            character.isShooting = false;
            character.isMeleeAttacking = false;
            character.anim.SetBool("GettingHit", true);
            GetComponentInChildren<MeleeWeapon>().gameObject.GetComponent<Animator>().SetTrigger("CANCLE");
            character.input.DisableInput();
        }

        public void HandleDamageMovement()
        {
            if (hit)
            {
                if (applyForce)
                {
                    rb.velocity = Vector2.zero;
                    //Handles vertical and horizontal knockback depending on what direction the Player is facing
                    rb.AddForce(Vector2.up * verticalDamageForce);
                    if (!left)
                    {
                        rb.AddForce(Vector2.right * horizontalDamageForce);
                    }
                    else
                    {
                        rb.AddForce(Vector2.left * horizontalDamageForce);
                    }
                }
                else
                {
                    if (teleportAfterHit)
                    {
                        teleportAfterHit = false;
                        character.rb.velocity = Vector2.zero;
                        character.transform.position = teleportPosition.position;
                    }
                }
            }

        }

        protected virtual void HandleIFrames()
        {
            Color spriteColors = new Color();
            if (hit)
            {
                foreach (SpriteRenderer sprite in sprites)
                {
                    spriteColors = sprite.color;
                    spriteColors.a = .5f;
                    sprite.color = spriteColors;
                }
            }
            else
            {
                foreach (SpriteRenderer sprite in sprites)
                {
                    spriteColors = sprite.color;
                    spriteColors.a = 1;
                    sprite.color = spriteColors;
                }
            }
        }

        protected virtual void HandleRecovery()
        {
            if (character.isDead)
            {
                return;
            }

            if (gainHealthFromAttack) {
                recoverTimeCountdown = healthPoints < playerCurrentLuckValue? recoverInterval : recoverTimeAfterGainHealth;
                gainHealthFromAttack = false;
            }
        
            if (hit) {
                recoverTimeCountdown = healthPoints > playerCurrentLuckValue ? recoverInterval : recoverTimeAfterHit;
            }
            else
            {
                if (recoverTimeCountdown > 0)
                {
                    recoverTimeCountdown -= Time.deltaTime;
                }
                else
                {
                    recoverTimeCountdown = recoverInterval;
                    if (healthPoints > playerCurrentLuckValue) {
                        GainCurrentHealth(Mathf.Max(-recoverAmount, playerCurrentLuckValue - healthPoints));
                    } else {
                        GainCurrentHealth(Mathf.Min(recoverAmount, playerCurrentLuckValue - healthPoints));
                    }
                }
            }
        }

        protected virtual void HitCancel()
        {
            Time.timeScale = originalTimeScale;
        }

        protected virtual void ForceCancel()
        {
            applyForce = false;
        }

        public virtual void GainCurrentHealth(int amount)
        {
            if (luckSkill) {
                amount = Mathf.Clamp(amount, 0, amount);
            }
            if (amount < 0) {
                recoverTimeCountdown = healthPoints > playerCurrentLuckValue ? recoverInterval : recoverTimeAfterHit;
            }

            healthPoints = Mathf.Clamp(healthPoints + amount, 0, maxHealthPoints);
            G.UI.playerHealthState.playerHealth = healthPoints;
            G.UI.playerHealthState.MarkDirty();
            if (healthPoints == 0) {
                character.isDead = true;
            }
        }

        public void GainHealthFromAttack(int amount) {
            GainCurrentHealth(amount);
            gainHealthFromAttack = true;
        }
        

        protected override void Cancel()
        {
            if (!character.isDead)
            {
                base.Cancel();
                character.isGettingHit = false;
                character.anim.SetBool("GettingHit", false);
                character.input.EnableInput();
            }
        }

        private void HandleDeath()
        {
            if (character.isDead)
            {
                if (character.isGrounded)
                {
                    character.anim.SetBool("Dying", true);
                }
                StartCoroutine(Dead());
            }
        }

        private IEnumerator Dead() {
            G.UI.overlayUIType = OverlayUIType.DeadScreen;
            G.UI.MarkDirty();
            yield return new WaitForSeconds(5);
            character.gameObject.SetActive(false);
            
        }

        public void ModifyPlayerLuckBarValue(int amount) {
            playerCurrentLuckValue = Mathf.Clamp(playerCurrentLuckValue + amount,  minimumLuckBar , maxHealthPoints);
            G.UI.playerHealthState.playerCurrentLuckValue = playerCurrentLuckValue;
            G.UI.playerHealthState.MarkDirty();
        }
    }
}