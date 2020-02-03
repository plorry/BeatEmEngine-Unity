using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerBehaviour : MonoBehaviour
{
    public enum Direction {LEFT, RIGHT};

    SideCollider left;
    SideCollider right;
    SideCollider front;
    SideCollider back;
    private float OFFSET_CONST = 0.65f;
    Vector3 dirVector;
    public BattlerSpriteBehaviour playerSprite;
    public AttackBox attackBox;
    bool inAir = false;
    public bool hasControl = true;
    Vector3 jumpVel;
    Vector3 gravity = new Vector3(0, 0.01f, 0);
    // Split the impulse vectors
    Vector3 impulse = new Vector3();
    Vector3 impulseVectorV;
    Vector3 impulseVectorH;
    // vPos = virtual position (as opposed to on-screen position)
    Vector3 vPos = new Vector3();
    public Direction facingDirection = Direction.RIGHT;
    // Originally this was just for directions, but now it's sort of a catch-all state manager for character
    protected Dictionary<string, bool> directionStatus = new Dictionary<string, bool>() {
        { "RIGHT", false },
        { "LEFT", false },
        { "UP", false },
        { "DOWN", false },
        { "JUMP", false },
        { "SLIDE", false },
        { "ATTACK", false },
        { "BLOCK", false},
    };
    // stats for the basic attack - can be pulled out to represent multiple attacks later maybe
    protected Dictionary<string, float> attackStats = new Dictionary<string, float>() {
        { "WINDUP_DURATION", 0.1f },
        { "WINDUP_SPEED", 0.2f },
        { "WINDUP_DECELERATION", 0 },
        { "ATTACK_DURATION", 0.1f },
        { "ATTACK_SPEED", 1.0f },
        { "ATTACK_DECELERATION", 0 },
    };
    private bool beingHit = false;
    private Queue<GameObject> targetSpots;
    protected Animator anim;
    // ____ CONFIGURABLE PROPERTIES ____
    public int attack_power = 1;
    public float speed = 1.65f;
    private float impulseDecel = 0.96f;

    // Start is called before the first frame update
    protected void Start() {
        vPos = PosToVPos(transform.position);
        InitTargetSpots();
        anim = playerSprite.GetComponent<Animator>();
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
        if (anim) {
            anim.SetFloat("speed", dirVector.magnitude);
        }
        // apply the movement by the speed value
        vPos += dirVector * speed / (1000 * Time.deltaTime);
        if (anim.GetBool("winding_up")) {
            vPos -= GetAttackDirection() * attackStats["WINDUP_SPEED"] / (1000 * Time.deltaTime);
        }
        if (anim.GetBool("attacking")) {
            vPos += GetAttackDirection() * attackStats["ATTACK_SPEED"] / (1000 * Time.deltaTime);
        }
        vPos += impulseVectorH / (1000 * Time.deltaTime);
        impulseVectorH *= impulseDecel;
        if (impulseVectorH.magnitude <= 0.01f) {
            impulseVectorH = Vector3.zero;
        }


        // and translate the virtual position to a coordinate position
        transform.position = VPosToPos(vPos);
    }

    private void BreakDownImpulse() {
        impulseVectorH = new Vector3(impulse.x, 0, impulse.z);
        // Jump velocity can only have a Y component
        jumpVel = new Vector3(0, impulse.y, 0);
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

    protected Vector3 GetAttackDirection() {
        // Similar to GetDirVector, but will always return either left or right if no direction is active
        Vector3 vec = GetDirVector();
        if (vec.magnitude == 0) {
            if (facingDirection == Direction.LEFT) {
                return Vector3.left;
            } else if (facingDirection == Direction.RIGHT) {
                return Vector3.right;
            }
        }
        return vec;
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
        Turn(Direction.RIGHT);
        directionStatus["RIGHT"] = true;
    }

    public void GoLeft() {
        Turn(Direction.LEFT);
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
            AudioManager.PlayClip("playerJump");
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
        AudioManager.PlayClip("playerLand");
    }

    public void LoseControl() {
        hasControl = false;
    }

    public void GainControl() {
        if (anim.GetBool("attacking") || anim.GetBool("blocking")) {
            return;
        }
        beingHit = false;
        hasControl = true;
    }

    public void Impulse(Direction direction, int magnitude) {
        impulse = new Vector3();
        inAir = true;
        LoseControl();
        AudioManager.PlayClip("playerHit");
        if (direction == Direction.RIGHT) {
            impulse += new Vector3(2f, 0.05f, 0) * magnitude;
        } else if (direction == Direction.LEFT) {
            impulse += new Vector3(-2f, 0.05f, 0) * magnitude;
        }
        BreakDownImpulse();
    }

    public void Attack() {
        if (CanAttack()) {
            directionStatus["ATTACK"] = true;
            LoseControl();
            AudioManager.PlayClip("playerAtk");
            Invoke("DoAttack", attackStats["WINDUP_DURATION"]);

            if (anim) {
                anim.SetBool("winding_up", true);
            }
        }
    }

    protected void DoAttack() {
        anim.SetBool("winding_up", false);
        anim.SetBool("attacking", true);
        AttackBox box = Instantiate(attackBox, playerSprite.transform);
        box.SetPower(attack_power);
        box.DestroyIn(attackStats["ATTACK_DURATION"]);
    }

    public void ResetAttack() {
        directionStatus["ATTACK"] = false;
    }

    public void EndAttack() {
        if (anim) {
            anim.SetBool("attacking", false);
        }
        GainControl();
    }

    public void Block() {
        anim.SetBool("blocking", true);
        LoseControl();
    }

    public void StopBlock() {
        anim.SetBool("blocking", false);
        GainControl();
    }

    public Vector3 GetActionPoint() {
        return new Vector3(0.66f, 0 , 0);
    }

    public void Hit(Direction direction, int power) {
        beingHit = true;
        Impulse(direction, power);
    }

    public Direction GetFacingDirection() {
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

    protected void Turn(Direction dir) {
        Direction prevDir = facingDirection;
        facingDirection = dir;

        if (dir != prevDir) {
            Vector3 scale = playerSprite.transform.localScale;
            scale.x *= -1;
            playerSprite.transform.localScale = scale;
        }
    }
}
