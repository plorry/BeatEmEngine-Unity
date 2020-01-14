﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerBehaviour : MonoBehaviour
{
    private float OFFSET_CONST = 0.65f;
    float speed = 0.05f;
    Vector3 dirVector;
    public BattlerSpriteBehaviour playerSprite;
    public AttackBox attackBox;
    bool inAir = false;
    public bool hasControl = true;
    Vector3 jumpVel;
    Vector3 gravity = new Vector3(0, 0.01f, 0);
    // Split the impulse vectors
    Vector3 impulseVectorV;
    Vector3 impulseVectorH;
    // vPos = virtual position (as opposed to on-screen position)
    Vector3 vPos = new Vector3();
    private string facingDirection = "right";
    Dictionary<string, bool> directionStatus = new Dictionary<string, bool>() {
        { "RIGHT", false },
        { "LEFT", false },
        { "UP", false },
        { "DOWN", false },
        { "JUMP", false },
        { "SLIDE", false },
        { "ATTACK", false }
    };
    private bool beingHit = false;
    
    // Start is called before the first frame update
    void Start() {
        playerSprite.SetParent(this);
    }

    // Update is called once per frame
    void Update() {
        UpdateControl();
        UpdatePosition();
    }

    void UpdateControl() {
        
    }

    void UpdatePosition() {
        // Apply air update if necessary.
        BreakDownImpulse();
        if (inAir) {
            var landed = false;
            if (jumpVel.y < 0) {
                // Here we see if we can just connect the sprite to the ground
                landed = playerSprite.Fall(-jumpVel.y);
            }
            if (!landed) {
                // we're still in the air; fall the regular amount
                playerSprite.transform.position += jumpVel;
                ApplyGravity();
            }
        }
        // Get the direction of movement from the combined directions
        dirVector = GetDirVector();
        // apply the movement by the speed value
        vPos += (dirVector * speed + impulseVectorH);
        // and translate the virtual position to a coordinate position
        transform.position = VPosToPos(vPos);
    }

    private void BreakDownImpulse() {
        impulseVectorH = new Vector3(jumpVel.x, 0, jumpVel.z);
        // Jump velocity can only have a Y component
        jumpVel = new Vector3(0, jumpVel.y, 0);
    }

    Vector3 GetDirVector() {
        Vector3 vec = new Vector3();
        if (hasControl || inAir) {
            if (directionStatus["RIGHT"]) {
                vec += Vector3.right;
            }
            if (directionStatus["LEFT"]) {
                vec += Vector3.left;
            }
            if (directionStatus["UP"]) {
                vec += Vector3.forward;
            }
            if (directionStatus["DOWN"]) {
                vec += Vector3.back;
            }
        }
        return vec.normalized;
    }

    Vector3 VPosToPos(Vector3 vPos) {
        // return an actual coordinate position in space from a vPos
        return new Vector3(vPos.x, vPos.z * 0.5f - OFFSET_CONST, vPos.z * 0.866f);
    }

    void ApplyGravity() {
        jumpVel -= gravity;
    }

    private bool CanJump() {
        return !inAir && !directionStatus["JUMP"];
    }

    private bool CanAttack() {
        return hasControl && !directionStatus["ATTACK"];
    }
    
    // _______PUBLIC_______
    
    public void GoRight() {
        facingDirection = "right";
        directionStatus["RIGHT"] = true;
    }

    public void GoLeft() {
        facingDirection = "left";
        directionStatus["LEFT"] = true;
    }

    public void StopRight() {
        directionStatus["RIGHT"] = false;
    }

    public void StopLeft() {
        directionStatus["LEFT"] = false;
    }

    public void GoUp() {
        directionStatus["UP"] = true;
    }

    public void GoDown() {
        directionStatus["DOWN"] = true;
    }

    public void StopUp() {
        directionStatus["UP"] = false;
    }

    public void StopDown() {
        directionStatus["DOWN"] = false;
    }

    public void Jump() {
        if (CanJump()) {
            jumpVel = new Vector3(0, 0.15f, 0);
            inAir = true;
        }
        directionStatus["JUMP"] = true;
    }

    public void ResetJump() {
        directionStatus["JUMP"] = false;
    }

    public void Land(){
        inAir = false;
        if (!hasControl) {
            GainControl();
        }
    }

    public void LoseControl() {
        hasControl = false;
    }

    public void GainControl() {
        beingHit = false;
        hasControl = true;
    }

    public Vector3 Impulse(string direction, float magnitude) {
        inAir = true;
        LoseControl();
        if (direction == "right") {
            return new Vector3(0.15f, 0.05f, 0);
        } else if (direction == "left") {
            return new Vector3(-0.15f, 0.05f, 0);
        } else {
            return new Vector3();
        }
    }

    public void Attack() {
        if (CanAttack()) {
            directionStatus["ATTACK"] = true;
            LoseControl();
            Instantiate(attackBox, playerSprite.transform);
        }
    }

    public void ResetAttack() {
        directionStatus["ATTACK"] = false;
    }

    public void EndAttack() {
        GainControl();
    }

    public Vector3 GetActionPoint() {
        if (facingDirection == "right") {
            return new Vector3(0.66f, 0 , 0);
        } else if (facingDirection == "left") {
            return new Vector3(-0.66f, 0, 0);
        } else {
            return new Vector3();
        }
    }

    public void Hit(string direction) {
        beingHit = true;
        jumpVel = Impulse(direction, 1);
    }

    public string GetFacingDirection() {
        return facingDirection;
    }
}