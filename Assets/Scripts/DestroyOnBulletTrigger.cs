using UnityEngine;

public class DestroyOnBulletTrigger : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject spawnOnDestroy;
    [SerializeField] [Range(1,10)] private int maxInstances = 5;
    [SerializeField] private Vector3 randomDelta = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        if (spawnOnDestroy)
        {
            int realInstances = Random.Range(1, maxInstances);

            for (int i = 0; i < realInstances; i++)
            {
                GameObject go = Instantiate(spawnOnDestroy, transform.position + 
                                                            randomDelta * Random.Range(-1f, 1f), spawnOnDestroy.transform.rotation);
                go.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
            }
        }

        
        Vector3 targetPosition = target.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, 1f);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bullet"))
            {
                Debug.Log("DESTROY BULLET: " + hitCollider.gameObject.name);
                Destroy(hitCollider.gameObject);
            }
        }

        // Distruggi il target.
        DestroyMyTarget();

    }

    public void DestroyMyTarget()
    {
        Debug.Log("DESTROY TARGET: " + target.gameObject.name);
        Destroy(target);
    }
}
