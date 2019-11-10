using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Paper Mario Clone/Unit Related/UnitData", order = 0)]
public class UnitData : ScriptableObject {
    public string Name = "Unnamed Unit";
    public UnitType UnitType = UnitType.Ground;
    public int HP = 10;
    public int MaxHP = 10;

    public int BaseAttack = 0;
    public int BaseDefense = 0;

}