using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectExampleTest : MonoBehaviour
{
    [Header("Destroy Settings:")]
    public float destroyDistance = 15.0f;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, startPosition) > destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
