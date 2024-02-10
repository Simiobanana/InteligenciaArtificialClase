using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
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
        // Aqui regresamos que la vision del objeto este situada justo en el o junto al VisionObject y para esto ocupamos el TransformDirecion
        // (De lo contrario este angulo apareceria en otra ubicacion del entorno)
        return VisionObject.TransformDirection(

            // Creamos un nuevo vector2 y ocupando trigonometria y formulas matematicas determinamos el coseno y seno del angulo total para
            // posteriormente convertirlo a radianes y finalmente multiplicar ambos resultados por la distancia maxima de vision
            new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))) * distance;
    }

    private void Update()
    {
        // Establecemos que siempre se intente establecer que el agente no fue detectado
        detected = false;

        // Creamos un vector 2 para el agente que servira para detectar cuando el mismo este dentro del angulo y rango de deteccion
        // del VisionObject
        Vector2 agentVector = Agent.position - VisionObject.position;

        // Si el angulo que se fotma entre el agentVector y la posicion derecha del visionObject es menor a la mitad del visionAngle y ...
        if(Vector3.Angle(agentVector.normalized, VisionObject.right) < VisionAngle * 0.5f)
        {
            // ... Si la magnitud/tamaño del vector entre el agente y el VisionObject es menor a la distancia de vision del VisionObject 
            // se establece que ha sido detectado el agente
            if(agentVector.magnitude < VisionDistance)
            {
                detected=true;
            }
        }
    }


    // Utilizamos el metodo OnDrawGizmos para poder visualizar el rango y angulo de vision de nuestro objeto
    private void OnDrawGizmos()
    {
        // Si nuestro anugulo de vision es igual o menor a 0 no se dibuja nada
        if (VisionAngle <= 0f) return;
        
        // Dividimos el angulo de vision a la mitad para facilitar la deteccion
        float HalfVisionAngle = VisionAngle * 0.5f ;

        // p1 = primera mitad del angulo, p2 = segunda mitad del angulo
        Vector2 p1, p2;

        // Establecemos quien es la mitad "positiva" y quien es la mitad "negativa" en cuestion de las mitades del angulo
        p1 = PointForAngle(HalfVisionAngle, VisionDistance);
        p2 = PointForAngle(-HalfVisionAngle, VisionDistance);

        // Establecemos que el color cuando es detectado sea rojo y cuando no lo este sea de color verde
        Gizmos.color = detected ? Color.red : Color.green;

        // Dibujamos el limite de hasta donde llega el angulo de deteccion tanto de la mitad superior o primera mitad asi como de la mitad inferior
        // o segunda mitad
        Gizmos.DrawLine(VisionObject.position, (Vector2) VisionObject.position + p1);
        Gizmos.DrawLine(VisionObject.position, (Vector2) VisionObject.position + p2);

        // Desde donde esta nuestro objeto tiramos un rayo en su direccion de la derecha con la finalidad de saber para donde esta apuntando
        Gizmos.DrawRay(VisionObject.position, VisionObject.right * 4f);
    }

    
}
