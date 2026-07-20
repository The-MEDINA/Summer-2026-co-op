using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialPage
    {
        [TextArea(3, 8)]
        public string description;

        public Sprite tutorialImage;
    }

    [Header("Tutorial Pages")]
    [SerializeField] private TutorialPage[] pages;

    [Header("UI References")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text pageNumberText;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text nextButtonText;

    [Header("Scene Names")]
    [SerializeField] private string titleSceneName = "TitleScreen";
    [SerializeField] private string practiceSceneName = "Demo_LocalTwoPlayer";

    private int currentPageIndex;

    private void Start()
    {
        currentPageIndex = 0;
        ShowCurrentPage();
    }

    public void NextPage()
    {
        if (pages == null || pages.Length == 0)
        {
            return;
        }

        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            ShowCurrentPage();
        }
        else
        {
            StartPracticeMatch();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowCurrentPage();
        }
    }

    public void ReturnToTitleScreen()
    {
        SceneManager.LoadScene(titleSceneName);
    }

    public void StartPracticeMatch()
    {
        SceneManager.LoadScene(practiceSceneName);
    }

    private void ShowCurrentPage()
    {
        if (pages == null || pages.Length == 0)
        {
            Debug.LogWarning("No tutorial pages have been added.");
            return;
        }

        TutorialPage currentPage = pages[currentPageIndex];

        if (descriptionText != null)
        {
            descriptionText.text = currentPage.description;
        }

        if (pageNumberText != null)
        {
            pageNumberText.text =
                $"{currentPageIndex + 1} / {pages.Length}";
        }

        if (tutorialImage != null)
        {
            tutorialImage.sprite = currentPage.tutorialImage;
            tutorialImage.enabled = currentPage.tutorialImage != null;
        }

        if (previousButton != null)
        {
            previousButton.interactable = currentPageIndex > 0;
        }

        if (nextButtonText != null)
        {
            bool isLastPage = currentPageIndex == pages.Length - 1;
            nextButtonText.text = isLastPage
                ? "Start Practice"
                : "Next";
        }
    }
}