using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Unit Unit;
    public StringMediator DisplayText;
    
    [Tooltip("Used for overlays showing things like player health")]
    public Image FillBar; 
    [Tooltip("Only reference necessary for Enemy displays")]
    public Transform EnemyFillTransform; // only used for overhead bars
    
    public float PointsPerSec = 8;

    public BuffType BarType = BuffType.HP;
    private float SmoothAmmount;
    private bool IsOverhead;


    public void OnEnable() {
        IsOverhead = Unit as Player == null;
        switch(BarType){
            case BuffType.HP:
            this.SmoothAmmount = Unit.HP;
            this.Unit.HPChanged += UpdateFillAmount;
            break;
            case BuffType.FP:
                Player player = Unit as Player;
                if(!player){
                    Debug.Log("This unit is not a player and does not have an FP associated with it");
                    return;
                }
                this.SmoothAmmount = player.FP;
                player.FPChanged += UpdateFillAmount;
            break;
            default:
            Debug.Log("This type of bar is not supported : " + BarType.ToString());
            break;
        }    
        DisplayFillAmmount();
    }
    
    private void OnDisable() {
        switch(BarType){
            case BuffType.HP:
            this.Unit.HPChanged -= UpdateFillAmount;
            break;
            case BuffType.FP:
            Player player = Unit as Player;
                if(!player){
                    Debug.Log("This unit is not a player and does not have an FP associated with it");
                    return;
                }
                player.FPChanged -= UpdateFillAmount;
            break;
            default:
            break;
        } 
    }

    [ContextMenu("Update Display")]
    public void UpdateFillAmount(){
        int targetAmmount = BarType == BuffType.HP ? Unit.HP : (Unit as Player).FP;

        StartCoroutine(UpdateFillAmountSmooth(targetAmmount));
    }

    

    private IEnumerator UpdateFillAmountSmooth(int targetAmount){
        bool increasing = targetAmount > SmoothAmmount;
        
        while(!Mathf.Approximately(SmoothAmmount,targetAmount)){
            float change = PointsPerSec * Time.deltaTime;
            if(increasing){
                SmoothAmmount = Mathf.Min(SmoothAmmount + change, targetAmount);
            }
            else{
                SmoothAmmount = Mathf.Max(SmoothAmmount - change, targetAmount);
            }
            DisplayFillAmmount();
            yield return null;
        }
        SmoothAmmount = targetAmount;
    }


    public void DisplayFillAmmount(){
        int max = BarType == BuffType.HP ? Unit.MaxHP : (Unit as Player).MaxFP;
        int roundedAmmount = Mathf.RoundToInt(SmoothAmmount);
        
        if(IsOverhead){
            EnemyFillTransform.localScale = new Vector3(SmoothAmmount / max,EnemyFillTransform.localScale.y, EnemyFillTransform.localScale.z);
        }
        else{
            this.FillBar.fillAmount = SmoothAmmount / max;
            this.DisplayText.Value = string.Format("{0} / {1}", roundedAmmount, max);
        }
    }
}
