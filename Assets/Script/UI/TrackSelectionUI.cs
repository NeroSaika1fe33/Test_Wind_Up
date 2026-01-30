using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectionUI : MonoBehaviour
{
    private string[] trackNames = new string[] { "SampleTrack", "Track01", "Track02" };
    [SerializeField] private int selectedTrackID = 0;
    [SerializeField] private TextMeshProUGUI TrackNameText;
    [SerializeField] private Image TrackImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectedTrackID = 2;
        TrackNameText.text = $"{ResourceManager.Instance.TrackDefs[selectedTrackID].trackName}";
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
        if (Input.GetKeyDown(KeyCode.Space))
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
        if (Input.GetKeyUp(KeyCode.S))
            selectedTrackID++;
        if (Input.GetKeyUp(KeyCode.W))
            selectedTrackID--;

        if (selectedTrackID > 2)
            selectedTrackID = 0;
        else if (selectedTrackID < 0)
            selectedTrackID = 2;

        TrackNameText.text = $"{ResourceManager.Instance.TrackDefs[selectedTrackID].trackName}";
        TrackImage.sprite = ResourceManager.Instance.TrackDefs[selectedTrackID].trackIcon;
    }
}
