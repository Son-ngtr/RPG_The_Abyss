using UnityEngine;

public enum SkillUpgradeType
{
    None,

    // ------- Dash Tree -------
    Dash,                           // Dash to avoid damage
    Dash_CloneOnStash,              // Create a clone when dash starts
    Dash_CloneOnStartAndArrival,    // Create a clone when dash starts and ends
    Dash_ShardOnStart,              // Create a shard when dash starts
    Dash_ShardOnStartAndArrival,    // Create a shard when dash starts and ends

    // ------- Shard Tree -------
    Shard,                          // The shard explodes when touched by an enemy or time goes
    Shard_MoveToEnemy,              // Shard will move towards closest enemy
    Shard_MultiCast,               // Shard ability have up to N casts, and you can cast in a row
    Shard_Teleport,                 // You can press button again, to change places with shard you created
    Shard_TeleportAndHeal,          // When you coming back to shard, you get same hp % as you had when you created shard
}
