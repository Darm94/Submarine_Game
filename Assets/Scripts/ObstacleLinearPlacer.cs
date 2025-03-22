using UnityEngine;

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
        [SerializeField] [Range(0.5f, 30f)] private float destroyDelay = 10;
    
        Vector3 _startPosition;
        private bool _started = false;
    
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
            
            _currentPosition = new Vector3(obstacle.transform.position.x, _startPosition.y, obstacle.transform.position.z);
    
            Destroy(obstacle, destroyDelay);
        }
}
