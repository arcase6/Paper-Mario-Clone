using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Orientation{
    left = 0,
    right = 1
}

public class OrientCharacter : MonoBehaviour
{
    public Unit unit;
    void Reset(){
        unit = GetComponent<Unit>();
    }

    
    [SerializeField]
    private Orientation orientation;
    public Orientation Orientation{
        get => orientation;
        set{
            if(orientation != value){
                orientation = value;
                unit.Animator.SetInteger(orientationHash,(int)orientation);
            }
        }
    }


    private int orientationHash = 0;
    private void Start() {
        orientationHash = Animator.StringToHash("TurnOrientation");
        unit.Animator.SetInteger(orientationHash,(int)orientation);
    }

}
