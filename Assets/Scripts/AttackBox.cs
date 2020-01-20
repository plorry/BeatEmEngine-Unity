using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    private int attack_power;
    // Start is called before the first frame update
    void Start() {
        transform.Translate(GetParent().GetActionPoint());
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void DestroyIn(float dur) {
        Destroy(gameObject, dur);
    }

    public BattlerBehaviour.Direction GetFacingDirection() {
        return GetParent().GetParent().GetFacingDirection();
    }

    private void OnDestroy() {
        GetParent().EndAttack();
    }

    private BattlerSpriteBehaviour GetParent() {
        return transform.parent.gameObject.GetComponent<BattlerSpriteBehaviour>();
    }

    public void SetPower(int power) {
        attack_power = power;
    }

    public int GetPower() {
        return attack_power;
    }
}
