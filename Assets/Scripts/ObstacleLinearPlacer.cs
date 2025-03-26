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
        [SerializeField] [Range(0.5f, 60f)] private float destroyDelay = 30;
        private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
        
        Vector3 _startPosition;
        private bool _started;
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
                ResetPool();
                StopAllCoroutines();
            }
        }
    
        void PlaceObstacles() { 
            
           GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]; 
           //GET FROM POOL if exist
           GameObject obstacle = GetAvailableObject(prefab);
           //availablePool.Remove(obstacle);//QUA LO IMPOSTO E' IMPOSSIBILE CHE LO VADA A SPOSTARE SE L'HO TOLTO DA QUA
           
           
           //POSITION AND ROTATIONS
           Vector3 newPosition = _currentPosition + distance * movementDirection + displacement + 
                                 new Vector3(0, Random.Range(0, randomVerticalDisplacement), 0);
           obstacle.transform.position = newPosition;
           
           //forboxes
           foreach (Transform child in obstacle.transform)
           {
               child.position = newPosition + child.localPosition;
           }
           obstacle.transform.rotation = prefab.transform.rotation;
           
           if (obstacle.name == "Mine" || obstacle.name == "Mine(Clone)")
           {
               //Rotating basing on Y
                if (obstacle.transform.position.y >= _startPosition.y + 1)
                {
                    obstacle.transform.rotation *= Quaternion.Euler(180, 0, 0);
                }
            }
           
            //RESET OBSTACLE
           obstacle.SetActive(true); //riattiva componenti di spostamento e rotazione
           obstacle.GetComponent<TimerResetter>().ToBeResetted = false; // Ora può essere riutilizzato
 
            
            _currentPosition = new Vector3(obstacle.transform.position.x, _startPosition.y, _startPosition.z);
            //Debug.Log("Cuttent " + obstacle.name +"swaner position is :" + _currentPosition);
            
            
            if(obstacle.name == "Box(Clone)")
                Debug.Log("START TIMEOUT COROUTINE , TobeResetted is= "+ obstacle.GetComponent<TimerResetter>().ToBeResetted);
            //StartCoroutine(MoveAndAddToAvailableAfterTime(obstacle, destroyDelay));
            if (activeCoroutines.ContainsKey(obstacle))
            {
                StopCoroutine(activeCoroutines[obstacle]); // Ferma la vecchia coroutine
                activeCoroutines.Remove(obstacle);
            }

            Coroutine newCoroutine = StartCoroutine(MoveAndAddToAvailableAfterTime(obstacle, destroyDelay));
            activeCoroutines[obstacle] = newCoroutine;
        }

        
        private GameObject GetAvailableObject(GameObject prefab)
        {
            string prefabTag = prefab.tag; // Ottieni il tag del prefab che vuoi
            for (int i = 0; i < availablePool.Count; i++)
            {
                GameObject obj = availablePool[i];
            
                // Se l'oggetto ha il tag corretto, usalo
                if (obj.CompareTag(prefabTag) && !obj.activeInHierarchy )
                {
                    availablePool.RemoveAt(i); // Rimuovilo prima di restituirlo
                    obj.GetComponent<TimerResetter>().ToBeResetted = false;
                    Debug.Log($"Recycling a {prefabTag}");
                    return obj;
                }
            }
            
            
            // Se nessun oggetto disponibile, creane uno nuovo
            GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            newObj.AddComponent<TimerResetter>();
            return newObj;
        }
        
        private IEnumerator MoveAndAddToAvailableAfterTime(GameObject obstacle, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!obstacle) yield break;//if destroy or reallocated for any reason (usato per le mine che ancora si distruggono e le scatole che si ricollocano)
            TimerResetter timerResetter = obstacle.GetComponent<TimerResetter>();
            if (timerResetter.ToBeResetted)
            {
                if(obstacle.name == "Box(Clone)")
                    Debug.Log(obstacle.name + " Has been resetted CANT REMOVE IT");
                yield break;
            }
            else
            {
                if(obstacle.name == "Box(Clone)")
                    Debug.Log(obstacle.name + " Has NOT BEEN RESETTED time to REMOVE IT");
            }
              
            
            if (obstacle.name == "Box" || obstacle.name == "Box(Clone)")
                Debug.Log("------DISABLING obj : " + obstacle.gameObject.name);
            
            
            SetAvailableObject(obstacle);
        }

        public void SetAvailableObject(GameObject oldObj)
        {
            
            if (activeCoroutines.ContainsKey(oldObj))
            {
                StopCoroutine(activeCoroutines[oldObj]); // Interrompe la coroutine in atto
                activeCoroutines.Remove(oldObj);
            }
            
            oldObj.SetActive(false);
            oldObj.GetComponent<TimerResetter>().ToBeResetted = true;
            
            
            
            if(oldObj.name == "Box(Clone)")
                Debug.Log("To Be Resetted? :" + oldObj.GetComponent<TimerResetter>().ToBeResetted);
            
            availablePool.Add(oldObj);
            
        }

        public void ResetPool()
        {
            
            //Eventualy implement e queue for elements not available and reset this objects
            //for now i calculated a specific time to be sure that all object goes in timeout disablig coroutine
            //foreach (GameObject obj in availablePool)
            //{
                //obj.SetActive(false);
                //Destroy(obj);
            //}
            //availablePool.Clear();
        }
        
}
