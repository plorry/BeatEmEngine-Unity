using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerSpriteBehaviour : MonoBehaviour
{
    private Collider myCollider;
    private Rigidbody rb;
    private bool falling = false;
    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) {
            case "Floor":
                GetParent().Land();
                break;
            case "AttackBox":
                GetParent().Hit(other.gameObject.GetComponent<AttackBox>().GetFacingDirection());
                break;
            default:
                break;
        }
    }

    public BattlerBehaviour GetParent() {
        return transform.parent.GetComponent<BattlerBehaviour>();
    }

    public bool Fall(float distance) {
        RaycastHit hit;
        if (rb.SweepTest(Vector3.down, out hit, distance)) {
            transform.Translate(new Vector3(0, -hit.distance, 0));
            Land();
            return true;
        }
        return false;
    }

    public void EndAttack() {
        GetParent().EndAttack();
    }

    public Vector3 GetActionPoint() {
        return GetParent().GetActionPoint();
    }

    private void Land() {
        GetParent().Land();
    }
}
