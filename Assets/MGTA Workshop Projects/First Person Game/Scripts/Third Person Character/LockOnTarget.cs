using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGTA
{
    public class LockOnTarget : MonoBehaviour
    {

        public static List<LockOnTarget> visibleTargets = new List<LockOnTarget>();

        //[Tooltip("If left empty, will lock onto the center of this object.")]
        //public Transform lockPoint;

        private void OnBecameVisible()
        {
            visibleTargets.Add(this);
        }

        private void OnBecameInvisible()
        {
            visibleTargets.Remove(this);
        }

        private void OnDisable()
        {
            visibleTargets.Remove(this);
        }

        public static LockOnTarget ClosestVisibleTarget(Vector3 yourPosition)
        {
            if (visibleTargets.Count <= 0 || visibleTargets == null) return null;
            float lowestDist = Mathf.Infinity;
            LockOnTarget selectedTarget = visibleTargets[0];
            foreach (LockOnTarget i in visibleTargets)
            {
                float dist = Vector3.Distance(yourPosition, i.transform.position);
                if (dist < lowestDist)
                {
                    lowestDist = dist;
                    selectedTarget = i;
                }
            }
            return selectedTarget;
        }
    }
}