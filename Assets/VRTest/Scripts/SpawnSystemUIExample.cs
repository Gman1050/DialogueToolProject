using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A example class to show how spawning can work through UI. Public methods are called in onClick() event in Button.cs component.
/// </summary>
public class SpawnSystemUIExample : MonoBehaviour
{
    [Header("Example Spawn Objects:")]
    public GameObject cubeObject;
    public GameObject sphereObject;
    public GameObject capsuleObject;

    [Header("Spawn Settings:")]
    public Transform spawnPosition;

    void Start()
    {
        if (!spawnPosition)
            spawnPosition = transform;
    }

    public void SpawnCubeObject()
    {
        Instantiate(cubeObject, spawnPosition.position, Quaternion.identity);
    }

    public void SpawnSphereObject()
    {
        Instantiate(sphereObject, spawnPosition.position, Quaternion.identity);
    }

    public void SpawnCapsuleObject()
    {
        Instantiate(capsuleObject, spawnPosition.position, Quaternion.identity);
    }
}
