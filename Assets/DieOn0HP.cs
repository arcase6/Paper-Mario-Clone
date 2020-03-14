using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOn0HP : MonoBehaviour
{
    
    Unit unit;
    Animator unitAnimator;
    public float DieDelaySeconds = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        unit.HPChanged += CheckForDeath;  
        unitAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void CheckForDeath()
    {
        if(unit.HP <= 0){
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die(){
        unitAnimator.SetTrigger("Die");
        yield return new WaitForSeconds(DieDelaySeconds);
        FinishDeath();
    }

    protected virtual void FinishDeath(){
        GameObject.Destroy(this.gameObject);
    }
    
}
