using Fusion;
using UnityEngine;

public class FlamePuzzlePlayerState : NetworkBehaviour
{
    public enum FlameColor
    {
        None = -1,
        Red = 0,
        Green = 1,
        Blue = 2
    }

    [Networked]
    public bool hasFlame { get; set; }

    [Networked]
    public int flameColor { get; set; }

    [Header("Flame Visuals")]
    public GameObject redFlame;
    public GameObject greenFlame;
    public GameObject blueFlame;

    public override void Render()
    {
        UpdateFlameVisuals();
    }

    public void AssignFlame(FlameColor newColor)
    {
        hasFlame = true;
        flameColor = (int)newColor;

        UpdateFlameVisuals();
    }

    public void RemoveFlame()
    {
        hasFlame = false;
        flameColor = (int)FlameColor.None;

        UpdateFlameVisuals();
    }

    private void UpdateFlameVisuals()
    {
        if (redFlame != null)
            redFlame.SetActive(false);

        if (greenFlame != null)
            greenFlame.SetActive(false);

        if (blueFlame != null)
            blueFlame.SetActive(false);

        if (!hasFlame)
            return;

        switch ((FlameColor)flameColor)
        {
            case FlameColor.Red:
                if (redFlame != null)
                    redFlame.SetActive(true);
                break;

            case FlameColor.Green:
                if (greenFlame != null)
                    greenFlame.SetActive(true);
                break;

            case FlameColor.Blue:
                if (blueFlame != null)
                    blueFlame.SetActive(true);
                break;
        }
    }
}