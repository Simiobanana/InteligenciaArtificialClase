using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class NaiveAttackState : NaiveFSMState
{
    private PatrolAgentFSM PatrolFSMRef = null;
    //Variable para tener control del rango de distancia con el jugador
    public GameObject agent;

    //Variables que manejan la distancia y angulo de vision
    public float AttackVisionAngleMultiplier;
    public float AttackVisionDistanceMultiplier;
    //Variables para regresar a otro estado
    public float TimeToChangeState;
    public float TimeBeforeChangeState = 0f;


    public NaiveAttackState(NaiveFSM FSM)
    {
        Name = "Attack";
        _FSM = FSM;
        PatrolFSMRef = ((PatrolAgentFSM)_FSM);
    }

    public void Init(float in_AttackVisionAngleMultiplier, float in_AttackVisionDistanceMultiplier)
    {
        AttackVisionAngleMultiplier = in_AttackVisionAngleMultiplier;
        AttackVisionDistanceMultiplier = in_AttackVisionDistanceMultiplier;
    }

    public override void Enter()
    {
        base.Enter();
        TimeToChangeState = 5f;
        PatrolFSMRef._Animator.SetBool("Ataque", true);
        PatrolFSMRef._Animator.SetBool("Alerta", true);
        agent = GameObject.Find("Player");
        PatrolFSMRef._light.color = Color.red;
    }

    public override void Update()
    {
        base.Update();
        Vector3 directionToPlayer = agent.transform.position - _FSM.transform.position;
        PatrolFSMRef._light.color = Color.red;
        PatrolFSMRef._NavMeshAgent.SetDestination(agent.transform.position);

        TimeToChangeState -= Time.deltaTime;
        if (TimeToChangeState <= TimeBeforeChangeState)
        {
            NaivePatrolState AlertStateInstance = PatrolFSMRef.PatrolStateRef;
            _FSM.ChangeState(AlertStateInstance);
            return;
        }
       

        if (directionToPlayer.magnitude < 1.0f)
        {
            NaivePatrolState PatrolStateInstance = PatrolFSMRef.PatrolStateRef;
            _FSM.ChangeState(PatrolStateInstance);
            DestroyPlayer();
            return;
        }

    }

    public override void Exit()
    {
        PatrolFSMRef._Animator.SetBool("Ataque", false);
        PatrolFSMRef._Animator.SetBool("Alerta", false);
        base.Exit();

    }

    private void DestroyPlayer()
    {
        
        agent.SetActive(false);
        Debug.Log("El jugador ha sido destruido.");
    }
}
