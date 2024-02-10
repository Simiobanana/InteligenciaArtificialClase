using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_SEEK : MonoBehaviour
{
    // Start is called before the first frame update
    //Vector3 Velocity = Vector3.zero;
    //Vector3 Acceleration = Vector3.zero;

    public enum SteeringBehaviour
    {
        None,
        Seek,
        Flee,
        Pursuit,
        Evade,
        Max
    };

    public SteeringBehaviour currentBehaviour = SteeringBehaviour.Seek;

    //Declaramos nuestro vector3 que representara la posicion del mouse
    Vector3 mousePos;
    public float maxSpeed = 1.0f;

    public float maxSteeringForce = 1.0f;

    private Rigidbody rb;

    public GameObject TargetObject;

    public Rigidbody rbTargetGameObject;
    void Start()
    {
        //Nos mueve una unidad en el eje x
        //Velocity = new Vector3(0, 0, 0);
        //Acceleration = new Vector3(0, -9.81f, 0);

        rb = GetComponent<Rigidbody>();

        TargetObject = FindAnyObjectByType<SimpleAgent>().gameObject;
        rbTargetGameObject = FindAnyObjectByType<SimpleAgent>().rb;
    }

    // Update is called once per frame
    // Update se ejecuta cada que puede
    void Update()
    {
        //Nuestra nueva posicion es igual a nuestra posicion actual + nuestra velocidad
        //transform.position = transform.position + (Velocity*Time.deltaTime);
        //Velocity = Velocity + (Acceleration * Time.deltaTime);

        //Determinamos que la posicion del mouse dentro de la camara sea un punto o coordenada visible e interactuable
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Forzamos a que la posicion de la camara en z sea 0
        mousePos.z = 0;

        
    }

    //Fixed: Arreglado o Fijo (Fijo para FixedUpdate)
    //FixedUpdate se actualiza un numero determinado de veces por segundo
    //Generalmente el numero es 30 o 60

    private void FixedUpdate()
    {

        Vector3 Distance = Vector3.zero
            ;
        //Segun el valor del current behaviour es el steering behaviour que vamos a utilizar
        switch (currentBehaviour)
        {
            case SteeringBehaviour.None:
                return;
            case SteeringBehaviour.Seek:
                {
                    Distance = mousePos - transform.position;
                    break;
                }
            case SteeringBehaviour.Flee:
                {
                    Distance = mousePos - transform.position;
                    break;
                }
            case SteeringBehaviour.Pursuit:
                {
                    Distance = transform.position - TargetObject.transform.position;
                    float predictedTime = Distance.magnitude / maxSpeed;
                    //Se usa el .magnitud por que no queremos la direccion si no cuanto es;
                    Vector3 predictedPosition = TargetObject.transform.position + rbTargetGameObject.velocity;

                    Distance = predictedPosition - transform.position;
                    break;
                }
            case SteeringBehaviour.Evade:
                {

                    break;
                }
            case SteeringBehaviour.Max:
                break;
        }


        Vector3 currentDirection = rb.velocity.normalized;
        float currentMagnitude = rb.velocity.magnitude;

        
        Vector3 desiredDirection = Distance.normalized;

        Vector3 desiredVelocity = desiredDirection * maxSpeed;

        Vector3 steeringForce = desiredVelocity - rb.velocity;

        //limitamos la steering force para que agarre la mas chica
        //steeringForce = Vector3.Min(steeringForce, steeringForce.normalized * maxSteeringForce);

        rb.AddForce(steeringForce,ForceMode.Acceleration);

        //Vector3 currentVelocity = currentDirection * currentMagnitude;


        //Aplicamos el metodo de punta menos cola donde la punta es la posicion del mouse y la posicion del agente es la cola
        //Vector3 Distance = mousePos - transform.position;
        //Vector3 agentToMouseDirection = Distance.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, mousePos);
    }

}
