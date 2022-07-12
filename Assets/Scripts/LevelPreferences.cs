using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Preferences", menuName = "ScriptableObjects/LevelPrefs", order = 1)]
public class LevelPreferences : ScriptableObject
{
    public GameObject floorPrefab;
    public Vector2 floorSizes;
    public int platesAmount;
    public float plateSpeed;
}
