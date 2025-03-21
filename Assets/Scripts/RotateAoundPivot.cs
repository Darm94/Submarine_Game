using UnityEngine;

public class RotateAoundPivot : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private Vector3 axis = Vector3.down;
    [SerializeField] private bool randomAngle = false;
    [SerializeField] private Space rotationSpace = Space.Self;

    [SerializeField] private bool _animate; //use it at true only if want to start with a visible object

    private void Start()
    {
        if (randomAngle)
        {
            transform.Rotate(axis, Random.Range(0, 360), rotationSpace);
        }
    }

    private void Update()
    {
        if (_animate)
        {
            transform.Rotate(axis, Time.deltaTime * speed, rotationSpace);
            //Debug.Log(transform.rotation.eulerAngles);
        }
    }

    private void OnBecameVisible()
    {
        _animate = true;
    }

    private void OnBecameInvisible()
    {
        _animate = false;
    }
}
