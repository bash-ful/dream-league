using UnityEngine;
using UnityEngine.UI;

public static class ImageTransparencyScript
{
    public static void UpdateImageTransparency(Image image)
    {
        if (image.sprite == null)
        {
            image.color = Color.clear;
        }
        else
        {
            image.color = Color.white;
        }
    }
}
