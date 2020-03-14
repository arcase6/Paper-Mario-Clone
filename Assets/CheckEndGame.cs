using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEndGame : MonoBehaviour
{

    public UnitRuntimeSet EnemySet;
    public Unit MarioReference;

    public SceneTransitioner WinTransitioner;
    public SceneTransitioner LoseTransitioner;

    void Start(){
        MarioReference.HPChanged += MarioHPChange;
    }

   public void OnEnemyUnitRemoved(){
       if(EnemySet.Units.Count == 0){
            WinGame();           
       }
   }

   public void MarioHPChange(){
       if(MarioReference.Stats.HP <= 0){
           LoseGame();
       }
   }

   public void LoseGame(){
       LoseTransitioner.Transition();
   }

    public void WinGame(){
        WinTransitioner.Transition();
    }


}
