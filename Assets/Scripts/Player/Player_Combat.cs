using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterRecovery = 0.2f;
    [SerializeField] private LayerMask whatIsCounterable;

    public bool CounterAttackPerformed()
    {
        bool hasPerformedCounter = false;

        foreach (var target in GetDetectedColliders(whatIsCounterable))
        {
            // Try to get ICounterable component from the target
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
            {
                continue;
            }

            if (counterable.CanBeCountered)
            {
                counterable.HandleCounter();
                hasPerformedCounter = true;
            }
        }

        return hasPerformedCounter;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;
}
