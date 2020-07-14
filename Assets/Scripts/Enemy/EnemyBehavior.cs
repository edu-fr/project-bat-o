using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Scripts.Utils;

public class EnemyBehavior : MonoBehaviour
{
    #region Enums
    enum States {Standard, Chasing, Attacking};

    #endregion

    #region Variables
    // Components
    Rigidbody2D myRigidbody;
    CircleCollider2D myCircleCollider;
    Animator myAnimator;
    AIDestinationSetter myAIDestinationSetter;
    Seeker mySeeker;
    AIPath myAIPath;


    // Movement
    Path path;
    bool reachedEndOfPath;
    float currentTimer = 0f; 
    float maxTimer = 3f;                // time to move to the next random spot
    // private float speed = 160f;         // walk speed
    private Vector3 homePosition;       // original position on the level
    private float walkableRange = 1f;   // Distance it can walk while isnt chasing the player 
    public GameObject target;

   

    // Searching for player
    [SerializeField] private Transform preFabFieldOfView = null;
    private FieldOfView fieldOfView = null;
    [SerializeField] private float fieldOfViewValue = 0;
    [SerializeField] private float viewDistance = 0;
    [SerializeField] private GameObject targetPlayer = null;
    private GameObject player;
    [SerializeField] private float surroundingDistance = 2f;

    // Animation
    private Vector3 currentDirection;
    private float curAngle;
    private Vector3 faceDirection;

    // State machine
    private States state;
    private bool isWalkingAround = false;

    // Health
    private HealthManager myHealthManager;

    // Ally search
    private float searchForAlliesRange = 5f;
    private LayerMask enemiesLayer;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCircleCollider = GetComponent<CircleCollider2D>();
        myAIDestinationSetter = GetComponent<AIDestinationSetter>();
        myAIPath = GetComponent<AIPath>();
        mySeeker = GetComponent<Seeker>();
        myHealthManager = GetComponent<HealthManager>();
        enemiesLayer = LayerMask.GetMask("Enemies");
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = new GameObject("target " + gameObject.name);
        state = States.Standard;
        // Set initial enemy position according to its initial position
        homePosition = myRigidbody.position;

        // Set the first random target movement 
        target.transform.position = GenerateNewTarget();
        myAIDestinationSetter.target = target.transform;

        // Instantiate prefab field of view
        fieldOfView = Instantiate(preFabFieldOfView, null).GetComponent<FieldOfView>();
        fieldOfView.gameObject.name = "Field of view" + gameObject.name;
        fieldOfView.setFieldOfView(fieldOfViewValue);
        fieldOfView.setViewDistance(viewDistance);
        fieldOfView.setMyEnemyBehavior(this);

        // Life bar

    }

    // Update is called once per frame
    private void Update()
    {
        // Verify if its alive
        if(myHealthManager.getCurrentHP() <= 0)
        {
            Destroy(gameObject);
            Destroy(fieldOfView.gameObject);
            Destroy(target.gameObject);
        }
        else
        {
            #region State Machine
            switch (state)
            {
                default:

                case States.Standard:
                    isWalkingAround = true;
                    // Checking if player is close
                    checkSurroundings();
                    
                    // Updating field of view
                    fieldOfView.SetAimDirection(faceDirection);
                    fieldOfView.SetOrigin(transform.position);
                    // Looking for the player
                    if(targetPlayer != null)
                    {
                        changeState(States.Chasing);
                    }
                    break;
                

                case States.Chasing:
                    // Verify if there is close enemies chasing the player
                    


                    if (targetPlayer != null) // Know where the player is
                    {
                        animate();
                        setCurrentFaceDirection();

                        // Return to Standard state
                        if(Vector2.Distance(transform.position, targetPlayer.transform.position) > 7f)
                        {
                            targetPlayer = null;
                        }
                    }
                    else // Lost sight of player
                    {
                        changeState(States.Standard);
                    }
                    break;

                case States.Attacking:

                   break;

            }
            #endregion
        }
    }

    // Fixed Update its used to treat physics matters
    private void FixedUpdate()
    {
        if(isWalkingAround)
            WalkAround();
    }
    #endregion

    #region Auxiliar Methods

    private Vector3 GenerateNewTarget()
    {
        return new Vector3(homePosition.x + (walkableRange * Random.Range(-1, 2)), homePosition.y + (walkableRange * Random.Range(-1, 2)), transform.position.z);
    }
    
    private void WalkAround()
    {
        // Generate new target every maxTimer (three seconds)

        if (myAIPath.reachedEndOfPath) //if reached desired location, wait three seconds and move to another
        {
            // Animate standing still
            myAnimator.SetBool("isMoving", false);
            currentTimer += Time.deltaTime;

            if (currentTimer >= maxTimer)
            {
                currentTimer = 0;
                target.transform.position = GenerateNewTarget();
                AstarPath.active.Scan();
            }
        }
        else 
        {
            // Animate movement
            animate();
        }
        setCurrentFaceDirection();
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public void setTargetPlayer(GameObject player)
    {
        this.targetPlayer = player;
    }
   
    private void changeState(States state)
    {
        switch (state)
        {
            default:

            case (States.Standard):
                isWalkingAround = true;
                targetPlayer = null;
                myAIDestinationSetter.target = target.transform;
                fieldOfView.gameObject.SetActive(true);
                break;

            case (States.Chasing):
                isWalkingAround = false;
                fieldOfView.gameObject.SetActive(false);
                myAIDestinationSetter.target = targetPlayer.transform;
                AstarPath.active.Scan();
                break;

            case (States.Attacking):
                break;
        }
        this.state = state;
    }

    private void checkSurroundings()
    {
        if(Vector2.Distance(player.transform.position, this.transform.position) < surroundingDistance)
        {
            targetPlayer = player;
        }
    }

    private void searchForAlliesActivityNearby() // if there is an ally nearby chasing the player, the object starts to follow the player too
    {
        Collider2D[] alliesNearby = Physics2D.OverlapCircleAll(transform.position, searchForAlliesRange, enemiesLayer);
        foreach (Collider2D ally in alliesNearby)
        {
            /*
            if (myCircleCollider.Distance(ally.GetComponent<CircleCollider2D>()) < searchForAlliesRange)
            {

            }
            */
        }
    }

    private void setCurrentFaceDirection()
    {
        if (myAnimator.GetBool("isMoving")) // just want to change the facing direction if the object is walking
        {
            currentDirection = myAIPath.desiredVelocity;
        }
        curAngle = Utilities.GetAngleFromVectorFloat(currentDirection);
        // Actual set of face direction
        faceDirection = Utilities.GetDirectionFromAngle(curAngle);
    }

    private void animate()
    {
        myAnimator.SetBool("isMoving", true);
        myAnimator.SetFloat("moveX", myAIPath.velocity.x);
        myAnimator.SetFloat("moveY", myAIPath.velocity.y);
    }
    #endregion

    public void TakeDamage(float weaponDamage, float weaponKnockback, Vector3 attackDirection, float knockbackDuration)
    {
        Debug.Log("tomou dano");
        // Decrease health
        myHealthManager.HurtObject(weaponDamage);

        // Take knockback
        myAIPath.enabled = false;
        myRigidbody.AddForce(attackDirection * weaponKnockback, ForceMode2D.Impulse);
        StartCoroutine(TakeKnockback(knockbackDuration));

    }

    private IEnumerator TakeKnockback(float knockbackTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        Debug.Log("ESPEROU o tempo");
        myAIPath.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            myHealthManager.HurtObject(25f);
        }    
    }
}
