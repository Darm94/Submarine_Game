using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 direction = Vector3.up;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float delta = 1;
    private Vector3 _startPosition;
    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.position = _startPosition + delta*Mathf.Sin(Time.time * speed)*direction;
        
    }
}
