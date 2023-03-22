using UnityEngine;

public class HyperlinkButton : MonoBehaviour
{
    public void OpenHyperlink(string url)
    {
        Application.OpenURL(url);
    }
}