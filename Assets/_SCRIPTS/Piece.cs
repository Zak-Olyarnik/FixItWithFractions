using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    // all of these things to be toggled on and off or changed opacity
    [SerializeField] GameObject draggable;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteRenderer label;
    [SerializeField] SpriteRenderer countBacking;
    [SerializeField] Text text;

    private GameController gc;
    private bool interactable = true;

    public bool Interactable
    {
        get { return interactable; }
    }

    public void EnableDraggable()
    {
        draggable.SetActive(true);
    }

    public void SetInteractable(bool b)
    {
        if (b && !interactable)    // setting to true when false
        {
            interactable = true;
            if (gc.ActiveCursor == Constants.CursorType.HAND)
            {
                draggable.SetActive(true);
            }

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
            label.color = new Color(label.color.r, label.color.g, label.color.b, 1f);
            countBacking.color = new Color(countBacking.color.r, countBacking.color.g, countBacking.color.b, 1f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }
        else if (!b && interactable)    // setting to false when true
        {
            interactable = false;
            draggable.SetActive(false);

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
            label.color = new Color(label.color.r, label.color.g, label.color.b, 0.5f);
            countBacking.color = new Color(countBacking.color.r, countBacking.color.g, countBacking.color.b, 0.5f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.5f);
        }
    }

    private void Start()
    {
        gc = GameController.Instance;
        sprite.color = Constants.trackColor;
    }
}
