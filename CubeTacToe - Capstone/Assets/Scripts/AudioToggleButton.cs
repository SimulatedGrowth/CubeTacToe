using UnityEngine;
using UnityEngine.UI;

public class AudioToggleButton : MonoBehaviour
{
    public Sprite unmutedSprite;     
    public Sprite mutedSprite;       
    public Image buttonImage;
    public GameObject howToPlayPanel;
    public GameObject ChallengePanel;


    private bool isMuted = false;

    private void Start()
    {
        
        isMuted = AudioListener.volume == 0;
        UpdateButtonImage();
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;

        AudioListener.volume = isMuted ? 0 : 1;

        UpdateButtonImage();
    }

    private void UpdateButtonImage()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = isMuted ? mutedSprite : unmutedSprite;
        }
    }
    public void ShowPanel()
    {
        howToPlayPanel.SetActive(true);
        
    }

    public void HidePanel()
    {
        howToPlayPanel.SetActive(false);
        
    }

    public void TogglePanel()
    {
        howToPlayPanel.SetActive(!howToPlayPanel.activeSelf);
    }

    public void CShowPanel()
    {
        ChallengePanel.SetActive(true);

    }

    public void CHidePanel()
    {
        ChallengePanel.SetActive(false);

    }

    public void CTogglePanel()
    {
        ChallengePanel.SetActive(!ChallengePanel.activeSelf);
    }
}
