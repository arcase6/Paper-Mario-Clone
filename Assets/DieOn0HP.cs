using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOn0HP : MonoBehaviour
{
    
    Unit unit;
    Animator unitAnimator;
    public float DieDelaySeconds = 1.2f;
    private float DieCooldown = 0;
    private bool DieReceived = false;
    private bool HasDied = false;
    
    public string DeathAnimName = "SpinDie";
    private float DeathAnimDuration;
    public ParticleSystem DeathParticles;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        unit.HPChanged += CheckForDeath;  
        unitAnimator = GetComponentInChildren<Animator>();
        DeathAnimDuration = 1; //default value
        foreach(AnimationClip clip in unitAnimator.runtimeAnimatorController.animationClips){
            if(clip.name == DeathAnimName){
                DeathAnimDuration = clip.length;
            }
        }
    }

    void Update(){
        if(DieReceived){
            if(DieCooldown <= 0 && HasDied == false){
                HasDied = true;
                unitAnimator.SetTrigger("Die");
                StartCoroutine(WaitForDeathToFinish());
            }
            DieCooldown -= Time.deltaTime;
        }
    }

    void CheckForDeath()
    {
        //note that cooldown is reset every time damage is received again
        if(unit.HP <= 0){
            DieCooldown = DieDelaySeconds;
            DieReceived = true;
            unitAnimator.SetTrigger("Flail"); //start flailing as prepare to die
        }
    }

    private IEnumerator WaitForDeathToFinish(){        
        yield return new WaitForSeconds(DeathAnimDuration);
        FinishDeath();
    }

    protected virtual void FinishDeath(){
        if(DeathParticles){
            DeathParticles.transform.parent = this.transform.parent;
            DeathParticles.gameObject.SetActive(true);
        }
        GameObject.Destroy(this.gameObject);
    }
    
}
