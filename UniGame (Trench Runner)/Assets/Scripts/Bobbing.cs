using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float speed = 2f;
    public float height = 0.3f;

    public Vector3 startPosition;

    // This finds the start position of the prefab
    void Start()
    {
        startPosition = transform.position;
    }

    // This will raise the prefab up and down to create a bobbing effect and also slowly rotate it around itself
    void Update()
    {
        //MATHS CONTENT HERE FOR ROTATING HEALTHPACK AROUND ITSELF - EULER ANGLES
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(0f, 50 * Time.deltaTime, 0f, Space.Self);
    }
}
