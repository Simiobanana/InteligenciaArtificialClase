using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAgent : MonoBehaviour
{
    // Start is called before the first frame update
    float currentTime = 0.0f;
    public Rigidbody rb;

    public float MaxSpeed = 1;
    public float MaxSteeringForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        float xPos = Mathf.Sin(currentTime);
        float yPos = Mathf.Cos(currentTime);

        Vector3 TargetPosition = new Vector3 (xPos, yPos, 0.0f);

        //Vector3 Distance 
    }
}
