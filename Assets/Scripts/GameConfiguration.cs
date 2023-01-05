using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
    [Header("Timing")]
    public float CameraTransitionTime = 1.0f;

    [Header("Game Metrics")]
    public float MinShootingDistance = 0.15f;
    public float MaxShootingDistance = 1.0f;

    [Header("Indicators")]
    public int TargetingIndicatorCount = 10;
    public GameObject TargetingIndicator;
    public HUDOverheadUI Overhead;
}
