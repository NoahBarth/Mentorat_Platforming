using UnityEngine;

public class DropPlatform : MonoBehaviour
{
    bool activated = false;

    public void Activate()
    {
        if (activated) return;

        activated = true;
        Invoke(nameof(HidePlatform), 0.5f);
    }

    void HidePlatform()
    {
        gameObject.SetActive(false);
        Invoke(nameof(ShowPlatform), 7f);
    }

    void ShowPlatform()
    {
        gameObject.SetActive(true);
        activated = false;
    }
}
