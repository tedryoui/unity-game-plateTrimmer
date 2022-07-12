using System;

[Serializable]
public class ResourcesViewController
{
    public ResourcesView view;

    public void UpdatePlatesAmount(int amount)
    {
        view.platesAmount.text = $"{amount}x";
    }
}
