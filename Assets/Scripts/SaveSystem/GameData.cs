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

    public Vector3 savedCheckPoint;

    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
        storageItems = new SerializableDictionary<string, int>();
        storageMaterials = new SerializableDictionary<string, int>();

        equippedItems = new SerializableDictionary<string, ItemType>();
        skillTreeUI = new SerializableDictionary<string, bool>();
        skillUpgrades = new SerializableDictionary<SkillType, SkillUpgradeType>();
    }
}
