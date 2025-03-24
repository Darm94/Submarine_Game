using UnityEngine;
using System.Collections;
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
    
    // Ghost effect
    private float _ghostDuration = 12f; // Durata dell'effetto fantasma in secondi
    private Color _ghostColor = new Color(0f, 0.5f, 1f, 0.2f); // Colore azzurro trasparente
    private Color[] _originalColors;
    private float[] _originalAlphas;
    private SkinnedMeshRenderer rendererShip;
    [SerializeField] private GameObject shipMesh;
    
    //for now a solution to get the spawners and gameLimits reset
    //get the object in case we need to move them
    [SerializeField] private GameObject boxesSpawner;
    [SerializeField] private GameObject minesSpawner;
    [SerializeField] private GameObject bottomColumnsSpawner;
    [SerializeField] private GameObject topColumnsSpawner;
    [SerializeField] private GameObject powerUpSpawner;
    [SerializeField] private float gameLimitX = 275;
    [SerializeField] private float resetPositionOffset = 14.138f;
    private float _obstacleLimitX;
    private bool _isOnResetPhase = false;
    private Vector3 _startPosition;
    private ObstacleLinearPlacer _boxesSpawnerComponent ;
    private ObstacleLinearPlacer _minesSpawnerComponent ;
    private ObstacleLinearPlacer _bottomColumnsSpawnerComponent ;
    private ObstacleLinearPlacer _topColumnsSpawnerComponent ;
    private PowerUpPlacer _powerUpSpawnerComponent ;
    
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
        
        // Salva il colore e l'alpha originali del materiale.
        rendererShip = shipMesh.GetComponent<SkinnedMeshRenderer>();
        if (rendererShip != null)
        {
            int numMateriali = rendererShip.materials.Length;
            _originalColors = new Color[numMateriali];
            _originalAlphas = new float[numMateriali];

            for (int i = 0; i < numMateriali; i++)
            {
                _originalColors[i] = rendererShip.materials[i].color;
                _originalAlphas[i] = rendererShip.materials[i].color.a;
            }
        }
        else
        {
            Debug.LogError("L'oggetto 'shipMesh' non ha uno Skinned Mesh Renderer!");
        }
        
        //Essential part for the spawner Coordination at X position gameLimit and for Gameover,we need to get this components    
        _boxesSpawnerComponent = boxesSpawner.GetComponent<ObstacleLinearPlacer>();
        _minesSpawnerComponent = minesSpawner.GetComponent<ObstacleLinearPlacer>();
        _bottomColumnsSpawnerComponent = bottomColumnsSpawner.GetComponent<ObstacleLinearPlacer>();
        _topColumnsSpawnerComponent = topColumnsSpawner.GetComponent<ObstacleLinearPlacer>();
        _powerUpSpawnerComponent = powerUpSpawner.GetComponent<PowerUpPlacer>();
        _obstacleLimitX = gameLimitX - 45; // this magic number is calculated to not show objects disappearing
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
            transform.position = new Vector3(_startPosition.x-resetPositionOffset , transform.position.y, transform.position.z);//30 magic number could be resetPositionOffset
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
            //other.gameObject.SetActive(false);
            //other.gameObject.transform.position = Vector3.zero;
            fuel = Mathf.Clamp(fuel + boxFuelAddition, 0, maxFuel);
            Debug.Log($"Fuel gained: {fuel}");
        }
        else if (other.gameObject.CompareTag("Mine"))
        {
            Destroy(other.gameObject);
            //other.gameObject.SetActive(false);
            //other.gameObject.transform.position = Vector3.zero;
            fuel = Mathf.Clamp(fuel - mineFuelReduction, 0, maxFuel);
            Debug.Log($"Fuel lost: {fuel}");
            if (fuel <= 0)
            {
                enabled = false;
            }
        }
        else if (other.gameObject.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject);
            StartCoroutine(CoroutineGhostEffect());
        }
    }

    IEnumerator CoroutineGhostEffect()
    {
        if (!rendererShip) yield break; // Esci se non c'è il renderer
        Debug.Log("GHOST EFFECT ACTIVATED");
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        shipMesh.layer = LayerMask.NameToLayer("Ghost");

        int numMateriali = rendererShip.materials.Length;
        Color[] coloriFantasmaConAlpha = new Color[numMateriali];

        // Inizia con l'alpha a 0 per tutti i materiali.
        for (int i = 0; i < numMateriali; i++)
        {
            coloriFantasmaConAlpha[i] = _ghostColor;
            coloriFantasmaConAlpha[i].a = 0f; // Inizia con alpha 0
        }

        float timer = 0f;
        while (timer < _ghostDuration)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < numMateriali; i++)
            {
                float alphaCorrente = Mathf.Lerp(0f, _originalAlphas[i], timer / _ghostDuration); // Inverti Lerp
                Color coloreCorrente = _ghostColor;
                coloreCorrente.a = alphaCorrente;
                rendererShip.materials[i].color = coloreCorrente;
            }
            yield return null;
        }

        // Reset dei materiali ai valori originali.
        for (int i = 0; i < numMateriali; i++)
        {
            rendererShip.materials[i].color = _originalColors[i];
        }
        Debug.Log("GHOST EFFECT EEEND");
        gameObject.layer = LayerMask.NameToLayer("Player");
        shipMesh.layer = LayerMask.NameToLayer("Player");
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
        _powerUpSpawnerComponent.enabled = false;
        
        //Destroy boxes and Mine after the limit
        Collider[] collidersTrovati = Physics.OverlapSphere(transform.position, 50);
        foreach (Collider collider in collidersTrovati)
        {
            if ((collider.CompareTag("Mine") || collider.CompareTag("Box")) && collider.transform.position.x > _obstacleLimitX + 20)
            {
                //max for 2 element for once,i guess that's sustaneable
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
        _powerUpSpawnerComponent.enabled = true;
    }
    
    
}
