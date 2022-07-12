using System;
using Scripts.GUI.LevelComplete;
using UnityEditor.UIElements;
using UnityEngine;

public class GUIHandler : MonoBehaviour
{
    public ProgressController progress;
    public ResourcesViewController resources;
    public LevelCompleteController levelComplete;

    public void ResetGUI()
    {
        progress.SetPercentage(0);
    }
}
