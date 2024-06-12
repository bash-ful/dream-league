using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    private Image buttonImage;
    private Sprite buttonImageOrigSprite;

    void Start() {
        buttonImage = GetComponent<Image>();
        buttonImageOrigSprite = buttonImage.sprite;
    }

    public void changeSprite(Sprite hoverSprite) {
        buttonImage.sprite = hoverSprite;
    }

    public void revertSprite() {
        buttonImage.sprite = buttonImageOrigSprite;
    }

}
