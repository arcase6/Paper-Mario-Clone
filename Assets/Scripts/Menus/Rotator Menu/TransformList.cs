using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransformList", menuName = "Paper Mario Clone/TransformList", order = 0)]
public class TransformList : ScriptableObject {
    public List<Transform> transforms;
}