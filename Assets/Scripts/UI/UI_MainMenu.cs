using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    private void Start()
    {
        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn(1f);
    }

    public void PlayButton()
    {
        GameManager.instance.ContinuePlay();
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
