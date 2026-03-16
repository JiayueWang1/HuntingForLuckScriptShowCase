using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.PlayerLoop;

namespace MetroidvaniaTools
{
    public class NormalEnemyBT : AbstractBT
    {
        private NormalEnemyCharacter enemyCharacter;
        private EnemyHealth enemyHealth;
        
        [Header("Patrol Parameters")]
        [SerializeField] private bool turnAroundOnCollision;
        [SerializeField] private float P_TimeTillMaxSpeed;
        [SerializeField] private float P_MaxSpeed;
        [SerializeField] private LayerMask collidersToTurnAroundOn;
        [SerializeField] private float minPatrolTime;
        [SerializeField] private float maxPatrolTime;
        [SerializeField] private float minIdleTime;
        [SerializeField] private float maxIdleTime;
        
        [Header("Chasing Parameters")]
        [SerializeField] private float C_detectRange;
        [SerializeField] private float C_TimeTillMaxSpeed;
        [SerializeField] private float C_MaxSpeed;

        [Header("JumpAttack Parameters")] 
        [SerializeField] private float JA_detectRange;
        [SerializeField] private float JA_jumpHeight;
        [SerializeField] private float JA_preMoveTime;
        [SerializeField] private float JA_postMoveTime;
        [SerializeField] private float JA_distanceOffset;
        [SerializeField] private float JA_maxDistance;

        [Header("Stager Parameters")] 
        [SerializeField] private float knockBackDistance;
        [SerializeField] private float knockBackHeight;
        [SerializeField] private float stagerTime;

        protected override void Initialization()
        {
            enemyCharacter = GetComponent<NormalEnemyCharacter>();
            enemyHealth = GetComponent<EnemyHealth>();
            base.Initialization();
        }

        protected override Node SetupTree()
        {
            Node chase = new Chase(enemyCharacter, C_detectRange, C_TimeTillMaxSpeed, C_MaxSpeed);
            Node patrol = new Patrol(enemyCharacter, turnAroundOnCollision, collidersToTurnAroundOn,
                P_TimeTillMaxSpeed, P_MaxSpeed, minPatrolTime, maxPatrolTime, minIdleTime, maxIdleTime);
            Node jumpattack = new JumpAttack(enemyCharacter,JA_detectRange,  JA_jumpHeight,JA_preMoveTime, JA_postMoveTime, JA_distanceOffset, JA_maxDistance);
            Node stagger = new Stagger(enemyCharacter, enemyHealth, knockBackDistance, knockBackHeight, stagerTime);
            
            Node root = new Selector(new List<Node>
            {
                stagger,
                jumpattack,
                chase,
                patrol
            });
            return root;
        }
    }   
}
