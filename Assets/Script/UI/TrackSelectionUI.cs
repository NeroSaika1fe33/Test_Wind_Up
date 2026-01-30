using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

public class TrackSelectionUI : MonoBehaviour
{
    private string[] trackNames = new string[] { "SampleTrack", "Track01", "Track02" };
    [SerializeField] private int selectedTrackID = 0;
    [SerializeField] private TextMeshProUGUI TrackNameText;
    [SerializeField] private Image TrackImage;

    [Header("Gamepad/Stick")]
    [SerializeField] private float stickThreshold = 0.5f;   // stick 半分くらい->入力
    [SerializeField] private float navCooldown = 0.15f;      // COOLDOWN
    private float nextNavTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectedTrackID = 2;
        ApplyUI();
        TrackNameText.text = $"{ResourceManager.Instance.TrackDefs[selectedTrackID].trackName}";
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) Debug.Log("PAD SOUTH");
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame) Debug.Log("PAD EAST");
        if (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame) Debug.Log("PAD R1");

        // Confirm: Space / Enter / Gamepad A
        UpdateSelection();
        if (SubmitPressed())
        {
            SetSelectedTrack();
        }
    }

    private void SetSelectedTrack()
    {
        GameManager.Instance.SetCurrentTrackByID(ResourceManager.Instance.TrackDefs[selectedTrackID].trackID);
    }

    private void UpdateSelection()
    {
        int delta = ReadUpDownOnce(); // -1 = up, +1 = down, 0 = none
        if (delta == 0) return;

        selectedTrackID += delta;

       
        if (selectedTrackID > 2) selectedTrackID = 0;
        else if (selectedTrackID < 0) selectedTrackID = 2;

        ApplyUI();
    }

    private void ApplyUI()
    {
        if (TrackNameText != null)
            TrackNameText.text = $"{ResourceManager.Instance.TrackDefs[selectedTrackID].trackName}";

        if (TrackImage != null)
            TrackImage.sprite = ResourceManager.Instance.TrackDefs[selectedTrackID].trackIcon;
    }

    // ===== Input helpers (Keyboard + Gamepad) =====

    
    private int ReadUpDownOnce()
    {
        float now = Time.unscaledTime;
        if (now < nextNavTime) return 0;

        // Keyboard：W/S 或 ↑/↓
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return -1;
            }
            if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return +1;
            }
        }

        // Gamepad：Dpad 優先的，後、 Stick
        var g = Gamepad.current;
        if (g != null)
        {
            if (g.dpad.up.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return -1;
            }
            if (g.dpad.down.wasPressedThisFrame)
            {
                nextNavTime = now + navCooldown;
                return +1;
            }

            float y = g.leftStick.y.ReadValue();
            if (y >= stickThreshold)
            {
                nextNavTime = now + navCooldown;
                return -1;
            }
            if (y <= -stickThreshold)
            {
                nextNavTime = now + navCooldown;
                return +1;
            }
        }

        return 0;
    }

    private bool SubmitPressed()
    {
        bool kb = Keyboard.current != null &&
              (Keyboard.current.spaceKey.wasPressedThisFrame ||
               Keyboard.current.enterKey.wasPressedThisFrame ||
               Keyboard.current.numpadEnterKey.wasPressedThisFrame);

        bool pad = AnyGamepadConfirmPressedThisFrame();


        return kb || pad;
    }

    private bool AnyGamepadConfirmPressedThisFrame()
    {
        var g = Gamepad.current;
        if (g == null) return false;

        
        if (g.buttonSouth.wasPressedThisFrame || g.buttonEast.wasPressedThisFrame || g.startButton.wasPressedThisFrame)
            return true;

        
        foreach (var c in g.allControls)
        {
            if (c is ButtonControl b && b.wasPressedThisFrame)
            {
                if (c == g.dpad.up || c == g.dpad.down || c == g.dpad.left || c == g.dpad.right)
                    continue;

                return true;
            }
        }
        return false;
    }
}
