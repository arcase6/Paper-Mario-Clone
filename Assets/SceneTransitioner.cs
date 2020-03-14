using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public Animator TransitionAnimator;

    public string TransitionOutName = "TransitionOut";
    private int HashTransitionOut = 0;
    public int SceneIndex = 0;
    
    public float StartDelay = 2f;
    public float TransitionDelay = 5f;
    
    private void Start() {
        HashTransitionOut = Animator.StringToHash(TransitionOutName);
    }


    public void Transition(){
        StartCoroutine(TransitionAfterDelay());

    } 
    public IEnumerator TransitionAfterDelay(){
        if(StartDelay != 0)
            yield return new WaitForSeconds(StartDelay);

        if(TransitionDelay != 0){
            TransitionAnimator.SetTrigger(HashTransitionOut);
            yield return new WaitForSeconds(TransitionDelay);
        }
        SceneManager.LoadScene(SceneIndex,LoadSceneMode.Single);
    }

    
}
