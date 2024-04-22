using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaivePatrolState : NaiveFSMState
{
    private PatrolAgentFSM PatrolFSMRef = null;

    // Variables No-exclusivas del estado
    // Cono de visión
    // Las dejé comentadas como ejemplo de que son las no-exclusivas del estado, y que me las llevé a la máquina de estados.
    //public float VisionAngle;
    //public float VisionDistance;
    //public bool DetectedPlayer;

    public float VisionAngle;
    public float VisionDistance;
    public GameObject agentTransform;

    // Variables Exclusivas de este estado.
    // 
    private float RotationAngle;
    private float TimeBeforeRotating = 2f;
    private float AccumulatedTimeBeforeRotating;
    private float TimeDetectingPlayerBeforeEnteringAlert;
    private float AccumulatedTimeDetectingPlayerBeforeEnteringAlert;

    public void Init(float in_VisionDistance, float in_VisionAngle, float in_RotationAngle,
        float in_TimeBeforeRotating, float in_TimeDetectingPlayerBeforeEnteringAlert)
    {
        RotationAngle = in_RotationAngle;
        TimeBeforeRotating = in_TimeBeforeRotating;
        TimeDetectingPlayerBeforeEnteringAlert = in_TimeDetectingPlayerBeforeEnteringAlert;
        // Estas dos de aquí las sacamos de la FSM
        VisionAngle = in_VisionAngle;
        VisionDistance = in_VisionDistance;
    }

    // Ojo: no hagan esto, porque conlleva a situaciones molestas y propensas a errores humanos.
    // Referencia al estado de alerta (OJO, ESTO CAUSARÁ ALTA DEPENDENCIA ENTRE LAS CLASES)
    // NaiveAlertState _AlertState;

    public NaivePatrolState(NaiveFSM FSM)
    {
        Name = "Patrol";
        _FSM = FSM;
        PatrolFSMRef = ((PatrolAgentFSM)_FSM);
    }


    public override void Enter()
    {
        // 
        base.Enter();
        PatrolFSMRef._Animator.SetBool("Patrullando", true);
        agentTransform = GameObject.Find("Patroller");
        Debug.Log("Entré al estado de Patrullaje.");
        // Acá ya puedo hacer lo que esta clase hija específicamente tiene que hacer
        AccumulatedTimeBeforeRotating = 0.0f;
        AccumulatedTimeDetectingPlayerBeforeEnteringAlert = 0.0f;
    }

    // Update is called once per frame
    public override void Update()
    {
        // Debug.Log("Update del estado patrullaje.");

        // Detectar al infiltrador
        // Lo detectamos?
        if (CheckFOV())
        {
            // Si la función del cono de visión nos regresa verdadero
            // Acumulamos tiempo
            // Lo acumulamos en la variable que declaramos para llevar el tiempo acumulado "AccumulatedTimeDetectingPlayerBeforeEnteringAlert"
            AccumulatedTimeDetectingPlayerBeforeEnteringAlert += Time.deltaTime;
            // si el tiempo acumulado es mayor a la cantidad de tiempo que establecimos, cambiamos al estado de alerta
            if (TimeDetectingPlayerBeforeEnteringAlert <= AccumulatedTimeDetectingPlayerBeforeEnteringAlert)
            {
                // Si sí, cambiamos al estado de alerta.
                // (PatrolAgentFSM)_FSM -> casteamos la máquina de estados base, al tipo específico de la FSM que es nuestra dueña.
                // esto nos permite acceder a las variables que tiene esa clase específica, en este caso, al estado de Alerta al que
                // queremos pasar, que lo obtenemos a través de "AlertStateRef".
                NaiveAlertState AlertStateInstance = PatrolFSMRef.AlertStateRef;
                PatrolFSMRef._Animator.SetBool("Patrullando", false);
                _FSM.ChangeState(AlertStateInstance);
                return; // Damos return siempre después del change state, para evitar que cualquier otra cosa 
                // del update se fuera a ejecutar.
            }
        }

        // Dónde pondríamos la parte de rotar al patrullero cada cierto tiempo?
        AccumulatedTimeBeforeRotating += Time.deltaTime;
        if (AccumulatedTimeBeforeRotating >= 4f)
        {
            RotateAgent();
            AccumulatedTimeBeforeRotating = 0.0f; // Reiniciamos el contador de tiempo
            Debug.Log("Rotando");
        }


    }

    private void RotateAgent()
    {
        // Rotar el agente según el ángulo de rotación definido
        agentTransform.transform.Rotate(Vector3.up, RotationAngle);
    }

    // Aquí estamos omitiendo a propósito la función Exit()
    // solo para demostrar que podemos hacerlo.

    private bool CheckFOV()
    {
        // únicamente mandamos a llamar la función de la FSM pero con nuestros parámetros específicos de este estado.
        return PatrolFSMRef.CheckFieldOfVision(VisionDistance, VisionAngle);
    }
}


