using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject pressAnyKeyHint;
    [SerializeField] private GameObject StartTuto;
    [SerializeField] private GameObject Hint;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Stick Settings")]
     private float stickThreshold = 0.5f;
     private float navCooldown = 0.18f;


    private Button[] buttons;
    private int index = 0;
    private bool inPrompt = false;
    private bool justOpenedPrompt = false;
    private float nextNavTime = 0f;
    void Start()
    {

        buttons = new[] { yesButton, noButton };

        if (StartTuto != null) StartTuto.SetActive(false);
        if (pressAnyKeyHint != null) pressAnyKeyHint.SetActive(true);
        if (Hint != null) Hint.SetActive(true);


        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => LevelManager.Instance.OnClickTutorial());

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => LevelManager.Instance.OnClickSelect());
    }

    void Update()
    {
      
        if (!inPrompt)
        {
            if (Input.anyKeyDown)
            {
                OpenPrompt();
            }
            return;
        }

        if (justOpenedPrompt)
        {
            justOpenedPrompt = false;
            return;
        }

        // 2)左右選択（Keyboard A/D/←→；Gamepad Dpad/Stick）
        int lr = ReadLeftRightOnce();
        if (lr != 0)
        {
            index = (index + lr + buttons.Length) % buttons.Length;
            SelectCurrent();
        }

        // 3) Confirm（Keyboard Enter/Space；Gamepad A）
        if (SubmitPressed())
        {
            buttons[index].onClick.Invoke();
        }

        // 4) Cancel（Keyboard Esc；Gamepad B）
        if (CancelPressed())
        {
            ClosePrompt();
        }
    }

    private void OpenPrompt()
    {
        inPrompt = true;
        justOpenedPrompt = true;

        if (pressAnyKeyHint != null) pressAnyKeyHint.SetActive(false);
        if (StartTuto != null) StartTuto.SetActive(true);
        if (Hint != null) Hint.SetActive(true);

        index = 0;
        SelectCurrent();
    }

    private void ClosePrompt()
    {
        inPrompt = false;

        if (StartTuto != null) StartTuto.SetActive(false);
        if (pressAnyKeyHint != null) pressAnyKeyHint.SetActive(true);
        if (Hint != null) Hint.SetActive(false);


        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void SelectCurrent()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }

    private bool AnyStartPressed()
    {
        // Keyboard any key
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) return true;

        // Gamepad any common buttons
        var g = Gamepad.current;
        if (g == null) return false;

        return g.buttonSouth.wasPressedThisFrame ||
               g.buttonEast.wasPressedThisFrame ||
               g.buttonWest.wasPressedThisFrame ||
               g.buttonNorth.wasPressedThisFrame ||
               g.startButton.wasPressedThisFrame ||
               g.selectButton.wasPressedThisFrame ||
               g.dpad.left.wasPressedThisFrame || g.dpad.right.wasPressedThisFrame ||
               g.dpad.up.wasPressedThisFrame || g.dpad.down.wasPressedThisFrame ||
               g.leftShoulder.wasPressedThisFrame || g.rightShoulder.wasPressedThisFrame ||
               g.leftTrigger.wasPressedThisFrame || g.rightTrigger.wasPressedThisFrame;
    }

    private bool SubmitPressed()
    {
        bool kb = Keyboard.current != null &&
                  (Keyboard.current.enterKey.wasPressedThisFrame ||
                   Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                   Keyboard.current.spaceKey.wasPressedThisFrame);

        bool pad = Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;

        return kb || pad;
    }

    private bool CancelPressed()
    {
        bool kb = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool pad = Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;
        return kb || pad;
    }

    private int ReadLeftRightOnce()
    {
        float now = Time.unscaledTime;
        if (now < nextNavTime) return 0;

        // Keyboard
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

        // Gamepad Dpad
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

            // Gamepad stick (edge-ish)
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
}
