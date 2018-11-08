using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RayCastController : MonoBehaviour
{

    [HideInInspector]
    public BoxCollider2D collider;
    public RayCastOrigins rayCastOrigin;
    public LayerMask collisionMask;

    public const float skinWidth = .015f;
    [HideInInspector]
    public int horizontalRayCount = 3;
    [HideInInspector]
    public int verticalRayCount = 3;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    // Use this for initialization
    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
        print("" + horizontalRayCount);
    }
    
    //Updates the information from the source points of the released RayCasts.
    //There are 4 positions for the rays to leave: Top right, top left, bottom right and bottom left. 
    //These positions are taken through the limits of the size of the player.
    public void UpdateRayCastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        rayCastOrigin.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayCastOrigin.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayCastOrigin.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayCastOrigin.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    //Calculates the distance between each released ray
    public void CalculateRaySpacing()
    {
        Bounds bound = collider.bounds;
        bound.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 3, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 3, int.MaxValue);

        horizontalRaySpacing = bound.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bound.size.y / (verticalRayCount - 1);
    }

    public struct RayCastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}

