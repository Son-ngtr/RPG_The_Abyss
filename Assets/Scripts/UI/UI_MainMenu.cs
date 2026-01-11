using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    private void Start()
    {
        transform.root.GetComponentInChildren<UI_Options>(true).LoadupVolume();
        transform.root.GetComponentInChildren<UI_FadeScreen>().DoFadeIn(1f);

        AudioManager.instance.StartBGM("playlist_mainMenu");
    }

    public void PlayButton()
    {
        AudioManager.instance.PlayGlobalSFX("button_click");
        GameManager.instance.ContinuePlay();
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
