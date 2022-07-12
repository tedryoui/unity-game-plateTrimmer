using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level level;
    
    public Floor floor;
    public Plate plate;
    public GUIHandler gui;
    public Player player;

    public List<LevelPreferences> availablePreferences = new List<LevelPreferences>();
    [HideInInspector] public LevelPreferences preferences;

    public void ResetLevel()
    {
        ResetPreferences();
        player.ResetPlayer();
        floor.ResetFloor();
        gui.ResetGUI();
        plate.ShowNext();
    }

    private void ResetPreferences() => preferences = availablePreferences[Random.Range(0, availablePreferences.Count)];

    private void Awake()
    {
        level = this;
        ResetPreferences();
    }
}
