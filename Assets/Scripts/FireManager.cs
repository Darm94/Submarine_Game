using UnityEngine;

public class FireManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float firePower = 100;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private ForceMode fireMode = ForceMode.Impulse;
    [SerializeField] private Transform[] firePositions;
    [SerializeField] private Transform root;

    float _fireTimer = 0;

    private void Update()
    {
        _fireTimer += Time.deltaTime;

        if (_fireTimer < fireRate) return;

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log($"Fire1: {transform.eulerAngles}");

            for (int i = 0; i < firePositions.Length; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePositions[i].position, bulletPrefab.transform.rotation * root.rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddRelativeForce(Vector3.down * firePower, fireMode); // or Vector3.down
            }

            _fireTimer = 0;
        }
    }
}
