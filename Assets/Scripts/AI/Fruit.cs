using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public FruitData data;
    public enum AIStates : int { Idle, IdleMove, Run, Zombie, NumStates };
    public AIStates state;

    private float[] stateMaxTime = new float[(int)AIStates.NumStates];
    private float[] speed = new float[(int)AIStates.NumStates];
    private float stateTimer = 0.0f;
    private float stateLockTimer = 0.0f;
    private float aggroRadius = 0.0f;


    private int chanceIdleMove;
    private UnityEngine.AI.NavMeshAgent Agent;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (data)
        {
            aggroRadius = data.aggroRadius + Random.Range(-data.aggroDelta, data.aggroDelta);
            float movement_delta = Random.Range(-data.movementSpeedDelta, data.movementSpeedDelta);
            speed[(int)AIStates.IdleMove] = data.IdleSpeed + movement_delta;
            speed[(int)AIStates.Run] = data.RunSpeed + movement_delta;

            stateMaxTime[(int)AIStates.Idle] = Mathf.Max(data.idleTime + Random.Range(-data.idleTimeDelta, data.idleTimeDelta), 0.0f);
            stateMaxTime[(int)AIStates.IdleMove] = Mathf.Max(data.idleMoveTime + Random.Range(-data.idleMoveTimeDelta, data.idleMoveTimeDelta), 0.0f);
            stateMaxTime[(int)AIStates.Run] = Mathf.Max(data.runTime + Random.Range(-data.runTimeDelta, data.runTimeDelta), 0.0f);

            chanceIdleMove = data.chanceToMoveAgain;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(stateLockTimer > 0)
        {
            stateLockTimer -= Time.deltaTime;
            return;
        }

        if (stateMaxTime[(int)state] > 0)
            stateTimer -= Time.deltaTime;

        bool player_within = Vector3.Distance(player.transform.position, transform.position) < aggroRadius;

        switch (state)
        {
            case AIStates.Idle:
                if (stateTimer < 0)
                    EnterState(AIStates.IdleMove);
                if (player_within)
                    EnterState(AIStates.Run);
                break;
            case AIStates.IdleMove:
                if (stateTimer < 0)
                    EnterState(Extentions.Roll(chanceIdleMove) ? AIStates.Idle : AIStates.IdleMove);
                if (player_within)
                    EnterState(AIStates.Run);
                break;
            case AIStates.Run:
                if (!player_within)
                    EnterState(AIStates.Idle);
                else
                {
                    RunAway();
                    if (stateTimer < 0)
                        EnterState(AIStates.Idle, stateMaxTime[(int)AIStates.Run]);
                }
                break;
            case AIStates.Zombie:
                break;
            default:
                break;
        }
    }

    void EnterState(AIStates new_state, float state_lock_duration = 0)
    {
        state = new_state;
        Agent.speed = speed[(int)state];
        stateTimer = stateMaxTime[(int)state];
        stateLockTimer = state_lock_duration;
        switch (state)
        {
            case AIStates.Idle:
                break;
            case AIStates.IdleMove:
                var dist = Agent.speed * stateTimer; //speed * time = dist
                var forward_vec2 = Extentions.FromAngle(Random.Range(0, 360) * Mathf.Deg2Rad, dist);
                Agent.SetDestination(transform.position + new Vector3(forward_vec2.x, 0.0f, forward_vec2.y));
                break;
            case AIStates.Run:
                RunAway();
                break;
            case AIStates.Zombie:
                break;
            case AIStates.NumStates:
                break;
            default:
                break;
        }
    }

    private void RunAway()
    {
        Vector3 direct = (transform.position - player.transform.position).normalized;

        Agent.SetDestination(transform.position + direct * Agent.speed);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && !data)
            return;

        Gizmos.color = Color.green;
        Extentions.GizmosDrawCircle(transform.position, Application.isPlaying ? aggroRadius : data.aggroRadius);

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Extentions.GizmosDrawCircle(transform.position, data.aggroRadius + data.aggroDelta);
            Extentions.GizmosDrawCircle(transform.position, data.aggroRadius - data.aggroDelta);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            Gizmos.color = Color.cyan;
            Extentions.GizmosDrawCircle(Agent.destination, 2);
        }
    }
}
