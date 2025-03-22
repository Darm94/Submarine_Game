using UnityEngine;

public class DestroyAtTopPosition : MonoBehaviour
{
    public float maxY = 15f; // Soglia Y per la distruzione

    void Update()
    {
        // Verifica se la posizione Y dell'oggetto supera la soglia.
        if (transform.position.y > maxY)
        {
            // Distruggi l'oggetto.
            Destroy(gameObject);
        }
    }
}
