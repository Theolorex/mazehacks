using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minotaur : MonoBehaviour
{


    private State _currentState;
    private float _currentSpeed; //move in intervals of this number
    private float _chargeTimer;
    private float _closenessRatio; //increase the ratio based off tiles in distance to player, 
    private float _lostTimer;
    private bool _playerSeen;
    private enum State
    {
        FINDING, //simply following the player, offscreen, should probably be faster to keep tension
        CHASING, 
        CHARGING,
        SCARING
    }

    void Start()
    {
        _currentState = State.FINDING;
        _currentSpeed = 5f;
    }

    void Update()
    {
        CalcDistance();
        
        switch (_currentState)
        {
            case State.FINDING:
                Movement();
                if (_closenessRatio == 0.0f)
                {
                    //start timer
                    //if timer reaches a certain point, 
                    _currentState = State.SCARING;
                }
                
                if (_playerSeen)
                {
                    //maybe make exclaimation visual to show its seen the player
                    _currentState = State.CHASING;
                }
                break;
            case State.CHASING:
                
                Movement();
                break;
            case State.CHARGING:
                //start timer
                //if player is still within a line of tiles, even through walls, set
                break;
            case State.SCARING:
                FindJumpscare(); //find a valid breakable tile closest to the player 
                if (_closenessRatio == 0.6f) //arbitrary closeness rn
                {
                    //SmashTile()
                    _currentState = State.CHASING;
                }
                break;
        }
        
        minoVisuals();
    }

    private void Movement()
    {
        //based on interval
        //jump to next tile
        //lerp position
    }
    
    //minotaur goals
    //follow the player ASTAR 
    //kill player if on same tile
    if ()
    {
        GameManager.Instance.AttemptFail();
    }
        
        //MINOTAUR SPAWN IN BREAKABLE TILE IF TOO FAR 
    
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
        _closenessRatio = //convert number of tiles between minotaur to player to a ratio,
                          //0.0 at a certain amount of tiles away so no shaking past like 5 tiles
                          //1.0 when hugging tiles to trigger catch, or just make it a hitbox to simplify later
    }

}
