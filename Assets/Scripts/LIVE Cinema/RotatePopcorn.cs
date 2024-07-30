using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePopcorn : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Vector2 scaleRange = new Vector2(0.6f, 0.8f);
    public float positionOffsetRange = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
        float randomX = Random.Range(0f, 360f);
        float randomY = Random.Range(0f, 360f);
        float randomZ = Random.Range(0f, 360f);

        Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);
        transform.rotation = randomRotation;

        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        float randomXOffset = Random.Range(-positionOffsetRange, positionOffsetRange);
        float randomYOffset = Random.Range(-positionOffsetRange, positionOffsetRange);
        float randomZOffset = Random.Range(-positionOffsetRange, positionOffsetRange);

        transform.position += new Vector3(randomXOffset, randomYOffset, randomZOffset);

        // Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        // rb.useGravity = true; // Enable gravity for the Rigidbody
        // SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        // collider.radius = 0.5f; // Adjust the collider size as needed
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
