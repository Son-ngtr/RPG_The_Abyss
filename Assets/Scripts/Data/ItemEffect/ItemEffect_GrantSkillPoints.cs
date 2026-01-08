using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Grant Skill Points", fileName = "Item effect data - Grant Skill Points")]

public class ItemEffect_GrantSkillPoints : ItemEffect_DataSO
{
    [SerializeField] private int skillPointsToGrant;


    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();

        ui.skillTreeUI.AddSkillPoints(skillPointsToGrant);
    }
}
