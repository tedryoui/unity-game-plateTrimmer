using System;
using UnityEditor.UIElements;
using UnityEngine;

[Serializable]
public class ProgressController
{
    public ProgressView view;

    public Gradient progressColor;

    public void SetPercentage(int percentage)
    {
        view.amount.text = $"{percentage}%";
        view.amount.color = progressColor.Evaluate(percentage / 100f);
    }
}
