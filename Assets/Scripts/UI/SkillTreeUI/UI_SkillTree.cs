using System.Linq;
using TMPro;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private int skillPoints;
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [SerializeField] private UI_TreeConnectionHandler[] parentNodes;

    private UI_TreeNode[] allTreeNodes;

    public Player_SkillManager skillManager {  get; private set; }

    private void Start()
    {
        UpdateAllConnection();
        UpdateSkillPointsUI();
    }

    private void UpdateSkillPointsUI()
    {
        skillPointsText.text = skillPoints.ToString();
    }

    public void UnlockDefaultSkills()
    {
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);
        skillManager = FindAnyObjectByType<Player_SkillManager>();

        foreach (var node in allTreeNodes)
        {
            node.UnlockDefaultSkill();
        }
    }

    [ContextMenu("Reset Skill Tree")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
        {
            node.Refund();
        }
    }

    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;

    public void RemoveSkillPoint(int cost)
    {
        skillPoints -= cost;
        UpdateSkillPointsUI();
    }

    public void AddSkillPoints(int points)
    {
        skillPoints += points;
        UpdateSkillPointsUI();
    }


    [ContextMenu("Update All Connections")]
    public void UpdateAllConnection()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }


    // SAVE LOAD SYSTEM
    public void LoadData(GameData data)
    {
        skillPoints = data.skillPoints;

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;

            if (data.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)
            {
                // Unlock the node without deducting skill points
                node.UnlockWithSaveData();
            }
        }

        foreach (var skill in skillManager.allSkills)
        {
            if (data.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType))
            {
                var upgradeNode = allTreeNodes.FirstOrDefault(n => n.skillData.upgradeData.upgradeType == upgradeType); // Find the node with the saved upgrade type

                if (upgradeNode != null)
                {
                    skill.SetSkillUpgrade(upgradeNode.skillData); // Set skill to the saved upgrade
                }
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.skillPoints = skillPoints;
        data.skillTreeUI.Clear();
        data.skillUpgrades.Clear();

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;
            data.skillTreeUI[skillName] = node.isUnlocked;
        }

        foreach (var skill in skillManager.allSkills)
        {
            data.skillUpgrades[skill.GetSkillType()] = skill.GetCurrentUpgradeType();
        }
    }
}
