using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : RayCastController{

    public CollisionInfo collisions;
   
    float maxClimbAngle = 80;
    float maxDescendAngle = 80;

    [HideInInspector]
    public Vector2 playerInput;

    PlayerController _playerController;

    public override void Start()
    {
        base.Start();
        collisions.faceDirection = 1;
        
        _playerController = GetComponent<PlayerController>();
    }

    public void Move(Vector3 velocity, bool standinonPlataform)
    {
        Move(velocity, Vector2.zero, standinonPlataform);
    }

    //Controls player collision checking
    public void Move(Vector3 velocity,Vector2 input,bool standinonPlataform = false)
    {
        UpdateRayCastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;
        playerInput = input;

        if (velocity.x != 0)
        {
            collisions.faceDirection = (int) Mathf.Sign(velocity.x);
        }

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);

        //Limitar o movimento do Player

       // MovemmentBounds();
     
        if (standinonPlataform)
        {
            collisions.bellow = true;
        }
    }

    //Controls vertical collisions + Control vertical collisions diagonally
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLegth = Mathf.Abs(velocity.y) + skinWidth * 8;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayorigin = (directionY == -1) ? rayCastOrigin.bottomLeft : rayCastOrigin.topLeft;
            rayorigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.up * directionY, rayLegth, collisionMask);
            Debug.DrawRay(rayorigin, Vector2.up * directionY * rayLegth, Color.red);

            if (hit)
            {
                if(hit.collider.tag == "Through")
                {
                    if(directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisions.FallingThroughPla)
                    {
                        continue;
                    }
                    if(playerInput.y == -1)
                    {
                        collisions.FallingThroughPla = true;
                        Invoke("ResetFallingThroughPlataform", .5f);
                        continue;
                    }
                
                }

                if (hit.collider.tag != "Building")
                {

                    velocity.y = (hit.distance - skinWidth) * directionY;
                    rayLegth = hit.distance;

                    if (collisions.climbSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(collisions.diagonalangle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }
                    collisions.above = directionY == 1;
                    collisions.bellow = directionY == -1;
                }
            }
        }

        if (collisions.climbSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLegth = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayorigin = ((directionX == -1) ? rayCastOrigin.bottomLeft : rayCastOrigin.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.right * directionX, rayLegth, collisionMask);

            if (hit)
            {
                float diagonalangle = Vector2.Angle(hit.normal, Vector2.up);
                if (diagonalangle != collisions.diagonalangle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.diagonalangle = diagonalangle;
                }
            }
        }

    }

  //Controls horizontal collisions + Control horizontal collisions diagonally
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = collisions.faceDirection;
        float rayLegth = Mathf.Abs(velocity.x) + skinWidth ;

        if(Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLegth = 6 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayorigin = (directionX == -1) ? rayCastOrigin.bottomLeft : rayCastOrigin.bottomRight;
            rayorigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.right * directionX, rayLegth, collisionMask);

            Debug.DrawRay(rayorigin, Vector2.right * directionX * rayLegth * 8, Color.red);

            if (hit)
            {

                //Get Angle
                float diagonalangle = Vector2.Angle(hit.normal, Vector2.up);
                //print(diagonalangle);
                if (i == 0 && diagonalangle <= maxClimbAngle)
                {
                    if (collisions.descendSlope)
                    {
                        collisions.descendSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceSlopeStart = 0;
                    if (diagonalangle != collisions.diagonalangleOld)
                    {
                        distanceSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, diagonalangle);
                    velocity.x += distanceSlopeStart * directionX;
                }

                if (!collisions.climbSlope || diagonalangle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLegth = hit.distance;

                    if (collisions.climbSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.diagonalangle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }

    }

  //Diagonal Climb
    void ClimbSlope(ref Vector3 velocity, float diagonalangle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(diagonalangle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(diagonalangle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.bellow = true;
            collisions.climbSlope = true;
            collisions.diagonalangle = diagonalangle;
        }
    }

//Descending diagonally
    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? rayCastOrigin.bottomRight : rayCastOrigin.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float diagonalangle = Vector2.Angle(hit.normal, Vector2.up);
            if (diagonalangle != 0 && diagonalangle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if ((hit.distance - skinWidth) <= Mathf.Tan(diagonalangle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(diagonalangle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(diagonalangle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.diagonalangle = diagonalangle;
                        collisions.descendSlope = true;
                        collisions.bellow = true;

                    }
                }
            }
        }

    }

    void ResetFallingThroughPlataform()
    {
        collisions.FallingThroughPla = false;
    }

    public struct CollisionInfo
    {
        public bool above, bellow;
        public bool left, right;
        public bool climbSlope;
        public bool descendSlope;
        public float diagonalangle, diagonalangleOld;
        public int faceDirection;
        public bool FallingThroughPla;


        public Vector3 velocityOld;

        public void Reset()
        {
            above = false;
            bellow = false;
            left = false;
            right = false;
            climbSlope = false;
            descendSlope = false;
            diagonalangleOld = diagonalangle;
            diagonalangle = 0;
        }
    }

}
