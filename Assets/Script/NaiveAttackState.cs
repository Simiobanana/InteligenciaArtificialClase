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

    //Inicializamos la distancia y angulo de vision de este estado en la maquina de estados para que cambie
    public void Init(float in_AttackVisionAngleMultiplier, float in_AttackVisionDistanceMultiplier)
    {
        AttackVisionAngleMultiplier = in_AttackVisionAngleMultiplier;
        AttackVisionDistanceMultiplier = in_AttackVisionDistanceMultiplier;
    }

    public override void Enter()
    {
        base.Enter();
        //Inicializamos la variable de cuanto tiempo tardara en perder el estado de ataque
        TimeToChangeState = 5f;
        //Inicializamos la animacion al entrar a este estado
        PatrolFSMRef._Animator.SetBool("Ataque", true);
        //PatrolFSMRef._Animator.SetBool("Alerta", true);
        agent = GameObject.Find("Player");
        PatrolFSMRef._light.color = Color.red;
    }

    public override void Update()
    {
        base.Update();
        //Establecemos para donde se va a dirigir mi personaje en el estado de ataque
        Vector3 directionToPlayer = agent.transform.position - _FSM.transform.position;
        //Establecemos la luz roja
        PatrolFSMRef._light.color = Color.red;
        //Aplicamos el navmesh cuando se vaya en direccion a mi jugador
        PatrolFSMRef._NavMeshAgent.SetDestination(agent.transform.position);

        //Empezamos a hacer que corra el timmpo para regresar de estado
        TimeToChangeState -= Time.deltaTime;

        //Si se acaba el tiempo regresa al estado de alerta
        if (TimeToChangeState <= TimeBeforeChangeState)
        {
            NaivePatrolState AlertStateInstance = PatrolFSMRef.PatrolStateRef;
            _FSM.ChangeState(AlertStateInstance);
            return;
        }
       
        //Si la distancia de la direccion a la que vamos con el jugador es menor a una unidad de unity destruye el personaje
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
        //Salimos de las animacioes
        PatrolFSMRef._Animator.SetBool("Ataque", false);
        PatrolFSMRef._Animator.SetBool("Alerta", true);
        base.Exit();

    }

    private void DestroyPlayer()
    {
        //Apagas a mi personaje
        agent.SetActive(false);
        Debug.Log("El jugador ha sido destruido.");
    }
}
