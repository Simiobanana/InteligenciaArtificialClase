using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerController : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ManualSkills();
    }

    private void ManualSkills()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("Q");
            Debug.Log("Se pulso la Q");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("E");
            Debug.Log("Se pulso la E");
        }
            

        
    }
}
