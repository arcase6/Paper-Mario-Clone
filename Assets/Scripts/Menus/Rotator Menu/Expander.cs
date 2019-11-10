using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ExpansionMode
{
    Width,
    Height
}

public class Expander : MonoBehaviour
{
    [Range(0,1)]
    public float ExpansionPercent; 


    public float MinExpansion;
    public float MaxExpansion;

    public SpriteRenderer Sprite;

    public ExpansionMode Mode; 

    // Update is called once per frame
    void Update()
    {
        float ExpansionAmmount = MinExpansion + (ExpansionPercent * (MaxExpansion - MinExpansion));
        if(this.Mode == ExpansionMode.Width){
            Sprite.size = new Vector2(ExpansionAmmount,Sprite.size.y);
        }
        else{
            Sprite.size = new Vector2(Sprite.size.x,ExpansionAmmount);
        }
    }
}
