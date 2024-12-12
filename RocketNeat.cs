using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketNeat : MonoBehaviour {
    [SerializeField]
    public Genome Brain;
    [HideInInspector]
    public float LifeTime;
    [HideInInspector]
    public NeatShip MotherShip;
    [HideInInspector]
    public float TurnSpeed = 10f;
    [HideInInspector]
    public float ObstacleDistance = 5f;
    [HideInInspector]
    public float Speed = 5;
    private Rigidbody2D myRB;
    private float[] Features;
    private Vector2[] Directions = new Vector2[] { Vector2.left + Vector2.up, Vector2.up, Vector2.up - Vector2.left };
    [HideInInspector]
    public float Fitness;
    [HideInInspector]
    public bool Dead;
    [HideInInspector]
    public float MutationRate = 0.1f;
    private float MaxRight;
    private float currentLifeTime;
    [HideInInspector]
    public Color color;
    
    //[1, Left, Forward, Right]

    // Use this for initialization
    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        Features = new float[] { 0, 0, 0, 0};
        GetComponent<SpriteRenderer>().color = color;

    }
    


    public void CreateNewBrain(int InputCount, int OutputCount)
    {
        Brain = new Genome(InputCount, OutputCount);
        Brain.Mutate();
    }

    public void CreateNewBrain(Genome P1, Genome P2)
    {
        Brain = new Genome(P1, P2);
        Brain.Mutate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Dead)
            return;
        float Dist = ObstacleDistance;

        RaycastHit2D RH = Physics2D.Raycast(transform.position, transform.up - transform.right, ObstacleDistance, 1 << LayerMask.NameToLayer("Obstacle"));
        if (RH.collider != null)
            Dist = RH.distance;

        // Set Left Distance
        Features[0] = (ObstacleDistance - Dist) / ObstacleDistance;

        Dist = ObstacleDistance;
        RH = Physics2D.Raycast(transform.position, transform.up, ObstacleDistance, 1 << LayerMask.NameToLayer("Obstacle"));
        if (RH.collider != null)
            Dist = RH.distance;
            
        // Set Forward Distance
        Features[1] = (ObstacleDistance - Dist) / ObstacleDistance;

        Dist = ObstacleDistance;
        RH = Physics2D.Raycast(transform.position, transform.up + transform.right, ObstacleDistance, 1 << LayerMask.NameToLayer("Obstacle"));
        if (RH.collider != null)
            Dist = RH.distance;
            
        // Set Right Distance
        Features[2] = (ObstacleDistance - Dist) / ObstacleDistance;

        // Set Angle Difference
        Features[3] = Vector2.SignedAngle(Vector2.right, transform.up) / 180;
        
        
        // Feed Forward
        float[] output = Brain.FeedForward(Features);

        float turnLeft = Sigmoid(output[0]);
        float turnRight = Sigmoid(output[1]);
        if (turnLeft > 0.5f && turnLeft > turnRight)
        {
            TurnLeft();
        }
        else if (turnRight > 0.5f)
        {
            TurnRight();
        }

        myRB.velocity = transform.up * 5;

        if (transform.position.x > MaxRight)
        {
            MaxRight = transform.position.x;
            currentLifeTime = 0;
        }


        currentLifeTime += Time.fixedDeltaTime;
        if (currentLifeTime > LifeTime)
            Die();

        if (Input.GetKeyDown(KeyCode.K))
            Die();
    }

    public float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-2f * x));
    }
    

    public void TurnLeft()
    {
        myRB.MoveRotation(myRB.rotation - TurnSpeed * Time.fixedDeltaTime);
    }

    public void TurnRight()
    {
        myRB.MoveRotation(myRB.rotation + TurnSpeed * Time.fixedDeltaTime);
    }

    public void Die()
    {
        myRB.velocity = Vector2.zero;
        myRB.angularVelocity = 0;
        CalculateFitness();
        Dead = true;
        MotherShip.CheckAllDead();
    }

    public void CalculateFitness()
    {
        Fitness = transform.position.x;

        Fitness = Mathf.Max(0, Fitness);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Dead)
            return;
        if (collision.gameObject.layer == 8)
        {
            Die();
        }
    }
}
