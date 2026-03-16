using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //Base script that houses similar data for Enemies and Player
    public class Health : Managers
    {
        public int maxHealthPoints = 100;
        public int initialHealthPoints;
        [SerializeField] protected bool damageable = true;
        [SerializeField] protected float iFrameTime = .5f;
        //[SerializeField] protected float verticalDamageForce = 50;
        //[SerializeField] protected float horizontalDamageForce = 50;
        [HideInInspector] public int healthPoints;
        [HideInInspector] public bool left;
        [HideInInspector] public bool hit;
        
        protected override void Initialization()
        {
            base.Initialization();
        }

        public virtual void DealDamage(int amount)
        {
            if (damageable && !hit && healthPoints > 0)
            {
                healthPoints -= amount;
                hit = true;
                Invoke("Cancel", iFrameTime);
            }
        }

        //Method that cancels the hit and invulnerable bools so the object can be hit again
        protected virtual void Cancel()
        {
            hit = false;
        }
    }
}