using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace MetroidvaniaTools
{
    public class Character : MonoBehaviour {
        public static Character Instance;

        public void Awake() {
            Instance = this;
        }

        [HideInInspector] public bool isFacingLeft;
        [HideInInspector] public bool isJumping;
        [HideInInspector] public bool isGrounded;
        [HideInInspector] public bool isShooting;
        [HideInInspector] public bool isMeleeAttacking;
        [HideInInspector] public bool isGettingHit;
        [HideInInspector] public bool isDashing = false;
        [HideInInspector] public bool isDead = false;

        [HideInInspector] public InputManager input;
        [HideInInspector] public Rigidbody2D rb;

        [HideInInspector] public Collider2D col;
        [HideInInspector] public Animator anim;
        [HideInInspector] public HorizontalMovement movement;
        [HideInInspector] public Jump jump;
        [HideInInspector] public Dash dash;
        [HideInInspector] public ObjectPooler objectPooler;
        [HideInInspector] public WeaponAttackManager WAM;
        [HideInInspector] public MeleeAttackManager MAM;
        [HideInInspector] public PlayerHealth health;
        private Vector2 facingLeft;
        
        // Start is called before the first frame update
        void Start()
        {
            Initialization();
        }

        protected virtual void Initialization()
        {
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            movement = GetComponent<HorizontalMovement>();
            jump = GetComponent<Jump>();
            dash = GetComponent<Dash>();
            input = GetComponent<InputManager>();
            anim = GetComponentInChildren<Animator>();
            objectPooler = ObjectPooler.Instance;
            facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);
            MAM = GetComponent<MeleeAttackManager>();
            WAM = GetComponent<WeaponAttackManager>();
            health = GetComponent<PlayerHealth>();
        }

        protected virtual void Flip()
        {
            if (isFacingLeft)
            {
                transform.localScale = facingLeft;
            }
            else
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,transform.localScale.z);
            }
        }

        protected virtual bool CollisionCheck(Vector2 direction, float distance, LayerMask collision)
        {
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int numHits = col.Cast(direction, hits, distance);
            for (int i = 0; i < numHits; i ++)
            {
                if ((1 << hits[i].collider.gameObject.layer & collision) !=  0)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool Falling(float velocity)
        {
            if (!isGrounded && rb.velocity.y < velocity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void FallSpeed(float speed)
        {
            rb.velocity = new Vector2(rb.velocity.x, (rb.velocity.y * speed));
        }
    }
}
