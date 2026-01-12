using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int gold;

    public SerializableDictionary<string, int> inventory; // itemSaveID -> stackSize

    public SerializableDictionary<string, int> storageItems;

    public SerializableDictionary<string, int> storageMaterials;

    public SerializableDictionary<string, ItemType> equippedItems; // itemSaveId -> slotType

    public SerializableDictionary<string, bool> skillTreeUI; // SkillName -> Unlock status
    public int skillPoints;
    public SerializableDictionary<SkillType, SkillUpgradeType> skillUpgrades; // skillType -> upgradeType 

    public SerializableDictionary<string, bool> unlockedCheckPoints; // checkpointID -> unlocked status

    public SerializableDictionary<string, Vector3> inScenePortals; // sceneName -> portalPosition
    public string portalDestinationSceneName;
    public bool returningFromTown;

    public string lastScenePlayedName;
    public Vector3 lastPlayerPosition;

    // QUEST
    public SerializableDictionary<string, bool> completedQuests; // questsaveID -> completed status
    public SerializableDictionary<string, int> activeQuests; // questsaveID -> progressAmount

    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        equippedItems = new SerializableDictionary<string, ItemType>();
        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();

        unlockedCheckPoints = new SerializableDictionary<string, bool>();

        inScenePortals = new SerializableDictionary<string, Vector3>();

        completedQuests = new SerializableDictionary<string, bool>();
        activeQuests = new SerializableDictionary<string, int>();
    }
}
