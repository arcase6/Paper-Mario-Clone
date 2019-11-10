using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAction{
    public ActionData Action;
    [Range(0,100)]
    public int Weight = 100;
}



[CreateAssetMenu(fileName = "EnemyActionList", menuName = "Paper Mario Clone/EnemyActionList", order = 0)]
public class EnemyActionList : ScriptableObject {
    public List<EnemyAction> Actions = new List<EnemyAction>();
}