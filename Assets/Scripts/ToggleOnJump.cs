using UnityEngine;

public class ToggleOnJump : MonoBehaviour
{

    public bool toggleOnJump = true;
    [SerializeField] private GameObject targetObject;

    private void OnEnable()
    {
        PlayerController.OnPlayerJump += ToggleObject;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerJump -= ToggleObject;
    }

    private void ToggleObject()
    {
        toggleOnJump = !toggleOnJump;
        if (targetObject == null) return;
        if (toggleOnJump)
        {
            targetObject.SetActive(true);
        }
        else
        {
            targetObject.SetActive(false);
        }
    }
}