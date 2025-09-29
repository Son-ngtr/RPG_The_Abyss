using UnityEngine;

public class Player_AnimationTriggers : MonoBehaviour
{

    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void CurrentStateTrigger()
    {
        // access the Player component and call a method to signal that the attack is over
        player.CallAnimationTrigger();
    }
}
