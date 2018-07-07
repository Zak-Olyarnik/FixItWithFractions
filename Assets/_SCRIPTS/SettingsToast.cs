using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsToast : MonoBehaviour
{
    [SerializeField] private Text toast;
    [SerializeField] private RectTransform backing;

    private void OnMouseOver()
    {
        Debug.Log("toast");
        toast.gameObject.SetActive(true);

    }

    private void OnMouseExit()
    {
        toast.gameObject.SetActive(false);
    }

    public void TurnOn(string s)
    {
        toast.text = s;
        Vector2 cursorPos = new Vector2(Input.mousePosition.x - 1920/2, Input.mousePosition.y - 1080/2);
        backing.localPosition = cursorPos;
        toast.gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        toast.gameObject.SetActive(false);
    }
}
