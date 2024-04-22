using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// No hereda de monobehaviour porque no se le asignan al gameObject en s�, sino a la m�quina de estados que s�
// se le asignar� a nuestro GameObject.
// Esta es la clase del estado Base, es decir, la clase de la cual van a heredar todos los estados que usar� la FSM.
public class NaiveFSMState
{
    // Un estado debe de tener un nombre reconocible por nosotros Humanos.
    public string Name;

    // Esta referencia a la M�quina de estados nos permite acceder a las cosas de nuestro GameObject due�o de este estado.
    // Todo estado creado debe de recibir esta referencia.
    protected NaiveFSM _FSM;

    // Los estados �nicamente se puede instanciar usando este constructor.
    // Eso evita que se creen estados que no tengan un nombre ni una referencia a la FSM que es su due�a.
    //public NaiveFSMState(string name, NaiveFSM FSM)
    //{
    //    Name = name;
    //    _FSM = FSM;
    //}


    // Tiene 3 funciones Enter, Update, Exit
    public virtual void Enter()
    {
        Debug.Log("Entr� al estado: " + Name);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // No hace nada.
        // Podr�amos poner el print, pero puede ser algo molesto. Ponerlo a discreci�n en los estados
        // que ustedes lo requieran.
        Debug.Log("Update del estado Base.");
    }

    public virtual void Exit()
    {
        Debug.Log("Sal� del estado: " + Name);
    }
}
