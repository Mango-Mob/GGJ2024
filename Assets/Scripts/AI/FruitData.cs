using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Data/FruitData", order = 1)]
public class FruitData : ScriptableObject
{
    [Header("Aggro")]
    public float aggroRadius;
    public float aggroDelta;
                      
    [Header("Speed")]
    public float IdleSpeed;
    public float RunSpeed;
    public float movementSpeedDelta;

    [Header("AI")]
    public float idleTime;
    public float idleTimeDelta;
    public float idleMoveTime;
    public float idleMoveTimeDelta;
    public float runTime;
    public float runTimeDelta;
    
    [Range(0, 100)]
    public int chanceToMoveAgain;
}
