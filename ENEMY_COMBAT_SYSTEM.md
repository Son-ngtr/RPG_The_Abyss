# ⚔️ ENEMY COMBAT SYSTEM - TECHNICAL DOCUMENTATION
## RPG: The Abyss - Chi tiết kỹ thuật hệ thống chiến đấu của Enemy

---

## 📋 MỤC LỤC

1. [Tổng quan hệ thống](#tổng-quan-hệ-thống)
2. [Kiến trúc State Machine](#kiến-trúc-state-machine)
3. [Hệ thống Damage & Combat](#hệ-thống-damage--combat)
4. [Chi tiết từng Enemy](#chi-tiết-từng-enemy)
5. [Counter System](#counter-system)
6. [Projectile System](#projectile-system)

---

## 🏗️ TỔNG QUAN HỆ THỐNG

### Core Components

Tất cả Enemy đều kế thừa từ class `Enemy : Entity` và sử dụng các component sau:

```csharp
// Core Components
public Enemy_Health health { get; private set; }
public Entity_Stats stats { get; set; }
public Entity_Combat combat { get; private set; }
public Entity_VFX vfx { get; private set; }
```

### State Machine Pattern

Mỗi Enemy sử dụng **State Machine Pattern** với các states cơ bản:

- **IdleState**: Đứng yên, chờ phát hiện player
- **MoveState**: Di chuyển tuần tra
- **BattleState**: Chiến đấu với player
- **AttackState**: Thực hiện tấn công
- **StunnedState**: Bị stun (có thể counter)
- **DeadState**: Chết

---

## 🎯 KIẾN TRÚC STATE MACHINE

### Base Class: `EnemyState`

```csharp
public abstract class EnemyState : EntityState
{
    protected Enemy enemy;
    
    // Constructor
    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName)
    
    // Update animation parameters mỗi frame
    public override void UpdateAnimationParameters()
    {
        float battleAnimSpeedMultiplier = enemy.battleMoveSpeed / enemy.moveSpeed;
        animator.SetFloat("battleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
        animator.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
        animator.SetFloat("xVelocity", rb.linearVelocity.x);
    }
}
```

### State Lifecycle

Mỗi state có 3 phương thức chính:

1. **Enter()**: Được gọi khi vào state
   - Set animation bool = true
   - Khởi tạo biến
   - Reset timer

2. **Update()**: Được gọi mỗi frame
   - Update logic
   - Check điều kiện chuyển state
   - Update animation parameters

3. **Exit()**: Được gọi khi rời state
   - Set animation bool = false
   - Cleanup

---

## ⚔️ HỆ THỐNG DAMAGE & COMBAT

### Entity_Combat Component

**File**: `Assets/Scripts/Entity/Entity_Combat.cs`

#### PerformAttack() - Tấn công cơ bản

```csharp
public void PerformAttack()
{
    bool targetGotHit = false;
    
    // Detect targets trong vùng tấn công
    foreach (var target in GetDetectedColliders(whatIsTarget))
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable == null) continue;
        
        // Tính toán damage từ stats
        AttackData attackData = stats.GetAttackData(basicAttackScale);
        
        float physicalDamage = attackData.physicalDamage;
        float elementalDamage = attackData.elementalDamage;
        ElementType element = attackData.element;
        
        // Apply damage
        targetGotHit = damageable.TakeDamage(physicalDamage, elementalDamage, element, transform);
        
        // Apply status effect nếu có
        if (element != ElementType.None)
        {
            statusHandler?.ApplyStatusEffect(element, attackData.effectData);
        }
        
        if (targetGotHit)
        {
            OnDoingPhysicalDamage?.Invoke(physicalDamage);
            vfx.CreateOnHitVfx(target.transform, attackData.isCrit, element);
            sfx?.PlayAttackHitSFX();
        }
    }
    
    // Play miss SFX nếu không trúng ai
    if (targetGotHit == false)
    {
        sfx?.PlayAttackMissSFX();
    }
}
```

#### Target Detection

```csharp
public Collider2D[] GetDetectedColliders(LayerMask whatToDetect)
{
    return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatToDetect);
}
```

- Sử dụng **Physics2D.OverlapCircleAll** để detect targets
- `targetCheck`: Transform điểm kiểm tra
- `targetCheckRadius`: Bán kính vùng tấn công
- `whatIsTarget`: LayerMask định nghĩa đối tượng có thể tấn công

---

## 🎮 CHI TIẾT TỪNG ENEMY

### 1. SKELETON ⚔️

**File**: `Assets/Scripts/Enemy/Skeleton/Enemy_Skeleton.cs`

#### Đặc điểm kỹ thuật:

- **Loại**: Melee cơ bản
- **States**: Idle → Move → Battle → Attack → Dead
- **Counter**: ✅ Có thể bị counter (`ICounterable`)

#### Code Structure:

```csharp
public class Enemy_Skeleton : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }
    
    protected override void Awake()
    {
        base.Awake();
        // Khởi tạo tất cả states
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }
    
    public void HandleCounter()
    {
        if (CanBeCountered == false) return;
        stateMachine.ChangeState(stunnedState);
    }
}
```

#### Combat Flow:

1. **Idle**: Đứng yên, check player detection
2. **Move**: Di chuyển tuần tra
3. **Battle**: Khi phát hiện player
   - Chase player với `battleMoveSpeed`
   - Check `WithinAttackRange()` và `CanAttack()`
   - Nếu đủ điều kiện → chuyển sang Attack
4. **Attack**: 
   - Gọi `SyncAttackSpeed()` để sync với attack speed stat
   - Animation trigger gọi `PerformAttack()`
   - Sau khi attack xong → quay về Battle
5. **Stunned**: Khi bị counter
   - Velocity bị đẩy lùi: `stunnedVelocity`
   - Duration: `stunnedDuration`
   - Sau khi hết → quay về Idle

---

### 2. ARCHER ELF 🏹

**File**: `Assets/Scripts/Enemy/Archer/Enemy_ArcherElf.cs`

#### Đặc điểm kỹ thuật:

- **Loại**: Ranged attacker
- **States**: Idle → Move → Battle (custom) → Attack → Dead
- **Counter**: ✅ Có thể bị counter

#### Custom Battle State:

**File**: `Assets/Scripts/Enemy/Archer/Enemy_ArcherElfBattleState.cs`

```csharp
public class Enemy_ArcherElfBattleState : Enemy_BattleState
{
    private bool canFlip;
    private bool reachedDeadEnd;
    
    public override void Update()
    {
        // Check nếu đã đến dead end (wall hoặc không có ground)
        if (enemy.isGrounded == false || enemy.isTouchingWall)
            reachedDeadEnd = true;
        
        // Nếu có thể tấn công
        if (CanAttack())
        {
            // Dừng lại và flip về hướng player nếu cần
            if (enemy.PlayerDetected() == false && canFlip)
            {
                enemy.HandleFlip(DirectionToPlayer());
                canFlip = false;
            }
            
            enemy.SetVelocity(0, rb.linearVelocity.y);
            
            // Nếu trong tầm tấn công → Attack
            if (WithinAttackRange() && enemy.PlayerDetected())
            {
                canFlip = true;
                lastTimeAttacked = Time.time;
                stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            // Chiến thuật: Lùi lại nếu player quá gần
            bool shouldWalkAway = reachedDeadEnd == false && 
                                   DistanceToPlayer() < (enemy.attackDistance * .85f);
            
            if (shouldWalkAway)
            {
                // Lùi lại với tốc độ battleMoveSpeed
                enemy.SetVelocity((enemy.GetBattleMoveSpeed() * -1) * DirectionToPlayer(), 
                                 rb.linearVelocity.y);
            }
            else
            {
                // Đứng yên và flip về hướng player
                enemy.SetVelocity(0, rb.linearVelocity.y);
                if (enemy.PlayerDetected() == false)
                    enemy.HandleFlip(DirectionToPlayer());
            }
        }
    }
}
```

#### Special Attack - Arrow:

```csharp
public override void SpecialAttack()
{
    GameObject newArrow = Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.identity);
    newArrow.GetComponent<Enemy_ArcherElfArrow>().SetupArrow(arrowSpeed * facingDirection, combat);
}
```

**Arrow Properties** (`Enemy_ArcherElfArrow.cs`):

```csharp
public void SetupArrow(float xVelocity, Entity_Combat combat)
{
    this.combat = combat;
    rb.linearVelocity = new Vector2(xVelocity, 0); // Tốc độ: arrowSpeed = 8
    
    // Rotate arrow sprite nếu bay ngược chiều
    if (rb.linearVelocity.x < 0)
        transform.Rotate(0, 180, 0);
}

private void OnTriggerEnter2D(Collider2D collision)
{
    // Check layer mask
    if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
    {
        combat.PerformAttackOnTarget(collision.transform);
        StuckIntoTarget(collision.transform); // Arrow stuck vào target
    }
}

private void StuckIntoTarget(Transform target)
{
    rb.linearVelocity = Vector2.zero;
    rb.bodyType = RigidbodyType2D.Kinematic;
    col.enabled = false;
    anim.enabled = false;
    transform.parent = target; // Attach vào target
    Destroy(gameObject, 3); // Tự hủy sau 3s
}
```

**Counter Arrow**:

```csharp
public void HandleCounter()
{
    // Đảo ngược hướng bay
    rb.linearVelocity = new Vector2(rb.linearVelocity.x * -1, 0);
    transform.Rotate(0, 180, 0);
    
    // Thay đổi layer mask để có thể damage enemy
    int enemyLayer = LayerMask.NameToLayer("Enemy");
    whatIsTarget = whatIsTarget | (1 << enemyLayer);
}
```

---

### 3. MAGE 🔮

**File**: `Assets/Scripts/Enemy/Mage/Enemy_Mage.cs`

#### Đặc điểm kỹ thuật:

- **Loại**: Magic caster với retreat mechanic
- **States**: Idle → Move → Battle → Retreat → SpellCast → Attack → Dead
- **Counter**: ✅ Có thể bị counter

#### Retreat System:

**File**: `Assets/Scripts/Enemy/Mage/Enemy_MageRetreatState.cs`

```csharp
public class Enemy_MageRetreatState : EnemyState
{
    private Vector3 startPosition;
    
    public override void Enter()
    {
        base.Enter();
        startPosition = enemy.transform.position;
        
        // Lùi lại với tốc độ cao (retreatSpeed = 15)
        rb.linearVelocity = new Vector2(enemyMage.retreatSpeed * -DirectionToPlayer(), 0);
        enemy.HandleFlip(DirectionToPlayer());
        
        // Untargetable trong lúc retreat
        enemy.gameObject.layer = LayerMask.NameToLayer("Untargetable");
        enemy.vfx.DoImageEchoEffect(1f); // VFX effect
    }
    
    public override void Update()
    {
        base.Update();
        
        // Check nếu đã retreat đủ xa hoặc không thể lùi thêm
        bool reachedMaxDistance = Vector2.Distance(enemy.transform.position, startPosition) > 
                                  enemyMage.retreatMaxDistance; // 8 units
        
        if (reachedMaxDistance || enemyMage.CantMoveBackwards())
            stateMachine.ChangeState(enemyMage.mageSpellCastState);
    }
    
    public override void Exit()
    {
        base.Exit();
        enemy.vfx.StopImageEchoEffect();
        enemy.gameObject.layer = LayerMask.NameToLayer("Enemy"); // Targetable lại
    }
}
```

**Collision Check**:

```csharp
public bool CantMoveBackwards()
{
    // Check wall phía sau
    bool detectedWall = Physics2D.Raycast(behindCollsionCheck.position, 
                                         Vector2.right * -facingDirection, 1.5f, groundLayer);
    // Check không có ground phía sau
    bool noGround = Physics2D.Raycast(behindCollsionCheck.position, 
                                     Vector2.down, 1.5f, groundLayer) == false;
    
    return noGround || detectedWall;
}
```

#### Spell Cast System:

**File**: `Assets/Scripts/Enemy/Mage/Enemy_MageSpellCastState.cs`

```csharp
public class Enemy_MageSpellCastState : EnemyState
{
    public override void Enter()
    {
        base.Enter();
        enemyMage.SetVelocity(0, 0); // Dừng lại khi cast
        enemyMage.SetSpellCastPerformed(false);
    }
    
    public override void Update()
    {
        base.Update();
        
        // Set animation parameter khi spell đã được cast
        if (enemyMage.spellCastPerformed)
            animator.SetBool("spellCast_performed", true);
        
        // Quay về battle state khi animation xong
        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
```

**Cast Spell Coroutine**:

```csharp
private IEnumerator CastSpellCo()
{
    // Bắn 3 projectile (amountToCast = 3)
    for (int i = 0; i < amountToCast; i++)
    {
        Enemy_MageProjectile projectile = Instantiate(spellPrefab, 
                                                      spellStartPosition.position, 
                                                      Quaternion.identity)
                                          .GetComponent<Enemy_MageProjectile>();
        
        projectile.SetupProjectile(player.transform, combat);
        yield return new WaitForSeconds(spellCastCooldown); // 0.3s giữa các đòn
    }
    
    SetSpellCastPerformed(true);
}
```

#### Mage Projectile - Ballistic Trajectory:

**File**: `Assets/Scripts/Enemy/Mage/Enemy_MageProjectile.cs`

```csharp
public void SetupProjectile(Transform target, Entity_Combat combat)
{
    this.combat = combat;
    
    // Tính toán ballistic velocity để projectile bay theo cung parabol
    Vector2 velocity = CalculateBallisticVelocity(transform.position, target.position);
    rb.linearVelocity = velocity;
}

private Vector2 CalculateBallisticVelocity(Vector2 start, Vector2 end)
{
    float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
    
    float displacementY = end.y - start.y;
    float displacementX = end.x - start.x;
    
    float peakHeight = Mathf.Max(arcHeight, end.y - start.y + .1f); // Đảm bảo arc luôn cao hơn target
    
    // Thời gian để lên đỉnh arc
    float timeToApex = Mathf.Sqrt(2 * peakHeight / gravity);
    
    // Thời gian để rơi từ đỉnh xuống target
    float timeFromApex = Mathf.Sqrt(2 * (peakHeight - displacementY) / gravity);
    
    // Tổng thời gian bay
    float totalTime = timeToApex + timeFromApex;
    
    // Vận tốc ban đầu theo trục Y để đạt peakHeight
    float velocityY = Mathf.Sqrt(2 * gravity * peakHeight);
    
    // Vận tốc ban đầu theo trục X để cover khoảng cách trong totalTime
    float velocityX = displacementX / totalTime;
    
    return new Vector2(velocityX, velocityY);
}
```

**Projectile Collision**:

```csharp
private void OnTriggerEnter2D(Collider2D collision)
{
    if (((1 << collision.gameObject.layer) & whatCanCollideWith) != 0)
    {
        combat.PerformAttackOnTarget(collision.transform);
        
        // Stop physics và play explosion animation
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        anim.enabled = true; // Play explosion anim
        col.enabled = false;
        Destroy(gameObject, 2);
    }
}
```

---

### 4. SLIME 🟢

**File**: `Assets/Scripts/Enemy/Slime/Enemy_Slime.cs`

#### Đặc điểm kỹ thuật:

- **Loại**: Melee với khả năng phân chia khi chết
- **States**: Idle → Move → Battle → Attack → Dead (custom) → Stunned
- **Counter**: ✅ Có thể bị counter

#### Split on Death:

**File**: `Assets/Scripts/Enemy/Slime/Enemy_SlimeDeadState.cs`

```csharp
public class Enemy_SlimeDeadState : Enemy_DeadState
{
    public override void Enter()
    {
        base.Enter();
        enemySlime.CreateSlimeOnDeath(); // Tạo slime con khi chết
    }
}
```

**Create Slime Logic**:

```csharp
public void CreateSlimeOnDeath()
{
    if (slimeToCreatePrefab == null) return;
    
    // Tạo 2 slime con (amountOfSlimesToCreate = 2)
    for (int i = 0; i < amountOfSlimesToCreate; i++)
    {
        GameObject newSlime = Instantiate(slimeToCreatePrefab, transform.position, Quaternion.identity);
        Enemy_Slime slimeScript = newSlime.GetComponent<Enemy_Slime>();
        
        // Điều chỉnh stats của slime con:
        // - HP/Defense giảm 60% (penalty = 0.6f)
        // - Attack tăng 20% (increase = 1.2f)
        slimeScript.stats.AdjustStatSetup(stats.resources, stats.offense, stats.defend, .6f, 1.2f);
        
        // Apply random velocity khi spawn
        slimeScript.ApplyRespawnVelocity();
        
        // Tự động vào battle state
        slimeScript.StartBattleStateCheck(player);
    }
}

public void ApplyRespawnVelocity()
{
    // Random velocity với hướng ngẫu nhiên
    Vector2 velocity = new Vector2(
        stunnedVelocity.x * Random.Range(-1f, 2f), 
        stunnedVelocity.y * Random.Range(2f, 4f)
    );
    SetVelocity(velocity.x, velocity.y);
}

public void StartBattleStateCheck(Transform player)
{
    TryEnterBattleState(player);
    // Check mỗi 0.3s để đảm bảo slime con vào battle state
    InvokeRepeating(nameof(ReEnterBattleState), 0, .3f);
}

private void ReEnterBattleState()
{
    if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
    {
        CancelInvoke(nameof(ReEnterBattleState));
        return;
    }
    stateMachine.ChangeState(battleState);
}
```

---

### 5. REAPER (BOSS) 💀

**File**: `Assets/Scripts/Enemy/Boss/Enemy_Reaper.cs`

#### Đặc điểm kỹ thuật:

- **Loại**: Boss với teleport + spell cast
- **States**: Idle → Move → Battle (custom) → Attack → Teleport → SpellCast → Dead
- **Counter**: ✅ Có thể bị counter

#### Battle State:

**File**: `Assets/Scripts/Enemy/Boss/Enemy_ReaperBattleState.cs`

```csharp
public class Enemy_ReaperBattleState : Enemy_BattleState
{
    public override void Enter()
    {
        base.Enter();
        // Set timer = maxBattleIdleTime (5s)
        stateTimer = enemyReaper.maxBattleIdleTime;
    }
    
    public override void Update()
    {
        base.Update();
        
        // Sau 5s → Teleport
        if (stateTimer < 0)
            stateMachine.ChangeState(enemyReaper.reaperTeleportState);
        
        // Check attack range
        if (WithinAttackRange() && enemy.PlayerDetected() && CanAttack())
        {
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(enemyReaper.reaperAttackState);
        }
        else
        {
            // Chase player nếu có thể
            float xVelocity = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;
            if (enemy.isGrounded == false)
                xVelocity = 0.00001f;
            
            enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);
        }
    }
}
```

#### Teleport System:

**File**: `Assets/Scripts/Enemy/Boss/Enemy_ReaperTeleportState.cs`

```csharp
public class Enemy_ReaperTeleportState : EnemyState
{
    public override void Enter()
    {
        base.Enter();
        enemyReaper.MakeUntargetable(true); // Untargetable khi teleport
    }
    
    public override void Update()
    {
        base.Update();
        
        // Animation trigger gọi teleport
        if (enemyReaper.teleporTrigger)
        {
            enemyReaper.transform.position = enemyReaper.FindTeleportPoint();
            enemyReaper.SetTeleportTrigger(false);
        }
        
        // Sau khi teleport xong
        if (triggerCalled)
        {
            // Nếu spell cast không trong cooldown → SpellCast
            if (enemyReaper.CanDoSpellCast())
                stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
            else
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        enemyReaper.MakeUntargetable(false); // Targetable lại
    }
}
```

**Find Teleport Point**:

```csharp
public Vector3 FindTeleportPoint()
{
    int maxAttempts = 10;
    float bossWithColliderHalf = collider.bounds.size.x / 2;
    
    for (int i = 0; i < maxAttempts; i++)
    {
        // Random X trong arena bounds
        float randomX = Random.Range(
            arenaBounds.bounds.min.x + bossWithColliderHalf,
            arenaBounds.bounds.max.x - bossWithColliderHalf
        );
        
        // Raycast từ trên xuống để tìm ground
        Vector2 raycastPoint = new Vector2(randomX, arenaBounds.bounds.max.y);
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint, Vector2.down, Mathf.Infinity, groundLayer);
        
        if (hit.collider != null)
            return hit.point + new Vector2(0, offsetCenterY); // Offset để boss đứng trên ground
    }
    
    return transform.position; // Fallback nếu không tìm được điểm hợp lệ
}
```

**Teleport Probability**:

```csharp
public bool ShouldTeleport()
{
    // Xác suất ban đầu = 25% (chanceToTeleport = 0.25f)
    if (Random.value < chanceToTeleport)
    {
        chanceToTeleport = defaultTeleportChance; // Reset về 25%
        return true;
    }
    
    // Tăng xác suất lên 5% mỗi lần không teleport
    chanceToTeleport = chanceToTeleport + .05f;
    return false;
}
```

#### Spell Cast System:

**File**: `Assets/Scripts/Enemy/Boss/Enemy_ReaperSpellCastState.cs`

```csharp
public class Enemy_ReaperSpellCastState : EnemyState
{
    public override void Enter()
    {
        base.Enter();
        enemyReaper.SetVelocity(0, 0);
        enemyReaper.SetSpellCastPreformed(false);
        enemyReaper.SetSpellCastOnCooldown(); // Set cooldown = 10s
    }
    
    public override void Update()
    {
        base.Update();
        
        if (enemyReaper.spellCastPreformed)
            animator.SetBool("spellCast_Performed", true);
        
        if (triggerCalled)
        {
            // Sau khi cast xong → check teleport hoặc quay về battle
            if (enemyReaper.ShouldTeleport())
                stateMachine.ChangeState(enemyReaper.reaperTeleportState);
            else
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
        }
    }
}
```

**Cast Spell Coroutine**:

```csharp
private IEnumerator CastSpellCo()
{
    if (playerScript == null)
        playerScript = player.GetComponent<Player>();
    
    // Bắn 6 spell (amountToCast = 6)
    for (int i = 0; i < amountToCast; i++)
    {
        // Predict vị trí player nếu đang di chuyển
        bool playerMoving = playerScript.rb.linearVelocity.magnitude > 0;
        float xOffset = playerMoving ? playerOffsetPrediction.x * playerScript.facingDirection : 0;
        Vector3 spellPosition = player.transform.position + new Vector3(xOffset, playerOffsetPrediction.y);
        
        Enemy_ReaperSpell spell = Instantiate(spellCastPrefab, spellPosition, Quaternion.identity)
                                  .GetComponent<Enemy_ReaperSpell>();
        
        spell.SetupSpell(combat, spellDamageScale);
        
        yield return new WaitForSeconds(spellCastRate); // 1.2s giữa các spell
    }
    
    SetSpellCastPreformed(true);
}

public bool CanDoSpellCast() => Time.time > lastTimeCastedSpells + spellCastStateCooldown; // 10s cooldown
```

**Reaper Spell**:

**File**: `Assets/Scripts/Enemy/Boss/Enemy_ReaperSpell.cs`

```csharp
public class Enemy_ReaperSpell : MonoBehaviour
{
    private Entity_Combat combat;
    private DamageScaleData damageScaleData;
    
    public void SetupSpell(Entity_Combat combat, DamageScaleData damageScaleData)
    {
        this.combat = combat;
        this.damageScaleData = damageScaleData;
        Destroy(gameObject, 2f); // Tự hủy sau 2s
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            // Sử dụng custom damage scale
            combat.PerformAttackOnTarget(collision.transform, damageScaleData);
            DisableCollider(); // Chỉ damage 1 lần
        }
    }
}
```

---

## 🛡️ COUNTER SYSTEM

### ICounterable Interface

```csharp
public interface ICounterable
{
    bool CanBeCountered { get; }
    void HandleCounter();
}
```

### Counter Window

**File**: `Assets/Scripts/Enemy/Enemy_AnimationTriggers.cs`

```csharp
private void EnableCounterWindow()
{
    enemy.EnableCounterWindow(true); // canBeStunned = true
    enemy_VFX.EnableAttackAlert(true); // Hiển thị VFX cảnh báo
}

private void DisableCounterWindow()
{
    enemy.EnableCounterWindow(false);
    enemy_VFX.EnableAttackAlert(false);
}
```

### Stunned State

**File**: `Assets/Scripts/Enemy/Enemy_States/Enemy_StunnedState.cs`

```csharp
public class Enemy_StunnedState : EnemyState
{
    public override void Enter()
    {
        base.Enter();
        
        vfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);
        
        stateTimer = enemy.stunnedDuration; // 1s mặc định
        
        // Đẩy enemy lùi lại với velocity
        rb.linearVelocity = new Vector2(
            enemy.stunnedVelocity.x * -enemy.facingDirection, 
            enemy.stunnedVelocity.y
        );
    }
    
    public override void Update()
    {
        base.Update();
        
        if (stateTimer <= 0f)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
```

---

## 🎯 PROJECTILE SYSTEM

### Base Projectile Pattern

Tất cả projectile đều follow pattern này:

1. **Setup**: Nhận target và combat reference
2. **Movement**: Di chuyển theo physics hoặc tính toán
3. **Collision Detection**: `OnTriggerEnter2D` để detect hit
4. **Damage**: Gọi `combat.PerformAttackOnTarget()`
5. **Cleanup**: Disable collider, play animation, destroy

### Arrow vs Projectile vs Spell

| Type | Movement | Collision | Special |
|------|----------|-----------|---------|
| **Arrow** | Linear velocity (8 units/s) | Stuck vào target | Có thể counter |
| **Mage Projectile** | Ballistic trajectory | Explode animation | Arc height = 2 |
| **Reaper Spell** | Static spawn tại vị trí | AOE damage | Predict player position |

---

## 📊 STATS & DAMAGE SYSTEM

### Entity_Stats Component

**File**: `Assets/Scripts/Entity/Entity_Stats.cs`

#### GetAttackData():

```csharp
public AttackData GetAttackData(DamageScaleData scaleData)
{
    return new AttackData(this, scaleData);
}
```

#### AttackData Structure:

```csharp
public class AttackData
{
    public float physicalDamage;
    public float elementalDamage;
    public ElementType element;
    public bool isCrit;
    public StatusEffectData effectData;
    
    // Tính toán từ:
    // - stats.offense.damage
    // - stats.offense.critChance
    // - stats.offense.critDamage
    // - Elemental damage (Fire/Ice/Lightning)
}
```

#### Elemental Damage Calculation:

```csharp
public float GetElementalDamage(out ElementType element, float scaleFactor = 1f)
{
    float fireDamage = offense.fireDamage.GetValue();
    float iceDamage = offense.iceDamage.GetValue();
    float lightningDamage = offense.lightningDamage.GetValue();
    
    // Bonus từ Intelligence: mỗi điểm INT = +1 elemental damage
    float bonusElementalDamage = major.intelligence.GetValue() * 1f;
    
    // Tìm element có damage cao nhất
    float highestElementalDamage = fireDamage;
    element = ElementType.Fire;
    
    if (iceDamage > highestElementalDamage)
    {
        highestElementalDamage = iceDamage;
        element = ElementType.Ice;
    }
    
    if (lightningDamage > highestElementalDamage)
    {
        highestElementalDamage = lightningDamage;
        element = ElementType.Lightning;
    }
    
    // Element yếu hơn chỉ tính 50% damage
    float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * 0.5f;
    float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * 0.5f;
    float bonusLightning = (element == ElementType.Lightning) ? 0 : lightningDamage * 0.5f;
    
    float weakerElementalDamage = bonusFire + bonusIce + bonusLightning;
    float finalElementalDamage = highestElementalDamage + weakerElementalDamage + bonusElementalDamage;
    
    return finalElementalDamage * scaleFactor;
}
```

---

## ⚙️ ATTACK SPEED SYNCHRONIZATION

### SyncAttackSpeed()

**File**: `Assets/Scripts/StateMachine/EntityState.cs`

```csharp
public void SyncAttackSpeed()
{
    float attackSpeed = stats.offense.attackSpeed.GetValue();
    animator.SetFloat("attackSpeedMultiplier", attackSpeed);
}
```

- Được gọi trong `Enemy_AttackState.Enter()`
- Sync animation speed với attack speed stat
- Animation Controller sử dụng `attackSpeedMultiplier` để điều chỉnh tốc độ animation

---

## 🎨 ANIMATION SYSTEM

### Animation Triggers

**File**: `Assets/Scripts/Enemy/Enemy_AnimationTriggers.cs`

Các animation events được gọi từ Animation Controller:

- `EnableCounterWindow()`: Mở cửa sổ counter
- `DisableCounterWindow()`: Đóng cửa sổ counter
- `SpecialAttackTrigger()`: Gọi `enemy.SpecialAttack()`
- `AnimationTrigger()`: Set `triggerCalled = true` để chuyển state

### Animation Parameters

Các parameters được update mỗi frame trong `UpdateAnimationParameters()`:

- `battleAnimSpeedMultiplier`: Tỷ lệ giữa battle speed và move speed
- `moveAnimSpeedMultiplier`: Multiplier cho move animation
- `xVelocity`: Vận tốc theo trục X
- `attackSpeedMultiplier`: Tốc độ tấn công (sync với stat)

---

## 🔍 PLAYER DETECTION SYSTEM

### PlayerDetected()

**File**: `Assets/Scripts/Enemy/Enemy.cs`

```csharp
public RaycastHit2D PlayerDetected()
{
    RaycastHit2D hit = Physics2D.Raycast(
        playerCheck.position, 
        Vector2.right * facingDirection, 
        playerCheckDistance, // 10f
        whatIsPlayer | groundLayer
    );
    
    if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
    {
        return default;
    }
    return hit;
}
```

- Sử dụng **Raycast2D** để detect player
- Range: `playerCheckDistance = 10f`
- Chỉ detect khi có line of sight (không bị chặn bởi ground)

---

## 📝 TÓM TẮT KỸ THUẬT

### Design Patterns:

1. **State Machine Pattern**: Mỗi enemy có state machine riêng
2. **Component Pattern**: Combat, Stats, Health là các component riêng
3. **Interface Pattern**: `ICounterable` để xử lý counter
4. **Object Pooling**: (Có thể implement cho projectiles)

### Performance Optimizations:

1. **Raycast2D**: Sử dụng cho player detection (nhẹ hơn OverlapCircle)
2. **LayerMask**: Filter collision hiệu quả
3. **State Caching**: Cache player reference để tránh tìm lại
4. **Animation Parameters**: Update mỗi frame thay vì mỗi state change

### Code Quality:

1. **Separation of Concerns**: Mỗi class có trách nhiệm rõ ràng
2. **Polymorphism**: Override `SpecialAttack()` cho từng enemy
3. **Event System**: `OnDoingPhysicalDamage` event cho items
4. **Null Checks**: Luôn check null trước khi sử dụng

---

**Tài liệu này được tạo tự động từ code analysis. Cập nhật: 2025**
