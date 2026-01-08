using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Refund all skills", fileName = "Item effect data - Refund all skills")]
public class ItemEffect_RefundAllSkills : ItemEffect_DataSO
{


    public override void ExecuteEffect()
    {
        /*UI_SkillTree skillTree = FindFirstObjectByType<UI_SkillTree>(FindObjectsInactive.Include);
        skillTree.RefundAllSkills();*/

        UI ui = FindFirstObjectByType<UI>();
        ui.skillTreeUI.RefundAllSkills();
    }
}
