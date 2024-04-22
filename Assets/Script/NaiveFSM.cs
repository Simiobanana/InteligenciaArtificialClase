using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaiveFSM : MonoBehaviour
{
    public NaiveFSMState _CurrentState;

    // Que pueda transicionar entre todos los estados de nuestro diseño (de nuestro diagrama o lo que sea que tengamos).
    // Para ello, hacemos que la FSM los contenga.
    //NaivePatrolState _PatrolState;
    //NaiveAlertState _AlertState;
    //NaiveAttackState _AttackState;
    // public Dictionary<string, NaiveFSMState> _StatesDict;

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Una FSM siempre inicia en su estado inicial.
        _CurrentState = GetInitialState();
        // Ahora nos toca entrar al estado (es decir, llamar su función Enter() )
        if (_CurrentState == null)
            Debug.LogError("No hay un estado inicial válido asignado.");
        else
        {
            _CurrentState.Enter();
        }
    }

    protected virtual NaiveFSMState GetInitialState()
    {
        // Regresa null para que cause error porque la función de esta clase padre nunca debe de usarse, siempre 
        // se le debe de hacer un override.
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // Mayormente solo debe importar lo que hace el estado actual, y nada de otros estados de la máquina.
        if (_CurrentState != null)
            _CurrentState.Update();
    }

    // Lo mismo va a pasar con el FixedUpdate.

    // La función para cambiar entre estados.
    public void ChangeState(NaiveFSMState newState)
    {
        // Manda a llamar el Exit() del estado actual.
        _CurrentState.Exit();
        // Pone que el estado nuevo es ahora el estado actual (current)
        _CurrentState = newState;
        // Manda a llamar el Enter() de este nuevo estado.
        _CurrentState.Enter();
    }

    private void OnGUI()
    {
        string text = _CurrentState != null ? _CurrentState.Name : "No current State asigned";
        GUILayout.Label($"<size=40>{text}</size>");
    }
}
