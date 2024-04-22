using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class NaiveAttackState : NaiveFSMState
{
    private PatrolAgentFSM PatrolFSMRef = null;

    public float AttackVisionAngleMultiplier;
    public float AttackVisionDistanceMultiplier;


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

        // Iniciar la animación de ataque, efectos de sonido, etc.
    }

    public override void Update()
    {
        base.Update();

        // Perseguir al jugador
        Vector3 directionToPlayer = PatrolFSMRef.LastKnownPlayerPosition - _FSM.transform.position;
        PatrolFSMRef._NavMeshAgent.SetDestination(PatrolFSMRef.LastKnownPlayerPosition);

        // Si el agente alcanza al jugador, destruirlo y cambiar al estado de patrullaje
        if (directionToPlayer.magnitude < 1.0f)
        {
            DestroyPlayer();
            NaivePatrolState PatrolStateInstance = PatrolFSMRef.PatrolStateRef;
            _FSM.ChangeState(PatrolStateInstance);
            return;
        }
        {
            NaivePatrolState PatrolStateInstance = PatrolFSMRef.PatrolStateRef;
            _FSM.ChangeState(PatrolStateInstance);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Detener la animación de ataque, efectos de sonido, etc.
    }

    private void DestroyPlayer()
    {
        // Aquí puedes desactivar, destruir, teletransportar al jugador, etc.
        Debug.Log("El jugador ha sido destruido.");
    }
}
