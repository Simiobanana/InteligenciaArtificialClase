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

    // radio del �rea en que nuestro agente que use arrive va a empezar a reducir su velocidad.
    public float slowAreaRadius = 5.0f;

    // Vector tridimensional para la posici�n del mouse en el mundo
    Vector3 mouseWorldPos = Vector3.zero;

    public float maxSpeed = 1.0f;

    // Qu� tanto tiempo queremos que pase antes de aplicar toda la steering force.
    public float maxSteeringForce = 1.0f;

    // Rigidbody ya trae dentro:
    // Vector tridimensional para la aceleraci�n.
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

    // Si nuestro objetivo est� a X de distancia,
    // y nuestro agente se mueve a Y de distancia por segundo,
    // cu�ntos segundos nos toma llegar hasta X?
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
        // Lo que est� dentro de la funci�n update, se va a ejecutar cada que se pueda.
        // print("Funcion update");

        // Input.mousePosition // Nos da coordenadas en pixeles.
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        mouseWorldPos.z = 0;  // la sobreescribimos con 0 para que no afecte los c�lculos en 2D.

        // print(this.name);
        print(base.GetType());

        //velocity = 

        // Vector3.Angle

        // print(mousePos);

        // �Qu� son coordenadas de Pixel?
        // 1920 (en X) * 1080 (en Y)
        // Esto nos da las coordenadas Normalizadas.
        // Normalizado es que va de 0 a 1
        // 0 de pixeles es el 0 de lo normalizado.
        // 1920 de pixeles el 1 de lo normalizado.
        // Qu� n�mero de pixel estar�a en el 0.5 de lo normalizado? El 960.
        // 1920 * 0.5 = 960
        // 1920 * 0.2 = 384

        // Aplicamos m�todo de Punta Menos Cola
        // Ya tenemos la posici�n del Mouse, Y ya tenemos la posici�n del agente.
        // Podemos decir la posici�n del mouse es la Punta,
        // �cu�l es nuestra posici�n del mouse en el C�digo? la variable "mousePos".
        // y la posici�n de nuestro agente es la cola.
        // �Cu�l es la posici�n de nuestro agente en el C�digo?
        // es la variable position del componente Transform de este GameObject
        // es decir: "transform.position".
        //                  Punta           Menos       Cola
        // Vector3 Distance = mouseWorldPos      -     transform.position;
        // print(Distance);


        // print(Time.deltaTime);

        // Nuestra velocidad es igual a nuestra velocidad actual + (la aceleraci�n * tiempo transcurrido)
        // velocity = velocity + (acceleration * Time.deltaTime);

        // Nuestra nueva posici�n es igual a nuestra posici�n actual + (la velocidad * tiempo transcurrido)
        // transform.position = transform.position + (velocity * Time.deltaTime);

        // Velocidad est� definido como: Distancia / Tiempo

        // Distancia + Distancia/Tiempo  (Esto de aqu� no proceder�a)

        // Distancia + (Distancia/Tiempo)*Tiempo

        // Aceleraci�n Distancia/Tiempo^2
    }

    int myCounter = 0;
    void FixedUpdate()
    {
        // Fixed: Fijo
        // Solo se ejecuta un n�mero fijo de veces por segundo.
        // Generalmente, ese es n�mero es 60 (o 30).
        // print("Funcion fixedUpdate");

        // La declaramos aqu� para poder usarla DENTRO del switch, pero que siga viva al salir del switch.
        Vector3 Distance = Vector3.zero;
        Vector3 steeringForce = Vector3.zero;

        // Seg�n el valor de la variable currentBehavior, es cu�l Steering Behavior vamos a ejecutar.
        switch (currentBehavior)
        {
            case SteeringBehavior.None:
                {
                    return;
                    // break;
                }
            case SteeringBehavior.Seek:
                {
                    // En qu� direcci�n vamos a hacer que se mueva nuestro agente? En la direcci�n en la que est� el mouse.
                    // Cuando hablemos de direcci�n, queremos vectores normalizados (es decir, de magnitude 1).
                    // Distance = mouseWorldPos - transform.position;
                    steeringForce = Seek(mouseWorldPos);
                    break;
                }
            case SteeringBehavior.Flee:
                {
                    // En qu� direcci�n vamos a hacer que se mueva nuestro agente? En la direcci�n en la que est� el mouse.
                    // Cuando hablemos de direcci�n, queremos vectores normalizados (es decir, de magnitude 1).
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

                    // Lo primero es proyectar este c�rculo en frente de nuestro agente.
                    // Cu�l es el frente de nuestro agente?
                    // gameObject.transform.forward
                    Vector3 LookingDirection = rb.velocity.normalized;
                    // a partir de la posici�n de nuestro agente, la esfera va a estar 
                    // desplazada sphereDistance en la direcci�n de LookingDirection
                    Vector3 spherePosition = gameObject.transform.position + LookingDirection * sphereDistance;
                    Vector3 unitCircle = Random.insideUnitCircle;
                    Vector3 TargetInsideSphere = spherePosition + (unitCircle * sphereRadius);
                }
                break;
            case SteeringBehavior.MAX:
                break;
        }


        // Direcci�n actual de movimiento de nuestro agente:
        // La vamos a obtener a partir de la velocidad que trae.
        // Vector3 currentDirection = rb.velocity.normalized;  // Nos da la direcci�n. Es un vector de magnitud 1.
        // float currentMagnitude = rb.velocity.magnitude;



        // Aqu� la limitamos a que sea la m�nima entre la fuerza que marca el algoritmo y la m�xima
        // que deseamos que pueda tener.
        steeringForce = Vector3.Min(steeringForce, steeringForce.normalized * maxSteeringForce);

        rb.AddForce(steeringForce, ForceMode.Acceleration);
    }

    private Vector3 GetSteeringForce(Vector3 DistanceVector)
    {
        Vector3 desiredDirection = DistanceVector.normalized;  // queremos la direcci�n de ese vector, pero de magnitud 1.

        // queremos ir para esa direcci�n lo m�s r�pido que se pueda.
        Vector3 desiredVelocity = desiredDirection * maxSpeed;

        if (useArrive)
        {
            // Si vamos a usar arrive, puede que no querramos ir lo m�s r�pido posible.
            float speedPercentage = ArriveFunction(DistanceVector);
            desiredVelocity *= speedPercentage;
        }

        // La diferencia entre la velocidad que tenemos actualmente y la que queremos tener.
        Vector3 steeringForce = desiredVelocity - rb.velocity;

        return steeringForce;
    }

    private float ArriveFunction(Vector3 DistanceVector)
    {
        // te dice si el agente est� a una distancia Menor que la del radio de la slowing area
        // (�rea de reducci�n de velocidad)
        // requisitos: posici�n del agente, radio del �rea, posici�n del objetivo 
        // calcular la distancia entre mi posici�n y la posici�n del objetivo.
        float Distance = DistanceVector.magnitude;
        // usamos esa distancia y la comparamos con el radio del �rea.
        if (Distance < slowAreaRadius)
        {
            // si la distancia es menor que el radio, Entonces en qu� porcentage de velocidad
            // deber�a ir mi agente
            return Distance / slowAreaRadius;
        }
        // sino, que vaya lo m�s r�pido que pueda.
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
        // Pursuit no es mucho m�s que hacerle Seek a la posici�n futura del objetivo.
        // Primero calculamos el tiempo T que nos tomar�a llegar al TargetPosition.
        Vector3 Distance = transform.position - TargetPosition;
        // con esa distancia, podemos saber cu�nto tiempo nos tomar� recorrer esa 
        // distancia usando nuestra m�xima velocidad.
        // TiempoT = Distancia/MaxSpeed
        float predictedTime = Distance.magnitude / maxSpeed;
        // usamos Distance.magnitude porque queremos cu�nto mide el vector, no hacia d�nde (o no hacia qu� direcci�n).
        // Ahora s� podemos predecir la posici�n futura de nuestro TargetObject.
        // Su posici�n futura es: Su posici�n actual + su velocidad * cu�nto tiempo transcurre.
        Vector3 predictedPosition = TargetPosition + TargetVelocity * predictedTime;

        return predictedPosition;
    }

    // Pursuit
    // Idea general es "no vayas ahorita hacia donde est� tu objetivo,
    // ve hacia donde va a estar despu�s de cierto tiempo."
    // queremos saber la posici�n actual de ese target.
    // queremos saber la velocidad actual de ese target.
    // �cu�l va a ser el tiempo en el que queremos predecir su posici�n?
    // 


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, mouseWorldPos);

        // Esto es para verificar que Pursuit est� funcionando adecuadamente.
        // Comprobamos que s� lo hace.
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, 
        //    PredictPosition(TargetGameObject.transform.position, rbTargetGameObject.velocity));

        Gizmos.color = Color.white;
        // Gizmos.DrawSphere(TargetPosition, 1);

        if (rb != null)
        {
            Vector3 LookingDirection = rb.velocity.normalized;
            // Esto de aqu� nos va a dar una l�nea chiquita.
            Gizmos.DrawLine(transform.position, transform.position + LookingDirection);

            Vector3 spherePosition = transform.position + LookingDirection * sphereDistance;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, spherePosition);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spherePosition, sphereRadius);

            // a partir de la posici�n de nuestro agente, la esfera va a estar 
            // desplazada sphereDistance en la direcci�n de LookingDirection
            // Vector3 spherePosition = gameObject.transform.position + LookingDirection * sphereDistance;
            Vector3 unitCircle = Random.onUnitSphere;
            // Punto en el c�rculo/esfera al cual vamos a hacer seek.
            Vector3 TargetInsideSphere = spherePosition + (unitCircle * sphereRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(TargetInsideSphere, 1.0f);
        }




    }
}
