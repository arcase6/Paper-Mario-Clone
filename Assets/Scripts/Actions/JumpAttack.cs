using System;
using System.Collections;
using UnityEngine;

public enum JumpAttackType
{
    Standard,
    Multibounce,
    RowBounce,
    EnemyBounce
}


[CreateAssetMenu(fileName = "JumpAttack", menuName = "Paper Mario Clone/Attacks/Jump Attack", order = 0)]
public class JumpAttack : BasicAttack
{
    public float AnticipationDurationSeconds = .3f;
    public JumpAttackType AttackType;

    //[Range(1,20)]
    //public float GravityCoefficient = 9.8f;
    public float JumpPeakOffset = 2f; // height to jump above the enemy

    public InputBuffer InputBufferRef;

    public string AButtonInputName = "Submit";

    [Tooltip("The time window in which an Action command can be pressed")]
    public float ActionPressWindow = .1f;

    [Tooltip("The time in which an A press will invalidate a valid A press (prevents button mashing)")]
    public float InvalidationWindow = .5f;

    public override void Attack(Unit actingUnit, Unit[] targetUnits)
    {
        int jumpLimit = 2;
        if(this.AttackType == JumpAttackType.Multibounce) jumpLimit = 101;
        else if(this.AttackType == JumpAttackType.RowBounce) jumpLimit = targetUnits.Length;

        //trigger jump anticipation and show for a given amount of time
        Rigidbody rigidbody = actingUnit.GetComponent<Rigidbody>();
        actingUnit.PerformAnticipationForAction(AnticipationDurationSeconds, () =>
        {
            JumpOnTarget(actingUnit, targetUnits, 0, rigidbody,0, jumpLimit , 200);
        });



    }

    private void JumpOnTarget(Unit ActingUnit, Unit[] targetUnits, int targetIndex, Rigidbody actingRigidBody, int jumpsPerformed,int jumpLimit, int multibounceQuotient)
    {
        ActingUnit.Animator.SetTrigger("Airborn");
        Unit targetUnit = targetUnits[targetIndex];


        Vector3 targetPosition = targetUnit.HeadPosition.transform.position;
        Vector3 actingUnitPosition = ActingUnit.transform.position;
        Vector3 initialVelocity = CalculateInitialJumpVelocity(actingUnitPosition, targetPosition,JumpPeakOffset);
        //Jump and land on enemies head
        actingRigidBody.constraints = actingRigidBody.constraints & (~RigidbodyConstraints.FreezePosition);
        actingRigidBody.velocity = initialVelocity;
        InputBufferRef.FlushBuffer();
        ActingUnit.WaitForTrigger(() =>
        {
            actingRigidBody.constraints = actingRigidBody.constraints | RigidbodyConstraints.FreezePosition;
            float impactTime = Time.time;

            //play the impact animation and freeze the position
            ActingUnit.SetTriggerAndTriggerCallbackOnReenter("Impact", () =>
            {
                ActionData.ApplyDamage(ActingUnit ,targetUnits[targetIndex],this.BaseAttackDamage);


                bool actionCommandHit = false;
                foreach (InputBufferData inputData in InputBufferRef)
                {
                    float timeDif = Math.Abs(impactTime - inputData.Time);
                    if (timeDif > this.InvalidationWindow)
                        break; //no more commands to check
                    if (inputData.InputName.Equals(AButtonInputName))
                    {
                        if (timeDif > this.ActionPressWindow || actionCommandHit)
                        {
                            actionCommandHit = false; //invalid button press recorded
                            break;
                        }

                        actionCommandHit = true;
                    }
                }
                
                jumpsPerformed ++;
                bool jumpLimitReached = jumpsPerformed >= jumpLimit;
                //calculate new multibounceQuotient
                if(jumpsPerformed > 2 && !jumpLimitReached){
                    multibounceQuotient = (multibounceQuotient * (targetUnit as Enemy).PowerBounceMultiplier) / 100; 
                    int thresholdValue = UnityEngine.Random.Range(0,101);
                    jumpLimitReached = multibounceQuotient < thresholdValue;
                }

                //will do nothing if not performing a row bounce

                if (!actionCommandHit || jumpLimitReached)
                {
                    //end the jump and return to the start

                    Transform landingSpot;
                    if(!actionCommandHit){
                        landingSpot = Unit.GetChildTransformByName(targetUnits[targetIndex].transform.parent,"Forward");
                    }
                    else{
                        landingSpot = Unit.GetChildTransformByName(ActingUnit.transform.parent,"Forward");
                    }
                    
                    float adjustedPeakOffset = ActingUnit.HeadPosition.position.y + (actionCommandHit ? JumpPeakOffset : JumpPeakOffset / 4) - landingSpot.position.y;
                    ActingUnit.StartJumpTo(landingSpot, adjustedPeakOffset,()=>{
                        ActingUnit.StartMoveTo("Standard",() => {
                            ActingUnit.EndTurn();
                        },.5f);
                    });
                }
                else
                {
                    //continue the jump chain (if possible) -- also need to remember that there are different types of jumps
                    targetIndex = targetIndex + 1 % targetUnits.Length;
                    JumpOnTarget(ActingUnit, targetUnits, targetIndex, actingRigidBody, jumpsPerformed, jumpLimit, multibounceQuotient);
                }
                
            });

        });
    }

    public static Vector3 CalculateInitialJumpVelocity(Vector3 startingPosition, Vector3 targetPosition, float jumpPeakOffset)
    {
        float timeInAir;
        return CalculateInitialJumpVelocity(startingPosition,targetPosition,jumpPeakOffset, out timeInAir);
    }

    public static Vector3 CalculateInitialJumpVelocity(Vector3 startingPosition, Vector3 targetPosition, float jumpPeakOffset, out float timeInAir)
    {
        float GravityCoefficient = Physics.gravity.y * -1;

        float jumpPeak = targetPosition.y + jumpPeakOffset;
        float heightDifference = jumpPeak - startingPosition.y;

        float initialYVelocity = Mathf.Sqrt(heightDifference * 2 * GravityCoefficient);

        float timeToPeak = initialYVelocity / GravityCoefficient;
        float timeFromPeakToTarget = Mathf.Sqrt(jumpPeakOffset / GravityCoefficient);
        timeInAir = timeToPeak + timeFromPeakToTarget;

        Vector3 horizontalVelocity = targetPosition / timeInAir - startingPosition / timeInAir;

        return new Vector3(horizontalVelocity.x, initialYVelocity, horizontalVelocity.z);
    }
}