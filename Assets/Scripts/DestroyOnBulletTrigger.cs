using UnityEngine;

public class DestroyOnBulletTrigger : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject spawnOnDestroy;
    [SerializeField] [Range(1,10)] private int maxInstances = 5;
    [SerializeField] private Vector3 randomDelta = Vector3.zero;
    private ObstacleLinearPlacer respawnManager;

    
    private void Start()
    {
        if (target != null && target.transform.parent != null)
        {
            GameObject father = target.transform.parent.gameObject;
            Debug.Log("The OBJ father  " + target.name + " is: " + father.name);
            respawnManager = father.GetComponent<ObstacleLinearPlacer>();
        }
        else
        {
            Debug.Log(target.name + " obj has no father.");
        }
    }
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
        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, 5f);

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
        if (target.CompareTag("Mine"))
        {
            //Debug.Log("MINE:  TARGET DESTROYED");
            Destroy(target);
            Debug.Log("MINE:  TARGET REMOVED");
            //GetComponent<SphereCollider>().enabled = false;
            //respawnManager.SetAvailableObject(target);
        }
        else if (target.CompareTag("Box"))
        {
            Destroy(target);
            Debug.Log("BOX:  TARGET REMOVED");
            //respawnManager.SetAvailableObject(target);
        }
        else
        {
            Debug.Log("OBjeCt DesTroyeD");
            Destroy(target);
        }
        
        //target.transform.position = Vector3.zero;
        //target.SetActive(false);
        
        
        Debug.Log("Managing the object:  " + target.gameObject.name);
        //Destroy(target);
    }
}
