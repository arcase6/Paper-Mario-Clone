using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSysInput : MonoBehaviour
{
    private TargetingSystem TargetingSystem;
    public float InputCooldownSeconds = .3f;
    private float CooldownAmountRemaining = 0;

    public float StartUpCooldown = 1f;


    // Start is called before the first frame update
    void OnEnable()
    {
        CooldownAmountRemaining = StartUpCooldown + Time.deltaTime;
        TargetingSystem = GetComponent<TargetingSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(CooldownAmountRemaining < 0){
            CooldownAmountRemaining -= Time.deltaTime;
            CooldownAmountRemaining = Mathf.Max(CooldownAmountRemaining, 0f);
        }

        if (Input.GetButtonDown("Submit"))
            TargetingSystem.ExecuteCurrentlySelectedActions();
        if (Input.GetButtonDown("Cancel"))
            TargetingSystem.CloseMenuAndReOoenCaller();

        if (TargetIsUnableToBeChanged())
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontal) > .7f)
        {
            CooldownAmountRemaining = InputCooldownSeconds;
            if (horizontal > 0) TargetingSystem.NextTarget();
            else TargetingSystem.PrevTarget();
        }

        
    }

    private bool TargetIsUnableToBeChanged()
    {
        return TargetingSystem.TargetingIsLocked;
    }
}
