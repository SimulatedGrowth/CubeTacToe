using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Sprite unmutedSprite;     
    public Sprite mutedSprite;       
    public Image buttonImage;
    public GameObject howToPlayPanel;
    public GameObject ChallengePanel;
    public GameObject bulletModePanel;
    public Animator animator;
    public Animator animator1;
    public Animator animator2;
    public string animationTrigger = "Click";

    private bool hasPlayed = false;
    private bool hasPlayed1 = false;
    private bool isMuted = false;

    private void Start()
    {
        
        isMuted = AudioListener.volume == 0;
        UpdateButtonImage();
    }
    
    public void ShowBulletModePanel()
    {
        if (hasPlayed1) return;
        hasPlayed1 = true;
        bulletModePanel.SetActive(true);
        animator2.SetTrigger(animationTrigger);

    }
    public void HideBulletModePanel()
    {
        hasPlayed1 = false;
        bulletModePanel.SetActive(false);
        animator2.Rebind();
        animator2.Update(0f);
    }

    public void TriggerMenu()
    {
        if (hasPlayed) return;
        hasPlayed = true;
        howToPlayPanel.SetActive(true);
        animator.SetTrigger(animationTrigger);
        
    }
    public void CloseMenu()
    {
        hasPlayed = false;
        howToPlayPanel.SetActive(false);
        animator.Rebind();               
        animator.Update(0f);
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

        if (hasPlayed) return;
        hasPlayed = true;
        ChallengePanel.SetActive(true);
        animator1.SetTrigger(animationTrigger);
        

    }

    public void CHidePanel()
    {

        hasPlayed = false;
        ChallengePanel.SetActive(false);
        animator1.Rebind();
        animator1.Update(0f);

    }

    public void CTogglePanel()
    {
        ChallengePanel.SetActive(!ChallengePanel.activeSelf);
    }
}
