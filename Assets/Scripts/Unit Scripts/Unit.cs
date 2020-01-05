using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}

public enum UnitType
{
    Ground,
    Air,
    Ceiling,
    Ally
}

public abstract class Unit : MonoBehaviour
{
    public bool CanAct;
    public UnitRuntimeSet UnitSet;
    public event System.Action HPChanged;

    public UnitData Stats;
    public UnitType UnitType
    {
        get => Stats.UnitType;
        set => Stats.UnitType = value;
    }

    public string Name
    {
        get => Stats.Name;
        set => Stats.Name = value;
    }
    public virtual int HP
    {
        get => Stats.HP;
        set => Stats.HP = value;
    }

    public int MaxHP
    {
        get => Stats.MaxHP;
        set => Stats.MaxHP = value;
    }



    public int BaseDefense
    {
        get => Stats.BaseDefense;
        set => Stats.BaseDefense = value;
    }

    public int AttackBonus = 0;
    public int DefenseBonus = 0;
    public float WalkSpeed = 8;
    public Animator Animator;
    public Transform HealingHeart;
    public Transform DamageBubble;
    public EffectSpawner EffectSpawner;
    public Transform HeadPosition;
    public Transform FootPosition;

    private StringMediator HeartText;
    private StringMediator DamageText;

    private AnimationClipOverrides animationClipOverrides = new AnimationClipOverrides(10);
    private AnimatorOverrideController overrideController;

    private Action RegisteredTriggerCallback;

    public void WaitForTrigger(Action callback)
    {
        if (RegisteredTriggerCallback != null)
        {
            Debug.Log("Warning - A callback was registered, but a previous callback was there. Overriding " + callback.Method.Name);
        }
        this.RegisteredTriggerCallback = callback;
    }

    private void OnCollisionEnter(Collision other)
    {
        Unit otherUnit = other.gameObject.GetComponent<Unit>();
        if (otherUnit != null)
        {
            //handle collision here
            if (this.RegisteredTriggerCallback != null)
            {
                var localRef = this.RegisteredTriggerCallback;
                RegisteredTriggerCallback = null;
                localRef();
            }
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (this.Animator == null)
            this.Animator = GetComponent<Animator>();
        overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
        overrideController.GetOverrides(animationClipOverrides);
        HeartText = HealingHeart?.GetComponentInChildren<StringMediator>();
        DamageText = DamageBubble?.GetComponentInChildren<StringMediator>();
    }

    /// <summary>
    /// This is a utility function that is used to call a callback when a state is returned to.
    /// The fuction sets a trigger (presumably to leave the current state). Intended for use with some kind of looping state behavior.!--
    /// Do not use this function if you do not think the current state will be returned to.
    /// </summary>
    /// <param name="triggerName"> The trigger to set</param>
    /// <param name="callback">The callback to trigger upon re-entry</param>
    public void SetTriggerAndTriggerCallbackOnReenter(string triggerName,Action callback){

        StartCoroutine(WaitForStateReturn(triggerName,callback));
    }

    private IEnumerator WaitForStateReturn(string triggerName,Action callback){
        int startingStateHash = Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        bool hasLeftState = false;
        Animator.SetTrigger(triggerName);
        while(hasLeftState == false || Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != startingStateHash){
            hasLeftState = hasLeftState || Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != startingStateHash;
            yield return null;
        }
        callback();




    }


    public abstract void BeginTurn();

    public Transform GetSiblingTransform(string transformName)
    {
        return GetChildTransformByName(transform.parent, transformName);
    }

    public static Transform GetChildTransformByName(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            string childName = parent.GetChild(i).name;
            if (childName.Contains(name))
                return parent.GetChild(i);
        }
        return null;
    }

    public void StartMoveTo(string positionName, System.Action finishMoveCallback = null,float movementDelaySeconds = 0f)
    {
        Transform goal = this.GetSiblingTransform(positionName);
        StartMoveTo(goal, finishMoveCallback,movementDelaySeconds);
    }
    public void StartMoveTo(Transform goal, System.Action finishMoveCallback = null, float movementDelaySeconds = 0f)
    {
        StartCoroutine(MoveTo(goal.position, finishMoveCallback,movementDelaySeconds));
    }


    public void StartJumpTo(Transform goal,float peakOffset, System.Action finishJumpCallback = null){
        StartCoroutine(JumpTo(goal,peakOffset, finishJumpCallback));
    }

    protected virtual IEnumerator JumpTo(Transform goal,float peakOffset, System.Action finishMoveCallback)
    {
        
        Rigidbody rb = this.GetComponent<Rigidbody>();
        float timeRemaining;
        Vector3 takeOffVelocity = JumpAttack.CalculateInitialJumpVelocity(this.FootPosition.position,goal.position,peakOffset, out timeRemaining);
        rb.constraints = rb.constraints & (~RigidbodyConstraints.FreezePosition);
        rb.velocity = takeOffVelocity;
        
        while(timeRemaining > 0){
            yield return null;
            timeRemaining -= Time.deltaTime;

        }
        Animator.SetTrigger("Grounded");
        if (finishMoveCallback != null)
        {
            finishMoveCallback();
        }
    }


    //Always takes at least one frame
    protected virtual IEnumerator MoveTo(Vector3 goal, System.Action finishMoveCallback, float movementDelaySeconds)
    {
        if(movementDelaySeconds > 0)
            yield return new WaitForSeconds(movementDelaySeconds);
        //approach goal
        goal.y = transform.position.y;
        float distanceRemaining = 0;
        do
        {
            //calculate ammount to go
            Vector3 difference = goal- transform.position;
            Vector3 directionVector = Vector3.Normalize(difference);
            distanceRemaining = Vector3.Magnitude(difference);

            float speed = WalkSpeed; //to-do: make acceleration for smoother movement
            float direction = (Vector3.Dot(directionVector, transform.forward) >= 0 ? 1 : 0); //check the direction
            float velocity = WalkSpeed;
            this.Animator.SetFloat("VelocityNormalized", 1);
            this.Animator.SetFloat("TurnOrientation", direction);
            
            //move towards end
            Vector3 offset = directionVector * (speed * Time.deltaTime);
            if (distanceRemaining < Vector3.Magnitude(offset)){
                transform.position = goal;
                break;
            }
            else{
                this.transform.position = transform.position + offset;
            }
            yield return null;
        } while (distanceRemaining > Mathf.Epsilon);

        this.Animator.SetFloat("VelocityNormalized", 0f);

        if (finishMoveCallback != null)
        {
            finishMoveCallback();
        }
    }

    public void PerformAnticipationForAction(float anticipationDurationSeconds, Action callback)
    {
        StartCoroutine(PerformAnticipationForActionEnumerator(anticipationDurationSeconds, callback));
    }

    private IEnumerator PerformAnticipationForActionEnumerator(float anticipationDurationSeconds, Action callback)
    {
        this.Animator.SetTrigger("Anticipation");
        bool isPlayer = this as Player != null;
        float timeElapsed = 0;
        Debug.Log(anticipationDurationSeconds);
        while (anticipationDurationSeconds > timeElapsed)
        {
            if (isPlayer)
            {
                if (Input.GetButtonDown("Submit"))
                {
                    break;
                }
            }
            yield return null;
            timeElapsed += Time.deltaTime;
        }
        callback();

    }

    private System.Action ApplyItemCallback;

    public void UseItem(List<AnimationRemapping> remappings, System.Action applyItemCallback)
    {
        foreach (AnimationRemapping animRemap in remappings)
        {
            this.animationClipOverrides[animRemap.OriginalName] = animRemap.ReplacementClip;
        }
        this.overrideController.ApplyOverrides(this.animationClipOverrides);
        this.ApplyItemCallback = applyItemCallback; // triggered by event (for flexibility)
        SetAnimatorTrigger("Use Item", 0);
    }

    //Where the item effect is actually activated
    //This method MUST be called in the timeline to trigger
    public void ApplyItemEffect()
    {
        this.ApplyItemCallback();
    }

    public void SetAnimatorTrigger(string triggerName, float timeDelay)
    {
        if (timeDelay == 0)
            Animator.SetTrigger(triggerName);
        else
            StartCoroutine(SetTriggerAfterDelay(triggerName, timeDelay));
    }

    public IEnumerator SetTriggerAfterDelay(string triggerName, float timeDelay)
    {
        float timeRemaining = timeDelay;
        while (timeRemaining > 0)
        {
            yield return null;
            timeRemaining -= Time.deltaTime;
        }
        Animator.SetTrigger(triggerName);
    }

    public void SetAnimatorTrigger(string triggerName)
    {
        this.Animator.SetTrigger(triggerName);
    }

    /* Methods for changing HP, FP, Attack, and Defense (animations are played when called) */

    public void ChangeHP(int amount)
    {
        if (amount > 0)
        {
            HP = Mathf.Min(HP + amount, MaxHP);
            //start heart efect
            if (HeartText)
            {
                HeartText.Value = amount.ToString();
                HealingHeart.gameObject.SetActive(true);
            }
            //note that the animators "React To Healing" state will use up any extra triggers when returning so there is no need to worry about FP etc. double triggering
            this.Animator.SetTrigger("Healing");
        }
        else
        {
            HP = Mathf.Max(HP + amount, 0);
            if (DamageText)
            {
                DamageText.Value = Mathf.Min(amount * -1, 99).ToString();
                DamageBubble.gameObject.SetActive(true);
            }

        }
        RaiseHPChangedEvent();
    }

    /// <summary>
    /// A method used to expose event to classes that inherit
    /// </summary>
    protected void RaiseHPChangedEvent()
    {
        HPChanged?.Invoke();
    }

    public void ChangeAttack(int amount)
    {
        if (amount > 0)
        {
            AttackBonus += amount;
        }
        else
        {
            AttackBonus -= amount;
        }
    }

    public void ChangeDefense(int amount)
    {
        if (amount > 0)
        {
            DefenseBonus += amount;
        }
        else
        {
            DefenseBonus -= amount;
        }
    }

    public void PollForEffectsToEnd(System.Action onPollingFinished)
    {
        StartCoroutine(PollEffectsActive(onPollingFinished));
    }
    public IEnumerator PollEffectsActive(System.Action onPollingFinished)
    {
        while (CheckEffectsActive())
        {
            yield return null;
        }
        onPollingFinished.Invoke();
    }

    protected virtual bool CheckEffectsActive()
    {
        return (HealingHeart && HealingHeart.gameObject.activeInHierarchy) || EffectSpawner.enabled;
    }

    //called when the Unit finishes a move
    //used to end the turn and whatever else needs to be done
    public abstract void EndTurn();

    protected virtual void OnEnable()
    {
        if (UnitSet)
            this.UnitSet.AddUnit(this);
        else
            Debug.Log(this.Name + " has not been added to a unit set which is necessary for turn management");
    }

    protected virtual void OnDisable()
    {
        if (UnitSet)
            this.UnitSet.RemoveUnit(this);
        else
            Debug.Log(this.Name + "has not been added to a unit set which is necessary for turn management");
    }

}
