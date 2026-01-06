using UnityEngine;

// PredictTrajectory --> Update the positions of the trajectory dots based on the current aiming direction
// ConfirmTrajectory --> Store the final aiming direction when the player confirms the throw --> used when actually throwing the sword

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword;
    private float currentThrowForce;

    [Header("REGULAR SWORD UPGRADE")]
    [SerializeField] private GameObject swordPrefab;
    [Range(0f, 10f)]
    [SerializeField] private float regularThrowForce = 6f;

    [Header("PIERCE SWORD UPGRADE")]
    [SerializeField] private GameObject pierceSwordPrefab;
    public int pierceAmount = 2;
    [Range(0f, 10f)]
    [SerializeField] private float pireceThrowForce = 6f;

    [Header("SPIN SWORD UPGRADE")]
    [SerializeField] private GameObject spinSwordPrefab;
    public int maxDistance = 7;
    public float attackPerSecond = 6;
    public float maxSpinDuration = 3f;
    [Range(0f, 10f)]
    [SerializeField] private float spinThrowForce = 6f;

    [Header("BOUNCE SWORD UPGRADE")]
    [SerializeField] private GameObject bounceSwordPrefab;
    public int bounceCount = 5;
    public float bounceSpeed = 12f;
    [Range(0f, 10f)]
    [SerializeField] private float bounceThrowForce = 6f;

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
        UpdateThrowForce();

        if (currentSword != null)
        {
            currentSword.GetSwordBacktoPlayer();
            return false;
        }

        return base.CanUseSkill();
    }

    public void ThrowSword()
    {
        GameObject currentSwordPrefab = GetSwordPrefab();
        // Instantiate the sword at the position of the first dot in the predicted trajectory
        GameObject newSword = Instantiate(currentSwordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowPower());

        SetSkillOnCoolDown();
    }

    private GameObject GetSwordPrefab()
    {
        switch(upgradeType)
        {
            case SkillUpgradeType.SwordThrow:
                return swordPrefab;

            case SkillUpgradeType.SwordThrow_Pierce:
                return pierceSwordPrefab;

            case SkillUpgradeType.SwordThrow_Spin:
                return spinSwordPrefab;

            case SkillUpgradeType.SwordThrow_Bounce:
                return bounceSwordPrefab;

            default:
                return null;
        }
    }

    private void UpdateThrowForce()
    {
        switch(upgradeType)
        {
            case SkillUpgradeType.SwordThrow:
                currentThrowForce = regularThrowForce;
                break;
            case SkillUpgradeType.SwordThrow_Pierce:
                currentThrowForce = pireceThrowForce;
                break;
            case SkillUpgradeType.SwordThrow_Spin:
                currentThrowForce = spinThrowForce;
                break;
            case SkillUpgradeType.SwordThrow_Bounce:
                currentThrowForce = bounceThrowForce;
                break;
        }
    }

    private Vector2 GetThrowPower() => confirmedDirection * (currentThrowForce * 10f);

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * predictionDotSpacing);
        }
    }

    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = currentThrowForce * 10;

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
