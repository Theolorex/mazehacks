    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.UI;


    public class MinotaurManager : MonoBehaviour
    { 
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite upSprite;
        [SerializeField] private Sprite downSprite;
        [SerializeField] private Sprite leftSprite;
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private MazePathfinder pathfinder;
        [SerializeField] float jumpRadius = 0.6f; //arbitrary, serialiszed for testing
        [SerializeField] private float patienceTime;
        [SerializeField] private float lostTime;
        [SerializeField] private float stepTime;
        [SerializeField] float ragePower; //how much each time the minotaur ramps up by
        [SerializeField] private float waitSecond = 0.5f;

        [SerializeField] private float leapTime = 0.15f;
        [SerializeField] private AnimationCurve leapCurve;
        
        private bool _isLeaping;
        private float _leapStartTime;
        private Vector2 _leapStart;
        private Vector2 _leapTarget;
        
        
        private Vector2 _currentPosition; //THINK:how to get current position, noncleanly its kinda easy just whatever tile its touching
        private State _currentState;
        
        float predictPlayerTime;
                
        private float _moveInterval; //move in intervals of this number
        private float _chargeTimer;
        private float _lostTimer; //different time
        private float maxDistance = 10f;
        private float _closenessRatio; //increase the ratio based off tiles in distance to player
        private float _chaseTimer;
        private float _rageAmount;
        private float rampTime;
        private float angerTime;
        private float accumulatedRage;
        
        private Vector2 savedDirection;
        private bool checkingStraightRun;
        
        private bool _playerSeen;
        private bool _timerStarted;
        private bool _rageStarted;
        private bool _lostsightStarted;
        private bool _jumpFound;
        private bool straightRun;
        
        private enum State
        {
            FINDING, //simply following the player, offscreen
            CHASING, 
            CHARGING,
            SCARING
        }

        void Start()
        {
            _currentState = State.FINDING;
            _moveInterval = Time.time + stepTime;
        }

        public void Respawn()
        {
            Maze maze = FindObjectOfType<Maze>();
            if (maze == null)
            {
                Debug.LogWarning("MinotaurManager.Respawn: no Maze found in scene.");
                return;
            }

            List<Vector2Int> floorTiles = new List<Vector2Int>();
            for (int x = 0; x < maze.w; x++)
            {
                for (int y = 0; y < maze.h; y++)
                {
                    if (maze.Get(x, y) == 0)
                    {
                        floorTiles.Add(new Vector2Int(x, y));
                    }
                }
            }

            if (floorTiles.Count == 0)
            {
                Debug.LogWarning("MinotaurManager.Respawn: maze has no floor tiles yet.");
                return;
            }

            Vector2Int spawn = floorTiles[UnityEngine.Random.Range(0, floorTiles.Count)];
            transform.position = new Vector3(spawn.x, spawn.y, 0);

            _isLeaping = false;
            _currentState = State.FINDING;
            _moveInterval = Time.time + stepTime;
            accumulatedRage = 0;
            _rageStarted = false;
            _timerStarted = false;
            _lostsightStarted = false;
            checkingStraightRun = false;
            straightRun = false;
            _jumpFound = false;
        }

        void Update()
        {
            
            UpdateLeap();
            CalcDistance();
            Debug.Log(_currentState);
            
            switch (_currentState)
            {
                case State.FINDING:
                    Movement();
                    CheckPatience();
                    if (_playerSeen)
                    {
                        //maybe make exclaimation visual to show its seen the player
                        _currentState = State.CHASING;
                    }
                    break;
                case State.CHASING:
                    Movement();
                    
                    if (!_playerSeen) //while chasing, if player isn't seen....
                    {
                        if (!_lostsightStarted)
                        {
                            _chaseTimer = Time.time + lostTime;
                        }
                        _lostsightStarted = true;
                        
                        if (Time.time >= _chaseTimer)
                        {
                            _lostsightStarted = false;
                            accumulatedRage = 0;
                            _currentState = State.FINDING;
                        }
                    }
                    else
                    {
                        _lostsightStarted = false;
                        _chaseTimer = Time.time + lostTime; //reset timer if the minotaur sees player again
                    }
                    break;
                case State.CHARGING:
                    //start timer
                    //if player is still within a line of tiles, even through walls, set
                    break;
                case State.SCARING:
                    FindJumpscare(); //find a valid breakable tile closest to the player 
                    if (_closenessRatio <= jumpRadius && _jumpFound) //arbitrary closeness rn
                    {
                        //SmashTile()
                        _currentState = State.CHASING;
                    }
                    break;
            }
        }

        private void FindJumpscare()
        {
            CheckStraightRun();

            if (straightRun)
            {
                _jumpFound = true;
            }
        }

        private void CheckStraightRun()
        {
            if (!checkingStraightRun)
            {
                savedDirection = Player.Instance.GetDirection();
                predictPlayerTime = Time.time + waitSecond;

                checkingStraightRun = true;
                straightRun = false;

                return;
            }

            if (Time.time < predictPlayerTime)
            {
                return;
            }
            
            if (savedDirection == Player.Instance.GetDirection())
            {
                straightRun = true;
            }
            else
            {
                straightRun = false;
            }

            checkingStraightRun = false;
            
        }

        private void SmashTile(Vector2 wallPosition)
        {
            
        }

        private void CheckPatience()
        {
            if (_closenessRatio == 0.0f)
            {
                if (!_timerStarted)
                {
                    _lostTimer = Time.time + patienceTime;
                    _timerStarted = true;
                }

                if (Time.time >= _lostTimer)
                {
                    _timerStarted = false;
                    _currentState = State.SCARING;
                }
            }
            else
            {
                _timerStarted = false;
            }
        }

        private void Movement()
        {

            if (_isLeaping)
            {
                return;
            }

            if (Time.time >= _moveInterval)
            {
                Vector2Int minoTile =
                    new Vector2Int(Mathf.FloorToInt(_currentPosition.x), Mathf.FloorToInt(_currentPosition.y));
                Vector2Int playerTile = new Vector2Int(Mathf.FloorToInt(Player.Instance.transform.position.x),
                    Mathf.FloorToInt(Player.Instance.transform.position.y));

                List<Vector2Int> path = pathfinder.FindPath(minoTile, playerTile);

                if (path != null && path.Count > 1)
                {
                    _leapStart = transform.position;
                    _leapTarget = path[1];

                    Vector2 direction = _leapTarget - _leapStart;
                    
                    UpdateDirection(direction);
                    
                    _leapStartTime = Time.time;
                    _isLeaping = true;
                    
                    minoVisuals();
                }

                _moveInterval = Time.time + stepTime;

                if (_currentState == State.CHASING)
                {
                    RampMovement();
                }

            }



        }
        
        private void UpdateDirection(Vector2 direction)
        {
            if (direction.x > 0)
                spriteRenderer.sprite = rightSprite;
            else if (direction.x < 0)
                spriteRenderer.sprite = leftSprite;
            else if (direction.y > 0)
                spriteRenderer.sprite = upSprite;
            else if (direction.y < 0)
                spriteRenderer.sprite = downSprite;
        }
        private void UpdateLeap()
        {
            if (!_isLeaping)
                return;

            float t = (Time.time - _leapStartTime) / leapTime;

            float curvedT = leapCurve.Evaluate(t);

            transform.position = Vector2.Lerp(
                _leapStart,
                _leapTarget,
                curvedT
            );
            
            if (t >= 1f)
            {
                transform.position = _leapTarget;
                _isLeaping = false;
            }
        }
        private void RampMovement()
        {
            //start timer
            //each tick normally decreases moveInterval by a tiny amount
            //overall timer will multiply that number
            if (!_rageStarted)
            {
                rampTime = Time.time + angerTime;
            }
            _rageStarted = true;

            _moveInterval -= accumulatedRage;
            
            if (Time.time >= rampTime)
            {
                Debug.Log("ramped rage");
                accumulatedRage += ragePower;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("TRIGGER WITH: " + collision.gameObject.name);

            if (collision.CompareTag("Player"))
            {
                GameManager.Instance.GameFail();
            }
        }

        
        //IF PLAYER is next to minotaur, cause camera shakes

        private void minoVisuals()
        {
            if (_closenessRatio > 0.0f) //if at all close
            {
                float convertClosenessRatio = _closenessRatio * 0.15f; //the cam manager shake amount is def not gonna be 1:1 witht eh closeness ratio
                CamMove.Instance.CameraShake(convertClosenessRatio);
            }
        }

        private void CalcDistance()
        {
            //math.flooring function floor the players position divided by tile size  //divided by tile size???
            _currentPosition = new Vector2(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));
            Debug.Log("here cause my math might be wrong, current mino position is " + _currentPosition);
            float distance = Vector2.Distance(_currentPosition, Player.Instance.GetPosition()); //convert number of tiles between minotaur to player to a ratio,
            _closenessRatio = 1f - Mathf.Clamp01(distance / maxDistance); //ratio 
            
            //1.0 when hugging tiles to trigger catch, or just make it a hitbox to simplify later
        }

        private Vector2 GetMinoPosition()
        {
            return _currentPosition;
        }
        
        
        
        

    }
