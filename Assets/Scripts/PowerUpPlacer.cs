using UnityEngine;

public class PowerUpPlacer : MonoBehaviour
{
        [SerializeField] GameObject[] obstaclePrefabs;
        [SerializeField] [Range(0, 5)] private float randomVerticalDisplacement = 1;

        
        private Vector3 _startPosition;
        private bool _started = false;
    
        void Start() {
            _startPosition = transform.position;
            PlacePowerUp();
            _started = true;
        }
        
        void OnEnable()
        {
            if (_started)
            {
                PlacePowerUp();
            }
            
        }
    
        void PlacePowerUp() {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
    
            GameObject obstacle = Instantiate(prefab,
                _startPosition +
                new Vector3(Random.Range(30,200), Random.Range(0, randomVerticalDisplacement), 0),
                prefab.transform.rotation,
                transform);
            
            Destroy(obstacle, 65f);//chrono time for one MainPlane round : 65 sec
        }
}
