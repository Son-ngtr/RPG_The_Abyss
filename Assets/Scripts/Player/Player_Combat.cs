using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Settings")]
    [SerializeField] private float counterDuration = 0.2f;

    public bool CounterAttackPerformed()
    {
        bool hasCounteredSomebody = false;

        foreach (var target in GetDetectedColliders())
        {
            // Try to get ICounterable component from the target
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable != null)
            {
                counterable.HandleCounter();
                hasCounteredSomebody = true;
            }
        }

        return hasCounteredSomebody;
    }

    public float GetCounterDuration() => counterDuration;
}
