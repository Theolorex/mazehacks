using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class MinotaurManager : MonoBehaviour
{ 
    [SerializeField] private MazePathfinder pathfinder;
    [SerializeField] float jumpRadius = 0.6f; //arbitrary, serialiszed for testing
    [SerializeField] private float patienceTime;
    [SerializeField] private float lostTime;
    [SerializeField] private float stepTime;
    [SerializeField] float ragePower; //how much each time the minotaur ramps up by
    
    
    private Vector2 _currentPosition; //THINK:how to get current position, noncleanly its kinda easy just whatever tile its touching
    private State _currentState;
    
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
    
    
    
    private bool _playerSeen;
    private bool _timerStarted;
    private bool _rageStarted;
    private bool _lostsightStarted;
    private bool _jumpFound;
    
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
        _moveInterval = 5f;
    }

    void Update()
    {
        CalcDistance();
        
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
                RampMovement();
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
                    _chaseTimer = Time.time + lostTime; //reset timer if the minotaur sees player again
                }
                break;
            case State.CHARGING:
                //start timer
                //if player is still within a line of tiles, even through walls, set
                break;
            case State.SCARING:
                FindJumpscare(); //find a valid breakable tile closest to the player 
                if (_closenessRatio >= jumpRadius && _jumpFound) //arbitrary closeness rn
                {
                    //SmashTile()
                    _currentState = State.CHASING;
                }
                break;
        }
        
        minoVisuals();
    }

    private void FindJumpscare()
    {
        //find player direction, floor it 
        //get wall in that direction
        _jumpFound = true;
    }

    private void SmashTile()
    {
        //get reference to tile, destroy it 
    }

    private void CheckPatience()
    {
        if (_closenessRatio == 0.0f)
        {
            if (!_timerStarted)
            {
                _lostTimer = Time.time + patienceTime;
            }
            _timerStarted = true;

            if (Time.time >= _lostTimer)
            {
                _timerStarted = false;
                _currentState = State.SCARING;
            }
        }
    }

    private void Movement()
    {

        if (Time.time >= _moveInterval)
        {
            Vector2Int minoTile = new Vector2Int(Mathf.FloorToInt(_currentPosition.x), Mathf.FloorToInt(_currentPosition.y));
            Vector2Int playerTile = new Vector2Int(Mathf.FloorToInt(Player.Instance.transform.position.x), Mathf.FloorToInt(Player.Instance.transform.position.y));
                    
            List<Vector2Int> path = pathfinder.FindPath(minoTile, playerTile);
            
            if (path != null && path.Count > 1)
            {
                //MOVE HERE
            }
            
            _moveInterval = Time.time + stepTime;
        }

        if (_currentState == State.CHASING)
        {
            RampMovement();
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
            accumulatedRage += ragePower;
        }
    }
    private void CheckDeath()
    {
        if (_closenessRatio >= 0.0f)
        {
            GameManager.Instance.GameFail();
        }
    }

    
    //IF PLAYER is next to minotaur, cause camera shakes

    private void minoVisuals()
    {
        if (_closenessRatio > 0.0f)
        {
            float convertClosenessRatio = _closenessRatio; //the cam manager shake amount is def not gonna be 1:1 witht eh closeness ratio
            CameraManager.Instance.CameraShake(convertClosenessRatio);
        }
    }

    private void CalcDistance()
    {
        //math.flooring function floor the players position divided by tile size  //divided by tile size???
        _currentPosition = new Vector2(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));
        Debug.Log("here cause my math might be wrong, current mino position is " + _currentPosition);
        float distance = Vector2.Distance(_currentPosition, Player.Instance.GetDirection()); //convert number of tiles between minotaur to player to a ratio,
        _closenessRatio = 1f - Mathf.Clamp01(distance / maxDistance); //ratio 
        
        //1.0 when hugging tiles to trigger catch, or just make it a hitbox to simplify later
    }

    private Vector2 GetMinoPosition()
    {
        return _currentPosition;
    }
    
    
    
    

}
