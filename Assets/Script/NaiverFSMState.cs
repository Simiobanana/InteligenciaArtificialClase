using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// No hereda de monobehaviour porque no se le asignan al gameObject en sí, sino a la máquina de estados que sí
// se le asignará a nuestro GameObject.
// Esta es la clase del estado Base, es decir, la clase de la cual van a heredar todos los estados que usará la FSM.
public class NaiveFSMState
{
    // Un estado debe de tener un nombre reconocible por nosotros Humanos.
    public string Name;

    // Esta referencia a la Máquina de estados nos permite acceder a las cosas de nuestro GameObject dueño de este estado.
    // Todo estado creado debe de recibir esta referencia.
    protected NaiveFSM _FSM;

    // Los estados únicamente se puede instanciar usando este constructor.
    // Eso evita que se creen estados que no tengan un nombre ni una referencia a la FSM que es su dueña.
    //public NaiveFSMState(string name, NaiveFSM FSM)
    //{
    //    Name = name;
    //    _FSM = FSM;
    //}


    // Tiene 3 funciones Enter, Update, Exit
    public virtual void Enter()
    {
        Debug.Log("Entré al estado: " + Name);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // No hace nada.
        // Podríamos poner el print, pero puede ser algo molesto. Ponerlo a discreción en los estados
        // que ustedes lo requieran.
        Debug.Log("Update del estado Base.");
    }

    public virtual void Exit()
    {
        Debug.Log("Salí del estado: " + Name);
    }
}
