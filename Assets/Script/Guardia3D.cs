using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardia3D : MonoBehaviour
{
    //Todo para que el arrive funcione
    public enum SteeringBehavior
    {
        None,  // 0
        seek,    //1
        Pursuit, //2
    };
    public float maxSteeringForce = 1.0f;
    public float maxSpeed = 1.0f;
    public Rigidbody rb;
    public bool useArrive = true;
    public float slowAreaRadius = 5.0f;
    // Variable donde guardamos la referencia al GameObject que es nuestro objetivo.
    private GameObject TargetGameObject;
    // Referencia al Rigidbody del TargetGameObject.
    private Rigidbody rbTargetGameObject;
    //-----------------------------------------------------------
    private Vector3 PosicionInicialAgente;
    public GameObject agentPrefab;
    //--------------------------------------------------------------
    //Establecemos la posicion inicial de nuestro guardia como un vector 3
    private Vector3 posicionInicial;
    //Establecemos la ultima posicion detectada de nuestro agente como un vector 3
    private Vector3 ultimaPosicionDetectada;
    // Todo para que cambie de color en los diferentes estados
    //Accedemos al render de nuestro objeto
    private Renderer RendererObject;
    //Dependiendo de en que estado vaya a estar le modificamos su color
    public Color ColorNormal = Color.green;
    public Color ColorAlerta = Color.yellow;
    public Color ColorAtaque = Color.red;
    //----------------------------------------------------------------------

    //Generamos un cronometro para que pasado cierto tiempo cuando nuestro guardia este en alerta si no ha visto de nuevo a nuestro agente, vuelva a 
    //Su estado normal
    [SerializeField] private float CuentaRegresiva;
    //Generamos un cronometro de deteccion (Si el jugador permanece cierta cantidad de tiempo en el rango se cambiara de estado)
    [SerializeField] private float CronometroEstado = 0f;
    //Generamos un cronometro especificamente para el pursuit
    [SerializeField] private float CronometroPursuit = 0f;

    //Hacemos una lista con todos los posibles estados
    public enum Estados
    {
        None,
        Normal,
        Alerta,
        Ataque
    }
    //Generamos una variable que establecera el estado actual
    public Estados EstadoActual = Estados.None;


    //ANTES DEL EXAMEN
    //------------------------------------------------------------------------------------------------------------------------------------------
    // Ubicamos la posicion de nuestro objeto a detectar
    public Transform Agent;

    // Ubicamos la posicion de nuestro objeto detector
    public Transform VisionObject;

    // Establecemos un angulo de vision para nuestro objeto detector
    // Ademas añadimos un rango de giro entre 0 y 360 gradfos que representan una vuelta completa
    [Range(0f, 360f)]
    public float VisionAngle = 30f;

    // Establecemos la distancia maxima hasta donde se va a poder ubicar o detectar a nuestro agente
    public float VisionDistance = 10f;

    // Creamos un booleano para poder identificar cuando hemos o no detectado a nuestro agente ademas de hacerlo visible en el inspector 
    // por cualquier inconveniente que pueda ocurrir
    [SerializeField] bool detected;

    // Declaramos un Vector3 que seran posteriormente los puntos a partir de donde se dividira el angulo de vision en 2 y asi obtener mitades 
    // para facilitar la deteccion del agente, para esto necesitamos el angulo y distancia maxima 
    Vector3 PointForAngle(float angle, float distance)
    {

        float radians = angle * Mathf.Deg2Rad;

        // Calcular los componentes X, Y y Z del vector tridimensional
        float x = Mathf.Cos(radians) * distance;
        float y = Mathf.Sin(radians) * distance;
        float z = Mathf.Tan(VisionAngle * 0.5f * Mathf.Deg2Rad) * distance;

        // Devolver el vector resultante
        return VisionObject.TransformDirection(new Vector3(x, y, z));
    }
    //------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        //Accedemos al componente renderer
        RendererObject = GetComponent<Renderer>();
        //Establecemos el color inicial
        RendererObject.material.color = ColorNormal;
        //Adquitimos la posicion inicial
        posicionInicial = transform.position;
        //Encontramos a nuestro objeto con el script MouseClick
        TargetGameObject = FindAnyObjectByType<PlayerNavMesh>().gameObject;
        //Adquirimos su rigidbody del objeto
        rbTargetGameObject = TargetGameObject.GetComponent<Rigidbody>();
        //Establecemos la posicion inical del infiltrador
        PosicionInicialAgente = TargetGameObject.transform.position;
    }

    private void Update()
    {
        // Establecemos que siempre se intente establecer que el agente no fue detectado
        detected = false;

        // Creamos un vector 2 para el agente que servira para detectar cuando el mismo este dentro del angulo y rango de deteccion
        // del VisionObject
        Vector3 agentVector = Agent.position - VisionObject.position;


        if (Vector3.Angle(agentVector.normalized, VisionObject.right) < VisionAngle * 0.5f)
        {
            if (agentVector.magnitude < VisionDistance)
            {
                detected = true;
            }
        }

        //Si ha sido detectado
        if (detected == true)
        {
            //El cronometro empezara a sumar 
            CronometroEstado += Time.deltaTime;
            //La cuenta regresiva se restablece
            CuentaRegresiva = 0;
            //Cambiamos la ultima posicion detectada a la agent.position
            ultimaPosicionDetectada = Agent.position;
        }//Si no ha sido detectado
        else if (detected == false)
        {
            //La cuenta regresiva estara actuando
            CuentaRegresiva += Time.deltaTime;
        }

        //Si entramos en estado de ataque comienza el cronometro de pursuit
        if (EstadoActual == Estados.Ataque)
        {
            CronometroPursuit += Time.deltaTime;
        }

        //Con el espacio hacemos que vuelvas a activar al personaje
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CrearNuevoAgente();
        }


    }

    private void FixedUpdate()
    {
        //Si la cuenta regresiva es mayor o igual a 5
        if (CuentaRegresiva >= 3)
        {
            //El estado siempre va a ser estado normal
            CambiarEstado(Estados.Normal);
            //La cuenta regresiva regresara a ser 0 una vez el estado normal este activo
            CuentaRegresiva = 0;
            CronometroEstado = 0;
        }

        if (CronometroPursuit >= 3)
        {
            //El estado siempre va a ser estado normal
            CambiarEstado(Estados.Normal);
            //La cuenta regresiva regresara a ser 0 una vez el estado normal este activo
            CuentaRegresiva = 0;
            //El cronometro de estado y pursuit igual se restablece
            CronometroEstado = 0;
            CronometroPursuit = 0;
        }

        //Si el cronometro de estado es igual o menor a 0 estamos en estado normal, si es mayor a 1 pero menor que 2 estamos en estado alerta
        //Y si es mayor a 2 pasamos al estado de ataque
        if (CronometroEstado <= 0)
        {
            CambiarEstado(Estados.Normal);
        }
        else if (CronometroEstado >= 1 && CronometroEstado < 2)
        {
            CambiarEstado(Estados.Alerta);
        }
        else if (CronometroEstado >= 2)
        {
            CambiarEstado(Estados.Ataque);
        }


    }

    //Creamos un void que nos servira para poder cambiar de estados mediante eventos o variables del juego
    private void CambiarEstado(Estados nuevoEstado)
    {
        Vector3 Distance = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;
        //Cambiamos el estado actual al nuevo estado y de esta forma el nuevo estado nos permitira cambiar entre estados
        EstadoActual = nuevoEstado;
        switch (EstadoActual)
        {
            case Estados.Normal:
     
                RendererObject.material.color = ColorNormal;
                VisionAngle = 30;
                steeringForce = Seek(posicionInicial);
                break;
            case Estados.Alerta:
                steeringForce = Seek(ultimaPosicionDetectada);
                RendererObject.material.color = ColorAlerta;
                VisionAngle = 60f;
                break;
            case Estados.Ataque:
                RendererObject.material.color = ColorAtaque;
                steeringForce = Seek(TargetGameObject.transform.position);
                break;
        }
        steeringForce = Vector3.Min(steeringForce, steeringForce.normalized * maxSteeringForce);

        rb.AddForce(steeringForce, ForceMode.Acceleration);
    }

    //Funcion de arrive del profe
    private float ArriveFunction(Vector3 DistanceVector)
    {
        // te dice si el agente está a una distancia Menor que la del radio de la slowing area
        // (área de reducción de velocidad)
        // requisitos: posición del agente, radio del área, posición del objetivo 
        // calcular la distancia entre mi posición y la posición del objetivo.
        float Distance = DistanceVector.magnitude;
        // usamos esa distancia y la comparamos con el radio del área.
        if (Distance < slowAreaRadius)
        {
            // si la distancia es menor que el radio, Entonces en qué porcentage de velocidad
            // debería ir mi agente
            return Distance / slowAreaRadius;
        }
        // sino, que vaya lo más rápido que pueda.
        return 1.0f;
    }

    //Funcion de GetSteeringForce del profe
    private Vector3 GetSteeringForce(Vector3 DistanceVector)
    {
        Vector3 desiredDirection = DistanceVector.normalized;  // queremos la dirección de ese vector, pero de magnitud 1.

        // queremos ir para esa dirección lo más rápido que se pueda.
        Vector3 desiredVelocity = desiredDirection * maxSpeed;

        if (useArrive)
        {
            // Si vamos a usar arrive, puede que no querramos ir lo más rápido posible.
            float speedPercentage = ArriveFunction(DistanceVector);
            desiredVelocity *= speedPercentage;
        }

        // La diferencia entre la velocidad que tenemos actualmente y la que queremos tener.
        Vector3 steeringForce = desiredVelocity - rb.velocity;

        return steeringForce;
    }

    //Funcion de Seek del profe
    private Vector3 Seek(Vector3 TargetPosition)
    {
        return GetSteeringForce(TargetPosition - transform.position);
    }

    //Funcion pursuit del profe
    private Vector3 Pursuit(Vector3 TargetPosition, Vector3 TargetVelocity)
    {
        Vector3 predictedPosition = PredictPosition(TargetPosition, TargetVelocity);

        return Seek(predictedPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (EstadoActual == Estados.Ataque)
        {
            TargetGameObject.SetActive(false);
            //Destroy(TargetGameObject);
            CambiarEstado(Estados.Normal);
            CronometroPursuit = 5;
            //CrearNuevoAgente();
        }
    }

    private void CrearNuevoAgente()
    {

        if (!Agent.gameObject.activeSelf)
        {
            // Reactivar el agente
            Agent.gameObject.SetActive(true);
            // Reiniciar la posición del agente a la posición inicial
            Agent.position = PosicionInicialAgente;
        }
    }

    //Funcion de predecir posicion del profe
    private Vector3 PredictPosition(Vector3 TargetPosition, Vector3 TargetVelocity)
    {
        // Pursuit no es mucho más que hacerle Seek a la posición futura del objetivo.
        // Primero calculamos el tiempo T que nos tomaría llegar al TargetPosition.
        Vector3 Distance = transform.position - TargetPosition;
        // con esa distancia, podemos saber cuánto tiempo nos tomará recorrer esa 
        // distancia usando nuestra máxima velocidad.
        // TiempoT = Distancia/MaxSpeed
        float predictedTime = Distance.magnitude / maxSpeed;
        // usamos Distance.magnitude porque queremos cuánto mide el vector, no hacia dónde (o no hacia qué dirección).
        // Ahora sí podemos predecir la posición futura de nuestro TargetObject.
        // Su posición futura es: Su posición actual + su velocidad * cuánto tiempo transcurre.
        Vector3 predictedPosition = TargetPosition + TargetVelocity * predictedTime;

        return predictedPosition;
    }

    private void OnDrawGizmos()
    {
        if (VisionAngle <= 0f) return;

        float halfVisionAngle = VisionAngle * 0.5f;

        // Calcular la dirección del cono de visión
        Vector3 forwardDirection = VisionObject.right;

        // Calcular la matriz de rotación para el cono de visión
        Quaternion leftRotation = Quaternion.AngleAxis(-halfVisionAngle, Vector3.up);
        Quaternion rightRotation = Quaternion.AngleAxis(halfVisionAngle, Vector3.up);

        // Calcular los puntos de los extremos del cono de visión
        Vector3 leftPoint = VisionObject.position + leftRotation * forwardDirection * VisionDistance;
        Vector3 rightPoint = VisionObject.position + rightRotation * forwardDirection * VisionDistance;

        // Dibujar las líneas que representan el cono de visión
        Gizmos.color = detected ? Color.red : Color.green;
        Gizmos.DrawLine(VisionObject.position, leftPoint);
        Gizmos.DrawLine(VisionObject.position, rightPoint);

        // Dibujar la línea central del cono de visión
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftPoint, rightPoint);

        // Dibujar el rayo que indica la dirección de visión
        Gizmos.color = Color.white;
        Gizmos.DrawRay(VisionObject.position, forwardDirection * VisionDistance);

        // Dibujar la esfera en la posición del objeto de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(VisionObject.position, 0.5f);
    }
}
