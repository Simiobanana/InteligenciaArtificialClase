using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Como NaiveFSM ya hereda de monoBehaviour, entonces PatrolAgentFSM también ya tiene las propiedades de MonoBehaviour.
public class PatrolAgentFSM : NaiveFSM
{
    // Como NaiveFSM ya hereda de monoBehaviour, entonces PatrolAgentFSM también ya tiene las propiedades de MonoBehaviour.
    private NaivePatrolState _PatrolState;
    private NaiveAlertState _AlertState;
    private NaiveAttackState _AttackState;

    // aquí faltaría el estado de la clase NaiveAttackState, así como su Getter, su inicialización, etc.
    // eso les toca a ustedes.

    public NaivePatrolState PatrolStateRef
    {
        get { return _PatrolState; }
    }

    public NaiveAlertState AlertStateRef
    {
        get { return _AlertState; }
    }

    public NaiveAttackState AttackStateRef
    { 
        get { return _AttackState; } 
    }

    // Ponerlo en el editor, o setearlo en el start.
    // Los estados pueden acceder a él a través de la máquina de estados.
    public NavMeshAgent _NavMeshAgent;

    // Usaremos esta wall layer mask para hacer que el patrullero no pueda ver al jugador a través de las paredes.
    // La inicializamos en el Start de nuestra FSM.
    // Para que funcione, tienen que ir a "Tags & Layers", agregar una nueva layer que se llame "Wall", y
    // asignarle dicha Layer a los gameobjects en su escena que sean paredes. Les recomiendo hacer un gameobject
    // vacío que sea padre de las walls de su escenario, y hacer que ese padre tenga el Layer de wall, y aplicarle
    // el cambio de layer a todos los hijos (vean la Jerarquía de mi escena para que vean que todas mis walls
    // tienen el tag de Wall).
    LayerMask WallLayerMask;

    //Dado que yo no voy a cambiar el color de mi patrulla voy a comentar todo lo relacionado al mesh renderer
    //MeshRenderer _Renderer;

    //Lo que si voy a cambiar sera el color de la luz, entonces la declaro
    public Light _light;

    //Asi mismo tambien al ocupar el animator tambien lo declaro
    public Animator _Animator;
    // acá iría la referencia al animator., para que los estados accedan a él
    // a través de la FSM.

    // public float MovementSpeed;
    // La posición de nuestro agente la podemos acceder a través del transform.

    // está dado por la posición inicial del Patrullero en la escena. Es decir, su transform
    // La hice private porque nadie la debe de modificar por ahora, y la pueden acceder los estados 
    // a través del Getter que puse justo debajo.
    private Vector3 _initialPatrolPosition = Vector3.zero;
    public Vector3 InitialPatrolPosition
    {
        get { return _initialPatrolPosition; }
    }

    // Otra ventaja de tenerlas en la FSM es que estas variables las podemos acceder y modificar desde el editor.
    // NOTA: Estas son el valor base para el estado de Patrullaje, pero se multiplican por un factor
    // en los otros estados. ¿Ok?
    public float VisionAngle = 45.0f;
    public float VisionDistance = 20.0f;


    // La variable detected plauyer sí queremos que sea accesible desde otros pedazos de código, pero no desde el editor.
    private bool _DetectedPlayer = false;

    // este getter es por si alguno de los estados ya mandó actualizar CheckFieldOfVision pero quiere volver
    // a checar el resultado sin tener que volver a ejecutar toda la función.
    public bool DetectedPlayer
    {
        get { return _DetectedPlayer; }
    }

    // El GameObject a través del cual obtendremos el transform del jugador al que estaremos tratando de atrapar.
    private GameObject _PlayerGameObject;

    private Vector3 _LastKnownPlayerPosition = Vector3.zero;
    // Uso este getter the LastKnownPlayerPosition para brindar la posición a los estados. (solo lectura, No escritura)
    public Vector3 LastKnownPlayerPosition
    {
        get { return _LastKnownPlayerPosition; }
    }

    // Pero... ¿y qué de las variables que son específicas de ciertos estados?
    // Depende de cómo lo quieran hacer ustedes.
    // Alternativa 1) sí poner las variables esas aquí en la FSM :C 
    // Lo malo es que nos podrían estar haciendo bulto en la FSM ya que no se usan en los demás estados.
    // Alternativa 2) Usar un archivo de lectura que contenga todos los valores para esas variables.
    // Por ejemplo, usar un archivo de tipo JSON
    // https://docs.unity3d.com/ScriptReference/JsonUtility.FromJson.html
    // Alternativa 2.5) Leer los valores desde un Excel o base de datos.
    // Cada una tiene sus ventajas y desventajas.

    // Grupo de variables para el estado de Patrullaje (NaivePatrolState)
    // Estas variables solo se van a setear en el estado al iniciar la máquina de estados (por el momento).
    [Range(15.0f, 180.0f)]
    public float RotationAngle = 45.0f;
    [Range(0.25f, 20.0f)]
    public float TimeBeforeRotating = 3.0f;
    [Range(0.1f, 10.0f)]
    public float TimeDetectingPlayerBeforeEnteringAlert = 1.0f;


    // Grupo de variables para el estado de Alerta (NaiveAlertState)
    [Range(1.0f, 3.0f)]
    public float AlertVisionAngleMultiplier = 1.2f;
    [Range(1.0f, 3.0f)]
    public float AlertVisionDistanceMultiplier = 1.2f;
    [Range(0.5f, 10.0f)]
    public float TimeSinceLastSeenTreshold = 3.0f;
    [Range(5.0f, 50.0f)]
    public float SpeedWhileCheckingLastKnownLocation = 10.0f;  // ahorita no la voy a usar.


    // Grupo de variables para el estado de Ataque (NaiveAttackState).
    // Les toca implementarlo a ustedes.
    [Range(1.0f, 3.0f)]
    public float AttackVisionAngleMultiplier = 1.5f;
    [Range(1.0f, 5.0f)]
    public float AttackVisionDistanceMultiplier = 1.5f;

    // Esta es una extensión del Start de la clase FSM base, por lo que hace todo lo que ella haría, más 
    // lo que se necesite específicamente en esta clase.
    public override void Start()
    {
        //_Renderer = GetComponent<MeshRenderer>();
        _PlayerGameObject = GameObject.Find("Player");
        //Obtenemos el animator que en mi caso esta en el parent
        _Animator = GetComponentInParent<Animator>();
        

        // Le decimos explícitamente que mande a llamar el start de su clase padre.
        base.Start();

        // Aquí inicializamos esto, que usaremos con un raycast más abajo.
        WallLayerMask = LayerMask.GetMask("Wall");

        // Inicializamos esto como la posición en la que el agente patrullero inicia en la escena.
        _initialPatrolPosition = transform.position;

        _PatrolState = (NaivePatrolState)_CurrentState;
        _AlertState = new NaiveAlertState(this);
        _AttackState = new NaiveAttackState(this);


        // Mandamos a llamar las funciones Init de los estados, las que se encargan de setear las variables
        // de cada estado. Por ahora lo hacemos aquí, pero recuerden que lo ideal sería a través de algún 
        // archivo de configuración, que lea estos valores y los ponga en cada estado como corresponda.
        _PatrolState.Init(VisionDistance, VisionAngle, RotationAngle, TimeBeforeRotating,
            TimeDetectingPlayerBeforeEnteringAlert);

        // Añadí los multiplicadores de ángulo y distancia a la inicialización de los estados.
        _AlertState.Init(VisionDistance * AlertVisionDistanceMultiplier, VisionAngle * AlertVisionAngleMultiplier,
            TimeSinceLastSeenTreshold, SpeedWhileCheckingLastKnownLocation);

        // Añadí los multiplicadores de ángulo y distancia a la inicialización de los estados.
        _AttackState.Init(VisionAngle * AttackVisionAngleMultiplier, VisionDistance * AttackVisionDistanceMultiplier);
    }

    protected override NaiveFSMState GetInitialState()
    {
        // Regresa null para que cause error porque la función de esta clase padre nunca debe de usarse, siempre 
        // se le debe de hacer un override.
        return new NaivePatrolState(this);
    }


    // OJO: esta función la uso con parámetros de Distancia y Ángulo de visión en vez de usar 
    // las variables de la FSM, porque cada estado tiene sus propios valores de ellas, ¿ok?
    // Entonces hay que pasarle las variables de cada estado al mandarla a llamar.
    public bool CheckFieldOfVision(float in_VisionDist, float in_VisionAngle)
    {
        // Verificar si el estado actual es de patrullaje y actualizar el rango de la luz
        if (_PatrolState != null && _CurrentState == _PatrolState)
        {
            UpdateLightRange(_PatrolState.VisionDistance);
        }
        else if (_AlertState != null && _CurrentState == _AlertState)
        {
            UpdateLightRange(_AlertState.VisionDistance);
        }
        else if (_AttackState != null && _CurrentState == _AttackState)
        {
            UpdateLightRange(_AttackState.AttackVisionDistanceMultiplier);
        }
        //CambiarAnimaciones();
        // Esta función regresa dos cosas, un booleano que dice: sí ví al jugador o no;
        // además, si sí lo vimos, actualizará la variable _LastKnownPlayerPosition.
        _DetectedPlayer = false; // por defecto la ponemos como falso.

        //Establecemos que el color de la luz sea verde normalmente cuando esta en falso el detected player
        //_light.color = Color.green;
        //_Renderer.material.color = new Color(1, 0, 0, 1);
        Vector3 AgentToTargetVector = (_PlayerGameObject.transform.position - transform.position);

        // Profiling o benchmarking
        float AgentToTargetDist = AgentToTargetVector.magnitude;
        if (AgentToTargetDist > in_VisionDist)
        {
            // Nos salimos porque no está en el rango de visión.
            return false;
        }

        if (Vector3.Angle(AgentToTargetVector, transform.forward) > in_VisionAngle * 0.5)
        {
            // Nos salimos porque no se está dentro del ángulo que define al cono.
            return false;
        }

        // si el raycast choca primero contra una wall que contra el Target, entonces no  
        // puede ver al Target tal cual, porque hay una pared de por medio.
        if (Physics.Raycast(transform.position, AgentToTargetVector.normalized,
            AgentToTargetVector.magnitude, WallLayerMask))
        {
            return false;
        }

        // Si sí vimos al jugador, actualizar la variable de la última posición conocida.
        _LastKnownPlayerPosition = _PlayerGameObject.transform.position;
        _DetectedPlayer = true; // solo cambia a verdadera aquí. 
        //Cambiamos la luz a amarillo cuando detected player es true
        //_light.color = Color.magenta;
        //_Renderer.material.color = new Color(1, 0, 1, 1);
        return true;
    }

    private void UpdateLightRange(float visionDistance)
    {
        // Ajustar el rango de la luz según el alcance del cono de visión
        _light.range = visionDistance;
    }


    /*private void CambiarAnimaciones()
    {
        if(_DetectedPlayer == true)
        {
            _Animator.SetBool("Patrullando", true);
        }
        else if (_DetectedPlayer==false) 
        {
            _Animator.SetBool("Patrullando", false);
        }

    }*/

    //Dibujamos los conos de vision que se ven afectados por los estados
    private void OnDrawGizmos()
    {
        if (_PatrolState != null)
        {
            Gizmos.color = Color.green;
            DrawVisionCone(_PatrolState.VisionDistance, _PatrolState.VisionAngle);
        }

        if (_AlertState != null && _CurrentState == _AlertState)
        {
            Gizmos.color = Color.yellow;
            DrawVisionCone(_AlertState.VisionDistance, _AlertState.VisionAngle);
        }

        if (_AttackState != null && _CurrentState == _AttackState)
        {
            Gizmos.color = Color.red;
            DrawVisionCone(_AttackState.AttackVisionDistanceMultiplier, _AttackState.AttackVisionAngleMultiplier);
        }
    }

    //Asi mismo como se hizo en el primer y segundo parcial, dividimos el angulo de vision en 2 para facilitar la deteccion
    private void DrawVisionCone(float distance, float angle)
    {
        float halfAngle = angle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);

        Vector3 startPoint = transform.position;

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(startPoint, leftRayDirection * distance);
        Gizmos.DrawRay(startPoint, rightRayDirection * distance);
        Gizmos.DrawRay(startPoint + leftRayDirection * distance, rightRayDirection * distance - leftRayDirection * distance);
    }

}

