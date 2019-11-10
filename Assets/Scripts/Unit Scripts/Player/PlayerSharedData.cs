using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSharedData", menuName = "Paper Mario Clone/Unit Related/PlayerSharedData", order = 0)]
public class PlayerSharedData : ScriptableObject {
    public int FP = 10;
    public int MaxFP = 10;

    public float SP = 1;
    public float MaxSP = 1;

    //not used in fights but important outside of fights
    public int BP = 3;
    public int MaxBP = 3;
}