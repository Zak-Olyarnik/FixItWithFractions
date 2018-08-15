using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsMenu : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject menu;

    //private float changeInterval = 5f;
    private int index;

    public void NextImage()
    {
        index++;
        if(index >= sprites.Length)
        {
            //CancelInvoke();
            menu.SetActive(true);
            gameObject.SetActive(false);
            return;
        }
        image.sprite = sprites[index];
    }

    public void PrevImage()
    {
        index--;
        if (index < 0)
        {
            //CancelInvoke();
            menu.SetActive(true);
            gameObject.SetActive(false);
            return;
        }
        image.sprite = sprites[index];
    }

    void OnEnable()
    {
        index = 0;
        image.sprite = sprites[index];
        //InvokeRepeating("UpdateImage", changeInterval, changeInterval);
    }
}