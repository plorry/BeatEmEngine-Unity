using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerBehaviour : MonoBehaviour
{
    SideCollider left;
    SideCollider right;
    SideCollider front;
    SideCollider back;
    private float OFFSET_CONST = 0.65f;
    public float speed = 0.05f;
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
    protected Dictionary<string, bool> directionStatus = new Dictionary<string, bool>() {
        { "RIGHT", false },
        { "LEFT", false },
        { "UP", false },
        { "DOWN", false },
        { "JUMP", false },
        { "SLIDE", false },
        { "ATTACK", false }
    };
    private bool beingHit = false;
    private Queue<GameObject> targetSpots;

    // Start is called before the first frame update
    protected void Start() {
        vPos = PosToVPos(transform.position);
        InitTargetSpots();
    }

    protected void Awake() {
        InitColliders();
    }

    private void InitColliders() {
        front = transform.Find("Front").GetComponent<SideCollider>();
        back = transform.Find("Back").GetComponent<SideCollider>();
        left = transform.Find("Left").GetComponent<SideCollider>();
        right = transform.Find("Right").GetComponent<SideCollider>();
    }

    private void InitTargetSpots() {
        targetSpots = new Queue<GameObject>();

        targetSpots.Enqueue(CreateTargetSpot(2.5f, 0));
        targetSpots.Enqueue(CreateTargetSpot(-2.5f, 0));
        targetSpots.Enqueue(CreateTargetSpot(1.75f, 2.5f));
        targetSpots.Enqueue(CreateTargetSpot(1.75f, -2.5f));
        targetSpots.Enqueue(CreateTargetSpot(-1.75f, 2.5f));
        targetSpots.Enqueue(CreateTargetSpot(-1.75f, -2.5f));
    }

    private GameObject CreateTargetSpot(float x, float z) {
        GameObject point = new GameObject();
        point.transform.SetParent(transform, false);
        point.transform.position = transform.position + VPosToPos(new Vector3(x, 0, z));
        return point;
    }

    // Update is called once per frame
    protected void Update() {
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
        vPos += (dirVector * (speed / (Time.deltaTime * 33)) + impulseVectorH);
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
            if (directionStatus["RIGHT"] && !right.IsBlocked()) {
                vec += Vector3.right;
            }
            if (directionStatus["LEFT"] && !left.IsBlocked()) {
                vec += Vector3.left;
            }
            if (directionStatus["UP"] && !back.IsBlocked()) {
                vec += Vector3.forward;
            }
            if (directionStatus["DOWN"] && !front.IsBlocked()) {
                vec += Vector3.back;
            }
        }
        return vec.normalized;
    }

    protected Vector3 VPosToPos(Vector3 vPos) {
        // return an actual coordinate position in space from a vPos
        return new Vector3(vPos.x, vPos.z * 0.5f - OFFSET_CONST, vPos.z * 0.866f);
    }

    protected Vector3 PosToVPos(Vector3 pos) {
        return new Vector3(pos.x, (pos.z + OFFSET_CONST) / 0.5f, pos.z / 0.866f);
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

    public void SetStartPosition(int x, int z) {
        vPos = new Vector3(x, 0, z);
    }

    public GameObject GetAvailableTargetSpot() {
        GameObject targetSpot;

        try {
            targetSpot = targetSpots.Dequeue();
        } catch (System.InvalidOperationException e) {
            return null;
        }
        return targetSpot;
    }

    public void EnqueueTargetSpot(GameObject targetSpot) {
        targetSpots.Enqueue(targetSpot);
    }

    public Vector3 GetVPos() {
        return vPos;
    }
}
