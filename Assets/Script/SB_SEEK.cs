using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using static SB_SEEK;

public class SB_SEEK : MonoBehaviour
{
    public enum SteeringBehavior
    {
        None,  // 0
        Seek,   // 1
        Flee,  // 2
        Pursuit, // 3
        Evade,  // 4
        Wander, // 5
        MAX     // 6
    };

    public SteeringBehavior currentBehavior = SteeringBehavior.Seek;

    // Bandera que determina si nuestro steering behavior activo debe detenerse al llegar al punto objetivo.
    // es decir, usar el arrive steering behavior.
    public bool useArrive = true;

    // radio del área en que nuestro agente que use arrive va a empezar a reducir su velocidad.
    public float slowAreaRadius = 5.0f;

    // Vector tridimensional para la posición del mouse en el mundo
    Vector3 mouseWorldPos = Vector3.zero;

    public float maxSpeed = 1.0f;

    // Qué tanto tiempo queremos que pase antes de aplicar toda la steering force.
    public float maxSteeringForce = 1.0f;

    // Rigidbody ya trae dentro:
    // Vector tridimensional para la aceleración.
    // Vector tridimensional para representar esa velocidad
    private Rigidbody rb;

    // Variable donde guardamos la referencia al GameObject que es nuestro objetivo.
    private GameObject TargetGameObject;
    // Referencia al Rigidbody del TargetGameObject.
    private Rigidbody rbTargetGameObject;

    private Vector3 TargetPosition = Vector3.zero;

    // Variables para Wander.
    public float sphereDistance = 1.0f;
    public float sphereRadius = 5.0f;

    // Si nuestro objetivo está a X de distancia,
    // y nuestro agente se mueve a Y de distancia por segundo,
    // cuántos segundos nos toma llegar hasta X?
    // 0 + Y/s = X
    // s = X/Y
    // 5/s = 20
    // s = 20/5 = 4

    // Start is called before the first frame update
    void Start()
    {
        print("Funcion Start");
        rb = GetComponent<Rigidbody>();

        TargetGameObject = FindAnyObjectByType<SimpleAgent>().gameObject;
        rbTargetGameObject = TargetGameObject.GetComponent<Rigidbody>();
    }

    //void myFunction()
    //{
    //    print("1");
    //    print("2");
    //    print("3");

    //    // myFunction();
    //}

    // Update is called once per frame
    protected void Update()
    {
        // Lo que esté dentro de la función update, se va a ejecutar cada que se pueda.
        // print("Funcion update");

        // Input.mousePosition // Nos da coordenadas en pixeles.
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        mouseWorldPos.z = 0;  // la sobreescribimos con 0 para que no afecte los cálculos en 2D.

        // print(this.name);
        print(base.GetType());

        //velocity = 

        // Vector3.Angle

        // print(mousePos);

        // ¿Qué son coordenadas de Pixel?
        // 1920 (en X) * 1080 (en Y)
        // Esto nos da las coordenadas Normalizadas.
        // Normalizado es que va de 0 a 1
        // 0 de pixeles es el 0 de lo normalizado.
        // 1920 de pixeles el 1 de lo normalizado.
        // Qué número de pixel estaría en el 0.5 de lo normalizado? El 960.
        // 1920 * 0.5 = 960
        // 1920 * 0.2 = 384

        // Aplicamos método de Punta Menos Cola
        // Ya tenemos la posición del Mouse, Y ya tenemos la posición del agente.
        // Podemos decir la posición del mouse es la Punta,
        // ¿cuál es nuestra posición del mouse en el Código? la variable "mousePos".
        // y la posición de nuestro agente es la cola.
        // ¿Cuál es la posición de nuestro agente en el Código?
        // es la variable position del componente Transform de este GameObject
        // es decir: "transform.position".
        //                  Punta           Menos       Cola
        // Vector3 Distance = mouseWorldPos      -     transform.position;
        // print(Distance);


        // print(Time.deltaTime);

        // Nuestra velocidad es igual a nuestra velocidad actual + (la aceleración * tiempo transcurrido)
        // velocity = velocity + (acceleration * Time.deltaTime);

        // Nuestra nueva posición es igual a nuestra posición actual + (la velocidad * tiempo transcurrido)
        // transform.position = transform.position + (velocity * Time.deltaTime);

        // Velocidad está definido como: Distancia / Tiempo

        // Distancia + Distancia/Tiempo  (Esto de aquí no procedería)

        // Distancia + (Distancia/Tiempo)*Tiempo

        // Aceleración Distancia/Tiempo^2
    }

    int myCounter = 0;
    void FixedUpdate()
    {
        // Fixed: Fijo
        // Solo se ejecuta un número fijo de veces por segundo.
        // Generalmente, ese es número es 60 (o 30).
        // print("Funcion fixedUpdate");

        // La declaramos aquí para poder usarla DENTRO del switch, pero que siga viva al salir del switch.
        Vector3 Distance = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;

        // Según el valor de la variable currentBehavior, es cuál Steering Behavior vamos a ejecutar.
        switch (currentBehavior)
        {
            case SteeringBehavior.None:
                {
                    return;
                    // break;
                }
            case SteeringBehavior.Seek:
                {
                    // En qué dirección vamos a hacer que se mueva nuestro agente? En la dirección en la que está el mouse.
                    // Cuando hablemos de dirección, queremos vectores normalizados (es decir, de magnitude 1).
                    // Distance = mouseWorldPos - transform.position;
                    steeringForce = Seek(mouseWorldPos);
                    break;
                }
            case SteeringBehavior.Flee:
                {
                    // En qué dirección vamos a hacer que se mueva nuestro agente? En la dirección en la que está el mouse.
                    // Cuando hablemos de dirección, queremos vectores normalizados (es decir, de magnitude 1).
                    // Distance = transform.position - mouseWorldPos;
                    steeringForce = Flee(mouseWorldPos);
                    break;
                }
            case SteeringBehavior.Pursuit:
                {
                    steeringForce = Pursuit(TargetGameObject.transform.position, rbTargetGameObject.velocity);
                }
                break;
            case SteeringBehavior.Evade:
                {
                    steeringForce = Evade(TargetGameObject.transform.position, rbTargetGameObject.velocity);
                }
                break;
            case SteeringBehavior.Wander:
                {
                    //if (myCounter > 60)
                    //{
                    //    TargetPosition = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0.0f);
                    //    steeringForce = Seek(TargetPosition);
                    //    myCounter = 0;
                    //}
                    //else
                    //{
                    //    myCounter++;
                    //}

                    // Lo primero es proyectar este círculo en frente de nuestro agente.
                    // Cuál es el frente de nuestro agente?
                    // gameObject.transform.forward
                    Vector3 LookingDirection = rb.velocity.normalized;
                    // a partir de la posición de nuestro agente, la esfera va a estar 
                    // desplazada sphereDistance en la dirección de LookingDirection
                    Vector3 spherePosition = gameObject.transform.position + LookingDirection * sphereDistance;
                    Vector3 unitCircle = Random.insideUnitCircle;
                    Vector3 TargetInsideSphere = spherePosition + (unitCircle * sphereRadius);
                }
                break;
            case SteeringBehavior.MAX:
                break;
        }


        // Dirección actual de movimiento de nuestro agente:
        // La vamos a obtener a partir de la velocidad que trae.
        // Vector3 currentDirection = rb.velocity.normalized;  // Nos da la dirección. Es un vector de magnitud 1.
        // float currentMagnitude = rb.velocity.magnitude;



        // Aquí la limitamos a que sea la mínima entre la fuerza que marca el algoritmo y la máxima
        // que deseamos que pueda tener.
        steeringForce = Vector3.Min(steeringForce, steeringForce.normalized * maxSteeringForce);

        rb.AddForce(steeringForce, ForceMode.Acceleration);
    }

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

    private Vector3 Seek(Vector3 TargetPosition)
    {
        return GetSteeringForce(TargetPosition - transform.position);
    }

    private Vector3 Flee(Vector3 TargetPosition)
    {
        return GetSteeringForce(transform.position - TargetPosition);
    }

    private Vector3 Pursuit(Vector3 TargetPosition, Vector3 TargetVelocity)
    {
        Vector3 predictedPosition = PredictPosition(TargetPosition, TargetVelocity);

        return Seek(predictedPosition);
    }

    private Vector3 Evade(Vector3 TargetPosition, Vector3 TargetVelocity)
    {
        Vector3 predictedPosition = PredictPosition(TargetPosition, TargetVelocity);

        return Flee(predictedPosition);
    }

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

    // Pursuit
    // Idea general es "no vayas ahorita hacia donde está tu objetivo,
    // ve hacia donde va a estar después de cierto tiempo."
    // queremos saber la posición actual de ese target.
    // queremos saber la velocidad actual de ese target.
    // ¿cuál va a ser el tiempo en el que queremos predecir su posición?
    // 


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, mouseWorldPos);

        // Esto es para verificar que Pursuit está funcionando adecuadamente.
        // Comprobamos que sí lo hace.
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, 
        //    PredictPosition(TargetGameObject.transform.position, rbTargetGameObject.velocity));

        Gizmos.color = Color.white;
        // Gizmos.DrawSphere(TargetPosition, 1);

        if (rb != null)
        {
            Vector3 LookingDirection = rb.velocity.normalized;
            // Esto de aquí nos va a dar una línea chiquita.
            Gizmos.DrawLine(transform.position, transform.position + LookingDirection);

            Vector3 spherePosition = transform.position + LookingDirection * sphereDistance;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, spherePosition);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spherePosition, sphereRadius);

            // a partir de la posición de nuestro agente, la esfera va a estar 
            // desplazada sphereDistance en la dirección de LookingDirection
            // Vector3 spherePosition = gameObject.transform.position + LookingDirection * sphereDistance;
            Vector3 unitCircle = Random.onUnitSphere;
            // Punto en el círculo/esfera al cual vamos a hacer seek.
            Vector3 TargetInsideSphere = spherePosition + (unitCircle * sphereRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(TargetInsideSphere, 1.0f);
        }




    }
}
