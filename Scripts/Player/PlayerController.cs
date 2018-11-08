using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerPhysics))]

public class PlayerController : MonoBehaviour {

    private PlayerPhysics _controller;
    private float _gravity;
    private float _moveSpeed = 6;
    private float _maxjumpvelocity;
    private float _minjumpvelocity;

    private Animator _anim;
    private bool _isseen = false;

    [SerializeField]
    private float _FireRate = 0.25f;
    private float CanFire = 0.0f;
    [SerializeField]
    private Camera _cam;

    [HideInInspector]
    public int _life;

    [SerializeField]
    private GameObject _weaponprefab;

    [SerializeField]
    private Transform _handprefab;


    [HideInInspector]
    public float maxjumpHeight = 4;
    [HideInInspector]
    public float minjumpHeight = 1;
    [HideInInspector]
    public float timetoJumpApex = .4f;
    [HideInInspector]
    public float wallSlidingSpeedMax = 3;
    [HideInInspector]
    public float wallStickTime = .25f;
    [HideInInspector]
    float timeToWallUnstick;

    public Vector2 walljumpingClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    [HideInInspector]
    public bool _facingright;

    private float _velocityXSmoothing;
    private float _accelerationTimeAirBone = 0.2f;
    private float _accelerationTimeGrounded = 0.2f;

    [HideInInspector]
    public Vector3 velocity;

    private GameController _gameController;

    void Start()
    {
        _controller = GetComponent<PlayerPhysics>();
        _anim = GetComponent<Animator>();
        _gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        _life = 4;
      
        _gravity = -(2 * maxjumpHeight) / Mathf.Pow(timetoJumpApex, 2);
        _maxjumpvelocity = Mathf.Abs(_gravity) * timetoJumpApex;
        minjumpHeight = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * minjumpHeight);

         if(this.gameObject.name == "Girl")
        {
            _facingright = true;
        }
        else
        {
            _facingright = false;
        }

    }

    private void Update()
    {
            CameraVision();
            Movement();

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _anim.SetBool("Walk", true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                _anim.SetBool("Walk", false);
            }

            //Control the character's jump system
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _anim.SetBool("Jump", true);
                
                if (velocity.y > _minjumpvelocity)
                {
                    velocity.y = _minjumpvelocity;
                }
   
                //The longer the space key is pressed the higher the jump.
                if (_controller.collisions.bellow)
                {
                    velocity.y = _maxjumpvelocity;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _anim.SetBool("Jump", false);
            }
          
          //Controls the attack system of the player
            if (Input.GetKeyDown(KeyCode.C))
            {
                _anim.SetBool("Ataque", true);
                LaserShoot();
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                _anim.SetBool("Ataque", false);
            }
        
    }

    // Control the movement of the player for now with the right and left arrows;
    // It works with the movement of the player in the rise of walls;
    private void Movement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
            //Change direction animation
            if (_controller.collisions.faceDirection == 1 && !_facingright)
            {
                flip();
            }
            else if (_controller.collisions.faceDirection == -1 && _facingright)
            {
                flip();

            }

        float targerVelocityX = input.x * _moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targerVelocityX, ref _velocityXSmoothing, (_controller.collisions.bellow) ? _accelerationTimeGrounded : _accelerationTimeAirBone);


        velocity.y += _gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime,input);

            if(_controller.collisions.above || _controller.collisions.bellow)
            {
                velocity.y = 0;
            }

    }


    private void LaserShoot()
    {
        //Control the launch of the "weapon"
        if (Time.time > CanFire)
        {
            _armprefab.GetComponent<Ataque>()._objectDirection = _facingright;
            Instantiate(_armprefab, _handprefab.transform.position, Quaternion.identity);
        }

        CanFire = Time.time + _FireRate;

    }

  //Controls player life
    public void Damage()
    {
        if (_life < 1)
            {
                Destroy(this.gameObject);
            }

            _life--;
    }
    
    //Controls the camera's view. If the camera can not see the player, the player dies.
    void CameraVision()
    {
        Vector3 viewPos = _cam.WorldToViewportPoint(this.transform.position);
        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
        {
            Debug.Log("Camera saw the player");
            _isseen = true;
        }
        else {

            _isseen = false;
            _gameController._deadplayers.Enqueue(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    //Change the direction of the animations (right and left);
    private void flip()
    {
        _facingright = !_facingright;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

    }
}
