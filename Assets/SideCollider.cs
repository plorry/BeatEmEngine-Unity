using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour
{
    bool blocked = false;
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreCollision(transform.parent.Find("PlayerSprite").GetComponent<BoxCollider>(), GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        blocked = true;    
    }

    private void OnTriggerExit(Collider other) {
        blocked = false;
    }

    public bool IsBlocked() {
        return blocked;
    }
}
