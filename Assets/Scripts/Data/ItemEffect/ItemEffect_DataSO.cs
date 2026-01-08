using UnityEngine;

public class ItemEffect_DataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    
    protected Player player;

    public virtual bool CanBeUsed()
    {
        return true;
    }

    public virtual void ExecuteEffect()
    {
        Debug.Log("Base Item Effect Executed");
    }

    public virtual void SubScribeToPlayerEvents(Player player)
    {
        this.player = player;
    }

    public virtual void UnSubScribeToPlayerEvents(Player player)
    {
        this.player = null;
    }
}
