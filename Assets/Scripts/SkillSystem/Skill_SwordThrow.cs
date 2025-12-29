using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    [Range(0f, 10f)]
    [SerializeField] private float throwForce = 6f;
    [SerializeField] private float swordGravity = 3.5f;

    [Header("TRAJECTORY SETTINGS")]
    [SerializeField] private GameObject predictionDotPrefab;
    [SerializeField] private int numberOfPredictionDots = 20;
    [SerializeField] private float predictionDotSpacing = 0.05f;
    private Transform[] dots;
    private Vector2 confirmedDirection;

    protected override void Awake()
    {
        base.Awake();

        dots = GenerateDots();
    }

    public void ThrowSword()
    {
        Debug.Log("Throwing sword in direction: " + confirmedDirection);
    }

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * predictionDotSpacing);
        }
    }

    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = throwForce * 10;

        // initial velocity component - the starting speed and direction of the sword throw
        Vector2 initialVelocity = direction * scaledThrowPower;

        // Gravity effect, pulling down over time. The longer sword is in the air, the more it drops.
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t);

        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect;

        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction.normalized;

    public void EnableDots(bool enable)
    {
        foreach (Transform dot in dots)
        {
            dot.gameObject.SetActive(enable);
        }
    }

    private Transform[] GenerateDots()
    {
        Transform[] tempDots = new Transform[numberOfPredictionDots];
        for (int i = 0; i < numberOfPredictionDots; i++)
        {
            tempDots[i] = Instantiate(predictionDotPrefab, transform.position, Quaternion.identity, transform).transform;
            tempDots[i].gameObject.SetActive(false);
        }
        return tempDots;
    }

}
