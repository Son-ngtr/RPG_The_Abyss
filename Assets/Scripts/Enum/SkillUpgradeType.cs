using UnityEngine;

public enum SkillUpgradeType
{
    // KHI DUNG THI THEM BIEN VAO CUOI, NEU KHONG SE BI LOI DO EDITOR SU DUNG BIEN ENUM THEO THU TU :)))
        // Gia su khi them bien None vao thi bien su dung dash on stash se thanh dash ...

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
    Shard_TeleportHpRewind,          // When you coming back to shard, you get same hp % as you had when you created shard

    // ------- Sword Tree -------
    SwordThrow,                     // Throw a sword that damage enemies on its way
    SwordThrow_Spin,                // The sword will spin, dealing damage to all enemies it passes through
    SwordThrow_Pierce,              // The sword pierces through enemies, hitting multiple targets in a line
    SwordThrow_Bounce,              // The sword bounces off walls and can hit enemies multiple times
}
