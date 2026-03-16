using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class WeaponAttackManager : Abilities
    {
        [SerializeField] List<WeaponTypes> weaponTypes;
        [SerializeField] private GameObject gunMuzzle;
        [SerializeField] Transform gunBarrel;
        [SerializeField] Transform gunRotation;

        public List<GameObject> currentPool = new List<GameObject>();
        public float shootInterval;
        public float disableTime;
        [HideInInspector] public GameObject currentProjectile;
        private GameObject projectileParentFolder;
        private bool keepShooting;
        private float shootCountDown;
        private float disableCountDown;

        protected override void Initialization()
        {
            base.Initialization();
            foreach(WeaponTypes weapon in weaponTypes) 
            {
                GameObject newPool = new GameObject();
                projectileParentFolder = newPool;
                objectPooler.CreatePool(weapon, currentPool, projectileParentFolder);
            }
            gunMuzzle.GetComponent<ParticleSystem>().Stop();
            character.isShooting = false;
            keepShooting = false;
        }

        protected virtual void Update()
        {
            
        }
        // Shoot bullet
        public void RangeFire() {
            if(!character.isMeleeAttacking && !character.isDashing && !character.isGettingHit && !character.isDead)
            {
                if (!character.isShooting)
                {
                    FireWeapon();
                }
                else 
                {
                    keepShooting = true;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (character.isShooting)
            {
                if (shootCountDown <= 0)
                {
                    if (!keepShooting)
                    {
                        character.isShooting = false;
                    }
                    else 
                    {
                        keepShooting = false;
                        FireWeapon();
                    }
                }
                else 
                {
                    shootCountDown -= Time.deltaTime;
                    // aimTarget.position = Vector2.Lerp(aimStartPosition.position, aimEndPosition.position, 1 - (shootCountDown / recoilAnimTime));
                }
            }

            if (!character.isShooting && anim.GetBool("Shooting")) 
            {
                if (disableCountDown <= 0)
                {
                    // aimingRightHand.enabled = false;
                    anim.SetBool("Shooting", false);
                }                     
                else
                {
                    disableCountDown -= Time.deltaTime;
                }
            }
        }

        protected virtual void FireWeapon()
        {
            G.I.playerHealth.GainCurrentHealth(-G.I.playerHealth.rangeDamageToPlayerSelf);
            character.isShooting = true;
            shootCountDown = shootInterval;
            disableCountDown = disableTime;
            anim.SetBool("Shooting", true);
            anim.SetTrigger("Shoot");
            AudioManager.Instance.PlaySFX("gunShot");
            
        }

        public void TriggerPlaceProjectile()
        {
            currentProjectile = objectPooler.GetObject(currentPool);
            if(currentProjectile != null) 
            {
                Invoke("PlaceProjectile", .1f);
            }
        }

        protected virtual void PlaceProjectile()
        {
            currentProjectile.transform.position = gunMuzzle.transform.position;
            currentProjectile.transform.rotation = gunMuzzle.transform.rotation;
            gunMuzzle.GetComponent<ParticleSystem>().Play();
            currentProjectile.SetActive(true);
            if(!character.isFacingLeft)
            {
                currentProjectile.GetComponent<Projectile>().left = false;
            }
            else
            {
                currentProjectile.GetComponent<Projectile>().left = true;
            }
            currentProjectile.GetComponent<Projectile>().fired = true;
            
        }
    }
}
