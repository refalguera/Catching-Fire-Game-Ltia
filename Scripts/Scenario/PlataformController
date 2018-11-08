using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contrala o Movimento das Plataformas Moveis
public class PlataformController : RayCastController {

    LayerMask passengerMask;

    public Vector3[] localWayPoints;
    Vector3[] globalWayPoints;

    public float speed;
    public bool cyclic;
    public float WaitTime;
    [Range(0,2)]
    public float EaseAmount;

    int fromWayPointIndex;
    float percentBetweenWayPoints;
    float NextMovetime;

    List<PassengersMovement> passengerMovement;
    Dictionary<Transform, PlayerPhysics> passengerDictionary = new Dictionary<Transform, PlayerPhysics>();

    public override void Start()
    {
        base.Start();

        globalWayPoints = new Vector3[localWayPoints.Length];
        for(int i = 0; i < localWayPoints.Length; i++)
        {
            globalWayPoints[i] = localWayPoints[i] + transform.position;
        }
    }

    private void Update()
    {
        UpdateRayCastOrigins();
        Vector3 velocity = CalculatePlataformMovement();

        CalculatePassengersMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    float Ease(float x)
    {
        float a = EaseAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
    // Platform Movement: Calculates the point at which point the platform moves
    // and its speed and the waiting time to move to the other point
    Vector3 CalculatePlataformMovement()
    {
        if(Time.time < NextMovetime)
        {
            return Vector3.zero;
        }
        fromWayPointIndex %= globalWayPoints.Length;

        int toWayPonitIndex = (fromWayPointIndex + 1) % globalWayPoints.Length;
        float DistanceBetweenWayPoints = Vector3.Distance(globalWayPoints[fromWayPointIndex], globalWayPoints[toWayPonitIndex]);
        percentBetweenWayPoints += Time.deltaTime * speed / DistanceBetweenWayPoints;
            
        //Deixa a variavel co valores entre 0 e 1 para que nao tenha valores diferentes da função Ease retorna; 
        percentBetweenWayPoints = Mathf.Clamp01(percentBetweenWayPoints);
        float EasePercBetweenWayPoints = Ease(percentBetweenWayPoints);

        Vector3 newPos = Vector3.Lerp(globalWayPoints[fromWayPointIndex], globalWayPoints[toWayPonitIndex], EasePercBetweenWayPoints);
        if(percentBetweenWayPoints >= 1)
        {
            percentBetweenWayPoints = 0;
            fromWayPointIndex++;

            if (!cyclic)
            {
                if (fromWayPointIndex >= globalWayPoints.Length - 1)
                {
                    fromWayPointIndex = 0;
                    System.Array.Reverse(globalWayPoints);
                }
                NextMovetime = Time.time + WaitTime;
            }
        }

        return newPos - transform.position;
       
    }

    void MovePassengers(bool beforemovePlataform)
    {
        foreach (PassengersMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
            {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<PlayerPhysics>());
            }

            if (passenger.moveBeforePlataform== beforemovePlataform)
            {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.stadingOnPlataform);
            }
        }
    }

   // Function that works with any object that was affected by the movement of the platform (that is being moved by the platform);
    void CalculatePassengersMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengersMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Moviemnto Vertical da plataforma

        if (velocity.y != 0)
        {
            float rayLegth = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayorigin = (directionY == -1) ? rayCastOrigin.bottomLeft : rayCastOrigin.topLeft;
                rayorigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.up * directionY, rayLegth, passengerMask);
               

                if (hit && hit.distance != 0)
                {
                    //Cada passageiro sera movido somente uma vez a cada frame
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        passengerMovement.Add(new PassengersMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        // Check for horizontal collision of platform with some object
        if (velocity.x != 0)
        {
            float rayLegth = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayorigin = (directionX == -1) ? rayCastOrigin.bottomLeft : rayCastOrigin.bottomRight;
                rayorigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.right * directionX, rayLegth, passengerMask);

               
                //If there was a collision and the distance between the platform and the collision object was different from zero
                if (hit && hit.distance !=0)
                {
                   // Each passenger will be moved only once to each frame
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        passengerMovement.Add(new PassengersMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Check for vertical collision of platform with some object
          if(directionY == -1 || velocity.y == 0 && velocity.x !=0)
        {
            float rayLegth = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayorigin = rayCastOrigin.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayorigin, Vector2.up, rayLegth, passengerMask);
               
                //If there was a collision and the distance between the platform and the collision object different from zero
                if (hit && hit.distance != 0)
                {
                  // Each passenger will be moved only once to each frame
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengersMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    //Moves the object that is on the platform based on the movement of the platform
    struct PassengersMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool stadingOnPlataform;
        public bool moveBeforePlataform;

        public PassengersMovement(Transform _transform,Vector3 _velocity, bool _stadingOnPlataform,bool _moveBeforePlataform)
        {
            transform = _transform;
            velocity = _velocity;
            stadingOnPlataform = _stadingOnPlataform;
            moveBeforePlataform = _moveBeforePlataform;
        }
    }
  //Draws in the scene the points chosen for departure and destination of the platform
    private void OnDrawGizmos()
    {
        if(localWayPoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWayPoints.Length; i++)
            {
                Vector3 globalWayPointPos = (Application.isPlaying)?globalWayPoints[i] : localWayPoints[i] + transform.position;
                Gizmos.DrawLine(globalWayPointPos - Vector3.up * size, globalWayPointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWayPointPos - Vector3.left * size, globalWayPointPos + Vector3.left * size);
            }
        }
    }
}
