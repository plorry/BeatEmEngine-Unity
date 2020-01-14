using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerSpriteBehaviour : MonoBehaviour
{
    private BattlerBehaviour parent;
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
                parent.Land();
                break;
            case "AttackBox":
                GetParent().Hit(other.gameObject.GetComponent<AttackBox>().GetFacingDirection());
                break;
        }
    }

    public void SetParent(BattlerBehaviour newParent) {
        parent = newParent;
    }

    public BattlerBehaviour GetParent() {
        return parent;
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
        parent.EndAttack();
    }

    public Vector3 GetActionPoint() {
        return parent.GetActionPoint();
    }

    private void Land() {
        parent.Land();
    }
}
