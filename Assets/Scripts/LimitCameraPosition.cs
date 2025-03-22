using UnityEngine;

public class LimitCameraPosition : MonoBehaviour
{
    [SerializeField] private float maxPositionY = 5f; // Posizione Y massima
    [SerializeField] private float minPositionY = -5f; // Posizione Y minima
    private float originalPositionY;

    void Start()
    {
        // Salva la posizione Y originale della camera.
        originalPositionY = transform.position.y;
    }

    void Update()
    {
        // Limita la posizione Y della camera all'intervallo specificato.
        float newPositionY = Mathf.Clamp(transform.position.y, minPositionY + originalPositionY, maxPositionY + originalPositionY);

        // Applica la nuova posizione alla camera.
        transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
    }
    
}
