using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    private CharacterController characterController;
    public Transform camera;
    public float speed = 4;
    public float runSpeed = 8;
    private float gravity = -9.8f;

    // Declaro el animator
    private Animator animator;
    //bool Moviendose;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        //Moviendose = false;
    }

    private void Update()
    {
        // Defino que si el personaje no se esta moviendo su booleano sea falso y asi se inicie la animacion de idle
        animator.SetBool("Moviendose", false);

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 movement = Vector3.zero;
        float movementSpeed = 0;

        if (hor != 0 || ver != 0)
        {
            Vector3 forward = camera.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = camera.right;
            right.y = 0;
            right.Normalize();

            Vector3 direction = forward * ver + right * hor;
            movementSpeed = Mathf.Clamp01(direction.magnitude);
            direction.Normalize();

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                movement = direction * runSpeed * movementSpeed * Time.deltaTime;

                // Determino que cuando el personaje se esté moviendo se inicie la animación de movimiento
                animator.SetBool("Moviendose", true);
                animator.SetBool("Agacharse", true);
            }
            else
            {
                movement = direction * speed * movementSpeed * Time.deltaTime;

                // Determino que cuando el personaje se esté moviendo se inicie la animación de movimiento
                animator.SetBool("Moviendose", true);
                animator.SetBool("Agacharse", false);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.2f);

            // Si el audio no se está reproduciendo, comienza a reproducirlo en bucle
            
        }
        else
        {
            // Si el personaje no se está moviendo, detiene la reproducción del audio
            
        }

        // Añade la gravedad al personaje
        movement.y += gravity * Time.deltaTime;
        // Aplica el movimiento
        characterController.Move(movement);
    }

}

