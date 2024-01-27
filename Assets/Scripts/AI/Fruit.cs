using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public FruitData data;
    public enum AIStates : int { Idle, IdleMove, Run, Zombie, NumStates };
    public AIStates state;
    public MeshRenderer mainRenderer;

    public AnimationCurve materialBlend;
    public Material healthyMaterial;
    public Material zombieMaterial;

    private float[] stateMaxTime = new float[(int)AIStates.NumStates];
    private float[] speed = new float[(int)AIStates.NumStates];
    private float healthTimer;
    private float stateTimer = 0.0f;
    public float stateLockTimer = 0.0f;
    public bool stateLock = false;
    private float aggroRadius = 0.0f;

    private int chanceIdleMove;
    private UnityEngine.AI.NavMeshAgent Agent;
    private GameObject player;

    public float juiceAmount = 1.0f;

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

            healthTimer = data.healthTime;
            chanceIdleMove = data.chanceToMoveAgain;
        }
        mainRenderer.material = healthyMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        Agent.enabled = stateLockTimer <= 0 && !stateLock;
        if (stateLockTimer > 0)
        {
            stateLockTimer -= Time.deltaTime;
            return;
        }
        if (stateLock)
            return;


        if (stateMaxTime[(int)state] > 0)
            stateTimer -= Time.deltaTime;

        healthTimer -= Time.deltaTime;

        if (state != AIStates.Zombie)
        {
            mainRenderer.material.Lerp(healthyMaterial, zombieMaterial, materialBlend.Evaluate(1.0f - healthTimer / data.healthTime));
            if (healthTimer < 0)
                EnterState(AIStates.Zombie);
        }

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
                Agent.SetDestination(player.transform.position);
                if (healthTimer < 0)
                    Destroy(gameObject);
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
                Agent.SetDestination(transform.position);
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
                Agent.SetDestination(player.transform.position);
                mainRenderer.material = zombieMaterial;
                healthTimer = data.deathTime;
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
