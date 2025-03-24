using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ObstacleLinearPlacer : MonoBehaviour
{
     [SerializeField] GameObject[] obstaclePrefabs;
        [SerializeField] [Range(0, 5)] private float randomVerticalDisplacement = 1;
        [SerializeField] [Range(0, 30)] private float distance = 5;
        [SerializeField] private Vector3 displacement = Vector3.zero;
        [SerializeField] private Vector3 movementDirection = Vector3.right;
        private Vector3 _currentPosition = Vector3.zero;
    
        [SerializeField] [Range(0.5f, 10f)] private float repeatingRatio = 1;
        [SerializeField] [Range(0.5f, 10f)] private float startDelay = 1;
        [SerializeField] [Range(0.5f, 30f)] private float destroyDelay = 15;
    
        Vector3 _startPosition;
        private bool _started = false;
        private List<GameObject> obstaclePool = new List<GameObject>();
        
        void Start() {
            _startPosition = transform.position;
            _currentPosition = _startPosition;
            InvokeRepeating(nameof(PlaceObstacles), startDelay, repeatingRatio);
            _started = true;
        }

        public void ManualPositionReset()
        {
            _currentPosition = _startPosition;
        }

        void OnDisable()
        {
            CancelInvoke(nameof(PlaceObstacles));
        }

        void OnEnable()
        {
            if (_started)
            {
                InvokeRepeating(nameof(PlaceObstacles), startDelay, repeatingRatio);
            }
            
        }
    
        void PlaceObstacles() {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
    
            GameObject obstacle = Instantiate(prefab,
                _currentPosition + distance * movementDirection + displacement + 
                new Vector3(0, Random.Range(0, randomVerticalDisplacement), 0),
                prefab.transform.rotation,
                transform);
            
            
            /*
           GameObject obstacle = GetPooledObject(prefab);
           obstacle.SetActive(true);
           obstacle.transform.position = _currentPosition + distance * movementDirection + displacement +
                                         new Vector3(0, Random.Range(0, randomVerticalDisplacement), 0);
           obstacle.transform.rotation = prefab.transform.rotation;
           ;
              */
            
           if (obstacle.name == "Mine" || obstacle.name == "Mine(Clone)")
           {
               /*
               Debug.Log(obstacle.name);
               Debug.Log(obstacle.transform.position.y);
               Debug.Log(_startPosition.y + randomVerticalDisplacement);
               Debug.Log("--------- ^^^^^^^ --------"); */
                if (obstacle.transform.position.y >= _startPosition.y + 1)
                {
                    obstacle.transform.rotation *= Quaternion.Euler(180, 0, 0);
                    //Debug.Log("--------- ROTATED ^^^ --------");
                }
                
            }

            
            _currentPosition = new Vector3(obstacle.transform.position.x, _startPosition.y, _startPosition.z);
            Debug.Log("Cuttent " + obstacle.name +"swaner position is :" + _currentPosition);
            Destroy(obstacle, destroyDelay);
            //StartCoroutine(DisableAfterTime(obstacle, destroyDelay));
        }
        
        private GameObject GetPooledObject(GameObject prefab)
        {
            foreach (var obj in obstaclePool)
            {
                if (!obj.activeInHierarchy)
                {
                    
                    return obj;
                }
            }

            // Se non ci sono oggetti disponibili, ne creiamo uno nuovo e lo aggiungiamo al pool
            GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            newObj.SetActive(false);
            obstaclePool.Add(newObj);
            return newObj;
        }

        private IEnumerator DisableAfterTime(GameObject obstacle, float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("DISABLE Time: " + obstacle.gameObject.name);
            obstacle.SetActive(false);
            obstacle.transform.position = Vector3.zero; // Spostiamo l'ostacolo in una posizione sicura
        }
        

}
