using UnityEngine;

// PredictTrajectory --> Update the positions of the trajectory dots based on the current aiming direction
// ConfirmTrajectory --> Store the final aiming direction when the player confirms the throw --> used when actually throwing the sword

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword;

    [Header("Regular Sword Upgrade")]
    [Range(0f, 10f)]
    [SerializeField] private float throwForce = 6f;
    [SerializeField] private GameObject swordPrefab;

    [Header("TRAJECTORY SETTINGS")]
    [SerializeField] private GameObject predictionDotPrefab;
    [SerializeField] private int numberOfPredictionDots = 20;
    [SerializeField] private float predictionDotSpacing = 0.05f;
    private float swordGravity;
    private Transform[] dots;
    private Vector2 confirmedDirection;

    protected override void Awake()
    {
        base.Awake();

        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();
    }

    public override bool CanUseSkill()
    {
        if (currentSword != null)
        {
            currentSword.GetSwordBacktoPlayer();
            return false;
        }

        return base.CanUseSkill();
    }

    public void ThrowSword()
    {
        // Instantiate the sword at the position of the first dot in the predicted trajectory
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowPower());
    }

    private Vector2 GetThrowPower() => confirmedDirection * throwForce * 10f;

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
