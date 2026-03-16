using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class LegnaBT : AbstractBT
    {
        private LegnaCharacter character;
        
        [Header("硬直")]
        [SerializeField] private float P1_stunTime;
        [SerializeField] private float P2_stunTime;
        
        [Header("追击")]
        [SerializeField] private float C_TimeTillMaxSpeed;
        [SerializeField] private float C_MaxSpeed;
        
        [Header("跳劈")]
        [SerializeField] private float JC_SuccessDistance;
        [SerializeField] private float JA_jumpHeight;
        [SerializeField] private float JA_distanceOffset;
        [SerializeField] private float JA_maxDistance;
        
        [Header("龙车")]
        [SerializeField] private float SA_SlowSpinTime;
        [SerializeField] private float SA_SlowMaxSpeed;
        [SerializeField] private float SA_FastSpinTime;
        [SerializeField] private float SA_FastMaxSpeed;

        [Header("闪避")] 
        [SerializeField] private float Dodge_distance;
        [SerializeField] private float GreatDodge_distance;
        
        [Header("平A")] 
        [SerializeField] private float NC_SuccessDistance;
        [SerializeField] private float N2_distance;
        
        [Header("二阶段平A")]
        [SerializeField] private float SL2_JumpDistance;
        [SerializeField] private float SL3_JumpDistance;
        
        [Header("防反")]
        [SerializeField] private float GC_maxGuardTime;
        [SerializeField] private float GC_maxDistance;
        [SerializeField] private float GC_closeDistance;
        [SerializeField] private float GC_dodgeForce;

        [Header("闪现大跳")] [SerializeField] private float TA_interval;
        
        [Header("时代变了")] [SerializeField] private float BF_distance;
        
        [Header("Excalibur!")]        
        [SerializeField] private float EX_maxChargeTime;
        [SerializeField] private float EX_minDistance;
        [SerializeField] private float EX_velocityOffset;
        [SerializeField] private float EX_maxDistanceToApply;
        [SerializeField] private float EX_normalRange;
        protected override void Initialization()
        {
            character = GetComponent<LegnaCharacter>();
            base.Initialization();
        }

        protected override Node SetupTree()
        {
            Node sleep = new LSleep(character);
            Node p1_stun = new LStun(character, P1_stunTime);
            Node p2_stun = new LStun(character, P2_stunTime);
            Node death = new LDeath(character);
            
            Node chaseForJump = new LChase(character, C_TimeTillMaxSpeed, C_MaxSpeed, JC_SuccessDistance);
            Node chaseForNormalAttack = new LChase(character, C_TimeTillMaxSpeed, C_MaxSpeed, NC_SuccessDistance);
            Node chaseForEXAttack = new LChase(character, C_TimeTillMaxSpeed, C_MaxSpeed, EX_normalRange);
            Node jumpAttack = new LJumpAttack(character, JA_jumpHeight, JA_distanceOffset, JA_maxDistance);
            Node spinAttack = new LSpinAttack(character, SA_SlowSpinTime, SA_SlowMaxSpeed, SA_FastSpinTime, SA_FastMaxSpeed);
            Node crossAttack = new LCrossAttack(character, N2_distance);
            Node guardCounter = new LGuardCounter(character, GC_maxGuardTime, GC_maxDistance, GC_closeDistance, GC_dodgeForce);
            Node dodge = new LBackToCenter(character, Dodge_distance);
            Node greatDodge = new LBackToCenter(character, GreatDodge_distance);
            Node shortIdle = new LIdle(character, .5f);
            Node longIdle = new LIdle(character, 1.5f);
            Node randomShortIdle = new RandomPicker(new List<Node>
            {
                shortIdle,
                new LIdle(character, 0)
            });
            Node twinkleAttack = new LTwinkleAttack(character, TA_interval);
            Node slashAttack2c = new LSlashAttack2C(character, SL2_JumpDistance);
            Node slashAttack4c = new LSlashAttack4C(character, SL2_JumpDistance, SL3_JumpDistance);
            Node fire = new LFire(character);
            Node backFire = new LBackFire(character, BF_distance);
            Node calibur = new LCalibur(character, EX_maxChargeTime, EX_minDistance, EX_velocityOffset, EX_maxDistanceToApply);
            Node phaseChange = new LPhaseChange(character);
            
            // Phase 1 Sequences
            Node p1_crossAttackSequence = new Sequence(new List<Node>
            {
                chaseForNormalAttack,
                crossAttack,
                shortIdle
            });
            
            Node p1_chaseAndJumpAttackSequence = new Sequence(new List<Node>
            {
                chaseForJump,
                jumpAttack,
                longIdle
            });;

            Node p1_spinAttackSequence = new Sequence(new List<Node>
            {
                spinAttack,
                longIdle
            });
            
            // Phase 1 Behaviours
            Node p1_veryShortBehavior = new RandomSelector(new List<Node>
            {
                // new LIdle(character, 3),
                p1_crossAttackSequence,
                dodge,
                guardCounter
            });
            Node p1_shortBehavior = new RandomSelector(new List<Node>
            {
                // new LIdle(character, 3),
                p1_crossAttackSequence,
                dodge,
                guardCounter
            });
            Node p1_longBehavior = new RandomSelector(new List<Node>
            {
                // new LIdle(character, 3),
                p1_chaseAndJumpAttackSequence,
                p1_spinAttackSequence
            });
            Node p1_unSeenBehavior = dodge;
            
            // Phase1 Detectors
            Node p1_unSeenDetector = new DetectPlayer(character, 0, p1_unSeenBehavior);
            Node p1_veryShortDetector = new DetectPlayer(character, 1, p1_veryShortBehavior);
            Node p1_shortDetector = new DetectPlayer(character, 2, p1_shortBehavior);
            Node p1_longDetector = new DetectPlayer(character, 3, p1_longBehavior);
            
            Node phase1Behavior = new Selector(new List<Node>
            {
                p1_unSeenDetector,
                p1_veryShortDetector,
                p1_shortDetector,
                p1_longDetector,
                p1_stun
            });

            Node phase1 = new PhaseChecker(character, 1, phase1Behavior);
            
            // Phase2 Sequences
            Node caliburSequence1 = new Sequence(new List<Node>
            {
                calibur,
                dodge,
                twinkleAttack,
                longIdle
            });
            Node caliburSequence2 = new Sequence(new List<Node>
            {
                calibur,
                chaseForEXAttack,
                slashAttack4c,
                longIdle
            });

            Node randomCalibur = new RandomPicker(new List<Node>
            {
                caliburSequence1,
                caliburSequence2,
            });

            Node randomSlash = new RandomPicker(new List<Node>
            {
                slashAttack2c,
                slashAttack4c
            });

            Node slashSequence = new Sequence(new List<Node>
            {
                chaseForNormalAttack,
                randomSlash,
                randomShortIdle
            });

            Node closeRandCalibur = new Sequence(new List<Node>
            {
                greatDodge,
                randomCalibur
            });

            Node p2_chaseAndJumpAttack = new Sequence(new List<Node>
            {
                chaseForJump,
                jumpAttack,
                dodge,
                jumpAttack,
                dodge,
                jumpAttack,
                shortIdle
            });
            
            Node p2_spinAttack = new Sequence(new List<Node>
            {
                spinAttack,
                dodge,
                spinAttack,
                shortIdle
            });
            
            
            // Phase 2 Behaviours
            Node p2_veryShortBehavior = new RandomSelector(new List<Node>
            {
                //shortIdle,
                closeRandCalibur,
                backFire,
                dodge
            });
            Node p2_shortBehavior = new RandomSelector(new List<Node>
            {
                //shortIdle,
                closeRandCalibur,
                fire,
                backFire,
                slashSequence,
                twinkleAttack,
                dodge
            });
            Node p2_longBehavior = new RandomSelector(new List<Node>
            {
                // shortIdle,
                randomCalibur,
                slashSequence,
                twinkleAttack,
                fire,
                p2_chaseAndJumpAttack,
                p2_spinAttack
            });
            Node p2_unSeenBehavior = new RandomSelector(new List<Node>
            {
                dodge,
                backFire
            });
            
            // Phase2 Detectors
            Node p2_unSeenDetector = new DetectPlayer(character, 0, p2_unSeenBehavior);
            Node p2_veryShortDetector = new DetectPlayer(character, 1, p2_veryShortBehavior);
            Node p2_shortDetector = new DetectPlayer(character, 2, p2_shortBehavior);
            Node p2_longDetector = new DetectPlayer(character, 3, p2_longBehavior);
            
            Node phase2Behavior = new Selector(new List<Node>
            {
                p2_unSeenDetector,
                p2_veryShortDetector,
                p2_shortDetector,
                p2_longDetector,
                p2_stun
            });

            Node phase2 = new PhaseChecker(character, 2, phase2Behavior);


            Node root = new Sequence(new List<Node>
            {
                sleep,
                phase1,
                phaseChange,
                phase2,
                death
            });
            return root;
        }
    }
}

