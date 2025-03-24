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
        private List<GameObject> availablePool = new List<GameObject>();
        
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
    
            /*GameObject obstacle = Instantiate(prefab,
                _currentPosition + distance * movementDirection + displacement +
                new Vector3(0, Random.Range(0, randomVerticalDisplacement), 0),
                prefab.transform.rotation,
                transform);
    */

           /* 
           GameObject obstacle = GetPooledObject(prefab);
           obstacle.SetActive(true);
           Vector3 newPosition = _currentPosition + distance * movementDirection + displacement + 
                                 new Vector3(0, Random.Range(0, randomVerticalDisplacement), 0);
    
           // Avvia Coroutine per aspettare 1 secondo e poi aggiornare la posizione
           obstacle.transform.position = newPosition;
           obstacle.transform.rotation = prefab.transform.rotation;
           ;
              */
           GameObject obstacle = GetAvailableObject(prefab);
           availablePool.Remove(obstacle);//QUA LO IMPOSTO E' IMPOSSIBILE CHE LO VADA A SPOSTARE SE L'HO TOLTO DA QUA
           //obstacle.SetActive(true);
           Vector3 newPosition = _currentPosition + distance * movementDirection + displacement + 
                                 new Vector3(0, Random.Range(0, randomVerticalDisplacement), 0);
    
           
           obstacle.transform.position = newPosition;
           foreach (Transform child in obstacle.transform)
           {
               child.position = newPosition + child.localPosition;
           }
           obstacle.transform.rotation = prefab.transform.rotation;
            
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
            //Destroy(obstacle, destroyDelay);
            //StartCoroutine(DisableAfterTime(obstacle, destroyDelay));
            
            
            StartCoroutine(MoveAndAddToAvailableAfterTime(obstacle, destroyDelay));
        }

        
        private GameObject GetAvailableObject(GameObject prefab)
        {
            foreach (GameObject obj in availablePool)
            {
                obj.GetComponent<TimerResetter>().ToBeResetted = false; //Beggining from now the object timer can be restarted
                //availablePool.Remove(obj);
                return obj;
            }
            
            
            // If no objects available ,create a new one
            GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            newObj.AddComponent<TimerResetter>();
            //obstaclePool.Add(newObj);
            return newObj;
        }
        
        private IEnumerator MoveAndAddToAvailableAfterTime(GameObject obstacle, float delay)
        {
            
            
            yield return new WaitForSeconds(delay);
            if (!obstacle) yield break;//if destroy or reallocated for any reason (usato per le mine che ancora si distruggono e le scatole che si ricollocano)
            TimerResetter timerResetter = obstacle.GetComponent<TimerResetter>();
            if (timerResetter.ToBeResetted)
            {
                //timerResetter.ToBeResetted = false;
                Debug.Log(obstacle.name + " Has been resetted CANT REMOVE IT");
                yield break;
            }
            else
            {
                Debug.Log(obstacle.name + " Has NOT BEEN RESETTED time to REMOVE IT");
            }
              
            Debug.Log("DISABLE Time: " + obstacle.gameObject.name);
            if (obstacle.name == "Mine" || obstacle.name == "Mine(Clone)")
            {
                
                //Debug.Log("SPOSTO LA MINA ALLA POSIZIONE ZEROOOOOOO/partenza");
                //obstacle.transform.localPosition = new Vector3(0, -50, 0);
                //obstacle.transform.position = _startPosition;
                //availablePool.Add(obstacle);
                Debug.Log("DESTROY MINE FOR TIMEOUT");
                Destroy(obstacle);
            }
            else
            {
                SetAvailableObject(obstacle);
            }
            
        }

        public void SetAvailableObject(GameObject oldObj)
        {
            Vector3 newPosition = new Vector3(0,-50,0); // Move the  obstacle in a safe zone (To change with DISABLE)
            oldObj.transform.position = newPosition;
            foreach (Transform child in oldObj.transform)
            {
                child.position = newPosition + child.localPosition;
            }
            oldObj.GetComponent<TimerResetter>().ToBeResetted = true;
            //availablePool.Add(oldObj);
            
        }
      
        
        
        
        
        
        
        
        
        
        
        
        private void MoveChildrenRecursively(Transform parent, Vector3 newPosition)
        {
            foreach (Transform child in parent)
            {
                // Calculate the new position of the child.
                child.position = newPosition + child.localPosition;

                // Recursively move the children of the child.
                MoveChildrenRecursively(child, newPosition);
            }
        }
        
        
        private GameObject GetPooledObject(GameObject prefab)
        {
            
            foreach (var obj in obstaclePool)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // Se non ci sono oggetti disponibili, ne creiamo uno nuovo e lo aggiungiamo al pool
            GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            
            //exclude mines from pooling
            //if (!newObj.CompareTag("Mine"))
            //{
                obstaclePool.Add(newObj);
            //}
            
            return newObj;
        }

        
        private IEnumerator DisableAfterTime(GameObject obstacle, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!obstacle) yield break;//se è già stato Distrutto per qualche motivo salta (usato per le mine che ancora si distruggono)
            
              
            Debug.Log("DISABLE Time: " + obstacle.gameObject.name);
            if (obstacle.name == "Mine" || obstacle.name == "Mine(Clone)")
            {
                
                Debug.Log("SPOSTO LA MINA ALLA POSIZIONE ZEROOOOOOO/partenza");
                //obstacle.transform.localPosition = new Vector3(0, -50, 0);
                //obstacle.transform.position = _startPosition;
                //availablePool.Add(obstacle);
                Destroy(obstacle);
            }
            else
            {
                Vector3 newPosition = new Vector3(0,-50,0); // move in a secure area
                obstacle.transform.position = newPosition;
                foreach (Transform child in obstacle.transform)
                {
                    child.position = newPosition + child.localPosition;
                }
                availablePool.Add(obstacle);
                obstacle.SetActive(false);
            }
            
        }
        
        private IEnumerator SetPositionAfterActivation(GameObject obstacle, Vector3 newPosition)
        {
            // Aspetta 1 frame
            yield return null; 
    
            
            obstacle.transform.position = newPosition;
        }
        
}
