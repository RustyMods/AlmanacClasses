using UnityEngine;

namespace AlmanacClasses.LoadAssets;

public class OrbBob : MonoBehaviour
{
    public float bobSpeed = 1f; // Speed of the bobbing motion
    public float bobHeight = 0.1f; // Maximum height of the bobbing motion

    private Vector3 initialPosition;
    private float randomOffset;

    private void Start()
    {
        // Store the initial position of the object
        initialPosition = transform.position;
        // Generate a random offset to add some variety to the bobbing motion
        randomOffset = UnityEngine.Random.Range(0f, 100f);
    }

    private void Update()
    {
        // Calculate the vertical position based on time and a random offset
        float yOffset = Mathf.Sin((Time.time + randomOffset) * bobSpeed) * bobHeight;

        // Apply the bobbing motion to the object's position
        transform.position = initialPosition + new Vector3(0f, yOffset, 0f);
    }
}