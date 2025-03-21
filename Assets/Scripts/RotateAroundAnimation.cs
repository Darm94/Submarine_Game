using UnityEngine;

public class RotateAroundAnimation : MonoBehaviour
{
    [SerializeField] private Transform rotationCenter; // Oggetto attorno al quale ruotare
    public float rotationSpeed = 30f; // Velocità di rotazione (gradi al secondo)
    public Vector3 rotationAxis = Vector3.up; // Asse di rotazione

    void Update()
    {
        if (rotationCenter != null)
        {
            // Ruota il pesce intorno ad un oggetto
            transform.RotateAround(rotationCenter.position, rotationAxis, -rotationSpeed * Time.deltaTime);
        }
    }
}
