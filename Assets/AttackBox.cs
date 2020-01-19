using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    float duration = 0.1f;
    // Start is called before the first frame update
    void Start() {
        transform.Translate(GetParent().GetActionPoint());
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public string GetFacingDirection() {
        return GetParent().GetParent().GetFacingDirection();
    }

    private void OnDestroy() {
        GetParent().EndAttack();
    }

    private BattlerSpriteBehaviour GetParent() {
        return transform.parent.gameObject.GetComponent<BattlerSpriteBehaviour>();
    }
}
