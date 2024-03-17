using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone3D : MonoBehaviour
{
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

    private void Update()
    {
        // Establecemos que siempre se intente establecer que el agente no fue detectado
        detected = false;

        Vector3 agentVector = Agent.position - VisionObject.position;


        if (Vector3.Angle(agentVector.normalized, VisionObject.right) < VisionAngle * 0.5f)
        {
            if (agentVector.magnitude < VisionDistance)
            {
                detected = true;
            }
        }
    }


    // Utilizamos el metodo OnDrawGizmos para poder visualizar el rango y angulo de vision de nuestro objeto
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
