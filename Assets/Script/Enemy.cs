using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Linq;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject weaponFlash;
    public float bloom;
    public float fireRate;
    private float lastShotTime = 0f;

    public Material hitMat;

    private Rigidbody rb;

    private Renderer rend;
    private Material originalMaterial;

    public AudioClip shootingSFX;
    //AI Settings
    public int currentPointIndex = 0;
    public Vector3 currentTarget;
    public float positionThreshold;
    public float idleTime = 5f;
    public float attackDistance = 5f;
    public float maxVisionDistance = 20f;
    public float minChasingHealth = 30f;

    public Transform[] patrolPoints;
    private float idleTimeCounter;
    private Transform playerTransform;
    private bool canSeePlayer;
    private Vector3 lastKnownPlayerPosition;

    private NavMeshAgent agent;

    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State state = State.Idle;

    private LayerMask visionMask; // Criamos uma variável para guardar a máscara otimizada

    void Start()
    {

        if (Time.timeScale == 0f) return;
        
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;

        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        GameObject patrolPointParent = GameObject.FindWithTag("PatrolPoint");

        if (patrolPointParent != null)
        {

            patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>()
                .Where(t => t != patrolPointParent.transform)
                .ToArray();


            if (patrolPoints.Length > 0)
            {
                currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
            }
        }
        else
        {
            Debug.LogError("Objeto com a tag 'PatrolPoint' não foi encontrado na cena!");
        }
        
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer != -1) // Garante que a layer existe
        {
            visionMask = ~(1 << enemyLayer); 
        }
        else
        {
            Debug.LogError("Layer 'Enemy' não encontrada! Certifique-se de criá-la na Unity.");
            visionMask = Physics.DefaultRaycastLayers; // Fallback de segurança
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        // Se colidir com um tiro mas o objeto que tocou for outro inimigo, ignora
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) return;

        if(collision.gameObject.tag == "Damage")
        {
            health -= 10;

            if(health <= 0)
            {
                Die();
            } 
            else
            {
                StartCoroutine(Blink());
            }
        }
    }

    
    void Die()
    {

        UIManager.Instance.quantidadeInimigosDerrotados += 1;
        UIManager.Instance.inimigosDerrotadosText.text = "Inimigos Derrotados: " + UIManager.Instance.quantidadeInimigosDerrotados;
        Destroy(gameObject);
    }

    IEnumerator Blink()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = originalMaterial;
    }

    private void Update() {

        if (Time.timeScale == 0f) return;
        
        LookForPlayer();

        switch(state) {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrolling();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Chasing:
                Chasing();
                break;
        }

        rb.linearVelocity = Vector3.zero;

        // CORREÇÃO: Só atualizar rotação e última posição se ele realmente souber onde o player está de fato
        if (canSeePlayer)
        {
            LookAtPlayer();
            SetLastKnownPlayerPosition();
        }
    }

    private void LookForPlayer()
    {
        if (playerTransform == null) return;

        // Origem do raio (centro do inimigo)
        Vector3 rayOrigin = transform.position + Vector3.up * 1f; 
        Vector3 directionToPlayer = playerTransform.position - rayOrigin;

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int layerMask = ~(1 << enemyLayer); 

    
        if(Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, maxVisionDistance, layerMask))
        {
            canSeePlayer = hit.transform.CompareTag("Player") || hit.transform.root.CompareTag("Player");

            if(canSeePlayer && state != State.Attacking)
            {
                state = State.Chasing;
            }
        }
        else
        {
            canSeePlayer = false; 
        }
    }

    private void Idle()
    {
        agent.ResetPath();

        idleTimeCounter -= Time.deltaTime;

        if(idleTimeCounter < 0)
        {
            state = State.Patrolling;
            idleTimeCounter = idleTime;
        }
    }

    private void Patrolling()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (Vector3.Distance(currentTarget, transform.position) < positionThreshold)
        {
            float chance = Random.Range(0, 100);

            if (chance < 10)
            {
                state = State.Idle;
                return;
            }

            currentPointIndex++;
            currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
        }
        
        agent.SetDestination(currentTarget);
    }

    public void Attacking() {
        idleTimeCounter = idleTime;
        agent.ResetPath();

        Shoot();

        if(Vector3.Distance(transform.position, playerTransform.position) > attackDistance || !canSeePlayer) {
            if(health < minChasingHealth) {
                state = State.Patrolling;
            } else {
                state = State.Chasing;
            }
        }
    }

    private void Chasing()
    {
        idleTimeCounter = idleTime;
        agent.SetDestination(lastKnownPlayerPosition);

        // CORREÇÃO: Se ele perdeu a visão E chegou na última posição conhecida, volta a patrulhar
        float distanceToLastPos = Vector3.Distance(transform.position, lastKnownPlayerPosition);

        if(health < minChasingHealth)
        {
            state = State.Patrolling;
        }
        else if(Vector3.Distance(transform.position, playerTransform.position) <= attackDistance && canSeePlayer)
        {
            state = State.Attacking;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) > maxVisionDistance)
        {
            state = State.Patrolling;
        }
        else if (distanceToLastPos < positionThreshold && !canSeePlayer)
        {
            state = State.Patrolling;
        }
    }

    private void LookAtPlayer()
    {
        if (canSeePlayer)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }
    }

    private void SetLastKnownPlayerPosition()
    {
        if (canSeePlayer)
        {
            lastKnownPlayerPosition = playerTransform.position;
        }
    }

    private void Shoot()
    {
        if(Time.time > lastShotTime + fireRate)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.Normalize();

            Quaternion bulletRotation = Quaternion.LookRotation(directionToPlayer);

            float maxInaccuracy = 10f;
            float currentInaccuracy = bloom * maxInaccuracy;
            float randomYaw = Random.Range(-currentInaccuracy, currentInaccuracy);
            float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

            bulletRotation *= Quaternion.Euler(randomPitch, randomYaw + 90f, 0f);

            AudioManager.Instance.PlaySFX(shootingSFX, 0.25f);

            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
            Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            lastShotTime = Time.time;
        }
    }
}