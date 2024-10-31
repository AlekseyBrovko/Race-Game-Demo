using UnityEngine;
using UnityEngine.UI;

public class CarStatIcon : MonoBehaviour
{
    [SerializeField] private Image[] _levelsImages;

    private Vector4 _notBuyedColor = new Vector4(91f / 255f, 91f / 255f, 91f / 255f, 255f / 255f);
    private Vector4 _buyedColor = new Vector4(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

    public void ShowLevelValue(int level)
    {
        for (int i = 0; i < _levelsImages.Length; i++)
        {
            Image iconImage = _levelsImages[i];
            if (i > level - 1)
                ShowNotBuyedImage(iconImage);
            else
                ShowBuyedImage(iconImage);
        }
    }

    private void ShowNotBuyedImage(Image levelImage) =>
        levelImage.color = _notBuyedColor;

    private void ShowBuyedImage(Image levelImage) =>
        levelImage.color = _buyedColor;
}