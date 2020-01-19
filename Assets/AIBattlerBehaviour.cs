using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBattlerBehaviour : BattlerBehaviour
{
    public BattlerBehaviour target; // the target Battler
    private Transform targetSpot; // the target Spot to follow

    protected void Update() {
        base.Update();

        if (!targetSpot) {
            PickTargetSpot();
        }

        if (targetSpot) {
            MoveToward( PosToVPos(targetSpot.position) );
        }
    }

    public void SetTarget(BattlerBehaviour newTarget) {
        target = newTarget;
        PickTargetSpot();
    }

    private void PickTargetSpot() {
        targetSpot = target.GetAvailableTargetSpot().transform;
    }

    private void MoveToward(Vector3 targetPosition) {
        Vector3 deltaVector = targetPosition - GetVPos();
        if (deltaVector.x > 0.1) {
            GoRight();
        } else if (deltaVector.x < -0.1) {
            GoLeft();
        } else {
            StopLeft();
            StopRight();
        }

        if (deltaVector.z > 0.1) {
            GoUp();
        } else if (deltaVector.z < -0.1) {
            GoDown();
        } else {
            StopUp();
            StopDown();
        }
    }

    public void GoRight() {
        directionStatus["RIGHT"] = true;
    }

    public void GoLeft() {
        directionStatus["LEFT"] = true;
    }
}
