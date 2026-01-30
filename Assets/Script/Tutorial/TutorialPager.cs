using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    public Button backToTitleButton;   
    public Button goToInGameButton;

    [Header("Stick Settings")]
    public float stickThreshold = 0.5f;
    public float navCooldown = 0.18f;   

    int pageIndex = 0;
    int lineIndex = 0;
    bool running = false;
    
    Rigidbody carRb;

    Button[] endButtons;
    int endIndex = 0;
    float nextNavTime = 0f;
    void Awake()
    {
        if (endChoicePanel != null) endChoicePanel.SetActive(false);

        if (backToTitleButton != null && goToInGameButton != null)
            endButtons = new[] { backToTitleButton, goToInGameButton };
    }

    void Start()
    {
        if (tutorialRoot != null) tutorialRoot.SetActive(true);
        if (endChoicePanel != null) endChoicePanel.SetActive(false);
        BeginTutorial();
    }


    void Update()
    {
        if (!running)
        {
            HandleEndChoice();
            return;
        }

        if (allowSkipWithX && Input.GetKeyDown(skipKey))
        {
            EndTutorialShowChoice();
            return;
        }

        if (Input.GetKeyDown(nextKey1) || Input.GetKeyDown(nextKey2))
        {
            Advance();
            return;
        }

        // gamepad next / skip
        var g = Gamepad.current;
        if (g != null)
        {
            // A next page
            if (g.buttonSouth.wasPressedThisFrame)
            {
                Advance();
                return;
            }
            // B skip   
            if (allowSkipWithX && g.buttonEast.wasPressedThisFrame)
            {
                EndTutorialShowChoice();
                return;
            }
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

        endIndex = 0;
        SelectEndButton();
    }

    void HandleEndChoice()
    {
        if (endChoicePanel == null || !endChoicePanel.activeSelf) return;
        if (endButtons == null || endButtons.Length == 0) return;

        int lr = ReadLeftRightOnce();
        if (lr != 0)
        {
            endIndex = (endIndex + lr + endButtons.Length) % endButtons.Length;
            SelectEndButton();
        }

        if (SubmitPressed())
        {
            endButtons[endIndex].onClick.Invoke();
        }
    }

    void SelectEndButton()
    {
        if (EventSystem.current != null && endButtons != null && endButtons.Length > 0)
            EventSystem.current.SetSelectedGameObject(endButtons[endIndex].gameObject);
    }

    bool SubmitPressed()
    {
        bool kb = Keyboard.current != null &&
                  (Keyboard.current.enterKey.wasPressedThisFrame ||
                   Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                   Keyboard.current.spaceKey.wasPressedThisFrame);
        bool pad = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
        return kb || pad;
    }

    int ReadLeftRightOnce()
    {
        float now = Time.unscaledTime;
        if (now < nextNavTime) return 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return -1;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return +1;
            }
        }

        var g = Gamepad.current;
        if (g != null)
        {
            if (g.dpad.left.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return -1;
            }
            if (g.dpad.right.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return +1;
            }

            float x = g.leftStick.x.ReadValue();
            if (x <= -stickThreshold)
            {
                nextNavTime = now + navCooldown;
                return -1;
            }
            if (x >= stickThreshold)
            {
                nextNavTime = now + navCooldown;
                return +1;
            }
        }

        return 0;
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
