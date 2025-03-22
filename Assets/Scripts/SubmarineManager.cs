using UnityEngine;

public class SubmarineManager : MonoBehaviour
{
    [SerializeField] float fuel = 100f;
    [SerializeField] float maxFuel = 100f;

    [SerializeField] float fuelUsageSpeed = 1f;
    [SerializeField] float mineFuelReduction = 5f;
    [SerializeField] private float boxFuelAddition = 6;
    
    [SerializeField] Vector3 impulseForce = Vector3.up * 10;
    [SerializeField] Vector3 constantForce = Vector3.up * 20;
    [SerializeField] Vector3 forwardForce = Vector3.right * 20;

    [SerializeField] ForceMode forceMode = ForceMode.Force;
    
    //for now a solution to get the spawners and gameLimits reset
    [SerializeField] private GameObject boxesSpawner;
    [SerializeField] private GameObject minesSpawner;
    [SerializeField] private GameObject bottomColumnsSpawner;
    [SerializeField] private GameObject topColumnsSpawner;
    [SerializeField] private float gameLimitX = 290;
    private float _obstacleLimitX;
    private bool _isOnResetPhase = false;
    private Vector3 _startPosition;
    private ObstacleLinearPlacer _boxesSpawnerComponent ;
    private ObstacleLinearPlacer _minesSpawnerComponent ;
    private ObstacleLinearPlacer _bottomColumnsSpawnerComponent ;
    private ObstacleLinearPlacer _topColumnsSpawnerComponent ;
    
    bool _thrust;
    Rigidbody rb;

    [SerializeField]
    float minRotation = 35;

    [SerializeField]
    float maxRotation = -35;

    [SerializeField]
    float pitchSpeed = 1;

    [SerializeField]
    float speed = 1;

    [SerializeField]
    Transform ship;

    private bool resetted = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        
    //Essential part for the spawner Coordination at X position gameLimit and for Gameover,we need to get this components    
    _boxesSpawnerComponent = boxesSpawner.GetComponent<ObstacleLinearPlacer>();
    _minesSpawnerComponent = minesSpawner.GetComponent<ObstacleLinearPlacer>();
    _bottomColumnsSpawnerComponent = bottomColumnsSpawner.GetComponent<ObstacleLinearPlacer>();
    _topColumnsSpawnerComponent = topColumnsSpawner.GetComponent<ObstacleLinearPlacer>();
    _obstacleLimitX = gameLimitX - 40;
    }

    // Update is called once per frame
    void Update()
    {
        fuel -= Time.deltaTime * fuelUsageSpeed;

        if (fuel <= 0)
        {
            enabled = false;
        }


        //Begin to Stop obstacle spawn
        if (transform.position.x >= _obstacleLimitX)
        {
            DisableAllSpawners();
        }
        //Teleport at start And Reset Player position
        if (transform.position.x >= gameLimitX)
        {
            transform.position = new Vector3(_startPosition.x-20 , transform.position.y, transform.position.z);
            _isOnResetPhase = true;
        }
        //Enable again and Reset all spawners
        if (_isOnResetPhase && transform.position.x >= _startPosition.x)
        {
            _isOnResetPhase = false;
            EnableAllSpawners();
            _boxesSpawnerComponent.ManualPositionReset();
            _minesSpawnerComponent.ManualPositionReset();
            _bottomColumnsSpawnerComponent.ManualPositionReset();
            _topColumnsSpawnerComponent.ManualPositionReset();
        }
        

        if (Input.GetButtonDown("Jump"))
        {
            _thrust = true;
            resetted = false;
        }
        else if (Input.GetButton("Jump"))
        {
            Vector3 dest = new Vector3(maxRotation, ship.transform.localRotation.eulerAngles.y, ship.transform.localRotation.eulerAngles.z);
            ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.Euler(dest), Time.deltaTime * pitchSpeed);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            _thrust = false;
        }
        else
        {
            Vector3 dest = new Vector3(minRotation, ship.transform.localRotation.eulerAngles.y, ship.transform.localRotation.eulerAngles.z);
            ship.transform.localRotation = Quaternion.Lerp(ship.transform.localRotation, Quaternion.Euler(dest), Time.deltaTime * pitchSpeed);
        }
    }
    
    private void FixedUpdate()
    {
        if (_thrust)
        {
            if (forceMode == ForceMode.Impulse)
            {
                _thrust = false;
                resetted = true;
                rb.AddForce(impulseForce, forceMode);
                ship.transform.localEulerAngles = new Vector3(maxRotation, ship.transform.localRotation.eulerAngles.y, ship.transform.localRotation.eulerAngles.z);
            }
            else
            {
                rb.AddForce(constantForce, forceMode);
            }
        }
        rb.AddForce(forwardForce, forceMode);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger by: {other.gameObject}", other.gameObject);

        if (other.gameObject.CompareTag("Box"))
        {
            Destroy(other.gameObject);
            fuel = Mathf.Clamp(fuel + boxFuelAddition, 0, maxFuel);
            Debug.Log($"Fuel gained: {fuel}");
        }
        else if (other.gameObject.CompareTag("Mine"))
        {
            Destroy(other.gameObject);
            fuel = Mathf.Clamp(fuel - mineFuelReduction, 0, maxFuel);
            Debug.Log($"Fuel lost: {fuel}");
            if (fuel <= 0)
            {
                enabled = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        
        rb.isKinematic = true;
        enabled = false;
        //Our GAmeOver method start from here
        GameOver();
    }

    private void GameOver()
    {
        DisableAllSpawners();
    }

    private void DisableAllSpawners()
    {
        _boxesSpawnerComponent.enabled = false;
        _minesSpawnerComponent.enabled = false;
        _bottomColumnsSpawnerComponent.enabled = false;
        _topColumnsSpawnerComponent.enabled = false;
        
        //Destroy boxes and Mine after the limit
        Collider[] collidersTrovati = Physics.OverlapSphere(transform.position, 50);
        foreach (Collider collider in collidersTrovati)
        {
            if ((collider.CompareTag("Mine") || collider.CompareTag("Box")) && collider.transform.position.x > _obstacleLimitX + 10)
            {
                //max for 2 element for one,i guess that's sustaneable
                collider.gameObject.GetComponent<DestroyOnBulletTrigger>().DestroyMyTarget();
            }
        }
        
    }

    private void EnableAllSpawners()
    {
        _boxesSpawnerComponent.enabled = true;
        _minesSpawnerComponent.enabled = true;
        _bottomColumnsSpawnerComponent.enabled = true;
        _topColumnsSpawnerComponent.enabled = true;
    }
    
    
}
