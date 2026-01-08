using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class TutorialPage
{
    public Sprite pageImage;
    [TextArea(2, 6)]
    public string[] lines;
}

public class TutorialPager : MonoBehaviour
{
    [Header("Refs")]
    public GameObject tutorialRoot;   
    public Image pageImage;
    public TMP_Text dialogueText;
    public TMP_Text hintText;

    [Header("Gameplay Refs")]
    public CarController car;
    public QTEController qte;

    [Header("Pages")]
    public TutorialPage[] pages;

    [Header("Input")]
    public KeyCode nextKey1 = KeyCode.Z;
    public KeyCode nextKey2 = KeyCode.Return;

    [Header("Skip")]
    public bool allowSkipWithX = true;
    public KeyCode skipKey = KeyCode.X;
   

    [Header("Debug")]
    public bool forceShowTutorial = false;
    public bool resetTutorialDoneOnStart = false;

    [Header("Save")]
    public string tutorialDoneKey = "TutorialDone";
    public bool markDoneWhenFinished = true;

    [Header("End Choice UI")]
    public GameObject endChoicePanel; 
    

    int pageIndex = 0;
    int lineIndex = 0;
    bool running = false;
    
    Rigidbody carRb;

    void Awake()
    {
        if (endChoicePanel != null) endChoicePanel.SetActive(false);
    }

    void Start()
    {
        if (tutorialRoot != null) tutorialRoot.SetActive(true);
        if (endChoicePanel != null) endChoicePanel.SetActive(false);
        BeginTutorial();
    }


    void Update()
    {
        if (!running) return;

        if (allowSkipWithX && Input.GetKeyDown(skipKey))
        {
            EndTutorialShowChoice();
            return;
        }

        if (Input.GetKeyDown(nextKey1) || Input.GetKeyDown(nextKey2))
        {
            Advance();
        }
    }

    void BeginTutorial()
    {
        running = true;

        if (endChoicePanel != null) endChoicePanel.SetActive(false);

        pageIndex = 0;
        lineIndex = 0;
        RenderCurrent();
    }

    void Advance()
    {
        if (pages == null || pages.Length == 0)
        {
            EndTutorialShowChoice();
            return;
        }

        var p = pages[pageIndex];
        if (p.lines == null) p.lines = Array.Empty<string>();

        
        lineIndex++;

        
        if (lineIndex < p.lines.Length)
        {
            RenderCurrent();
            return;
        }

       
        pageIndex++;
        lineIndex = 0;

        
        if (pageIndex >= pages.Length)
        {
            EndTutorialShowChoice();
            return;
        }

        RenderCurrent();
    }

    void RenderCurrent()
    {
        if (pages == null || pages.Length == 0) return;

        pageIndex = Mathf.Clamp(pageIndex, 0, pages.Length - 1);
        var p = pages[pageIndex];

        if (pageImage != null) pageImage.sprite = p.pageImage;

        string line = "";
        if (p.lines != null && p.lines.Length > 0)
        {
            int idx = Mathf.Clamp(lineIndex, 0, p.lines.Length - 1);
            line = p.lines[idx];
        }

        if (dialogueText != null) dialogueText.text = line;

        if (hintText != null)
        {
            hintText.text = allowSkipWithX
                ? "Z/Enter: next   X: skip"
                : "Z/Enter: next";
        }
    }


    void EndTutorialShowChoice()
    {
        running = false;

        if (markDoneWhenFinished)
        {
            PlayerPrefs.SetInt(tutorialDoneKey, 1);
            PlayerPrefs.Save();
        }

        
        if (endChoicePanel != null) endChoicePanel.SetActive(true);
    }

   

    public void OnClickBackToTitle()
    {
        LevelManager.Instance.LoadScene(SceneList.Title);
    }

    public void OnClickGoToInGame()
    {
        LevelManager.Instance.LoadScene(SceneList.Car_Selection);
    }

    
}
