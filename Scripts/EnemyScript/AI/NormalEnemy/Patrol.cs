using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    
    public class Patrol : Node
    {
        // Passing in
        private NormalEnemyCharacter enemyCharacter;
        private bool turnAroundOnCollision;
        private LayerMask collidersToTurnAroundOn;
        private float timeTillMaxSpeed;
        private float maxSpeed;
        private float minPatrolTime;
        private float maxPatrolTime;
        private float minIdleTime;
        private float maxIdleTime;
        
        // Local
        private float patrolCountDown;
        private float idleCountDown;
        private bool patrolling;

        public Patrol(NormalEnemyCharacter enemyCharacter, bool turnAroundOnCollision,
            LayerMask collidersToTurnAroundOn, float timeTillMaxSpeed, float maxSpeed, 
            float minPatrolTime, float maxPatrolTime, float minIdleTime, float maxIdleTime)
            => (this.enemyCharacter, 
                    this.turnAroundOnCollision, this.collidersToTurnAroundOn,
                    this.timeTillMaxSpeed, this.maxSpeed,
                    this.minPatrolTime, this.maxPatrolTime,
                    this.minIdleTime, this.maxIdleTime) =
                (enemyCharacter, turnAroundOnCollision, collidersToTurnAroundOn, timeTillMaxSpeed, maxSpeed, 
                    minPatrolTime, maxPatrolTime, minIdleTime, maxIdleTime);
        
        
        public override NodeState Evaluate()
        {
            if (enemyCharacter.curState != NormalEnemyStates.PATROL)
            {
                enemyCharacter.curState = NormalEnemyStates.PATROL;
                idleCountDown = Random.Range(minIdleTime, maxIdleTime);
                patrolling = false;
            }
            
            if (patrolling && patrolCountDown <= 0)
            {
                idleCountDown = Random.Range(minIdleTime, maxIdleTime);
                patrolling = false;
            }

            if (!patrolling && idleCountDown <= 0)
            {
                patrolCountDown = Random.Range(minPatrolTime, maxPatrolTime);
                RandomFlip();
                patrolling = true;
            }

            if (patrolling)
            {
                CheckForTurnAround();
                enemyCharacter.GeneralMovement(timeTillMaxSpeed, maxSpeed);
                patrolCountDown -= Time.deltaTime;   
            }
            else
            {
                enemyCharacter.GeneralIdle();
                idleCountDown -= Time.deltaTime;
            }
            state = NodeState.SUCCESS;
            return state;
        }

        private void CheckForTurnAround()
        {
            if (!turnAroundOnCollision)
                return;
            if (!enemyCharacter.facingLeft)
            {
                if (enemyCharacter.CollisionCheck(Vector2.right, .5f, collidersToTurnAroundOn))
                {
                    enemyCharacter.Flip();
                }
            }
            else
            {
                if (enemyCharacter.CollisionCheck(Vector2.left, .5f, collidersToTurnAroundOn))
                {
                    enemyCharacter.Flip();
                }
            }
        }

        private void RandomFlip()
        {
            if (Random.Range(0, 2) == 0)
                enemyCharacter.Flip();
        }
    }   
}
