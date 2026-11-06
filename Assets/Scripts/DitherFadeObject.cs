using UnityEngine;

public class DitherFadeObject : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propertyBlock;

    private static readonly int FadeAmount =
        Shader.PropertyToID("_FadeAmount");

    private float currentFade = 1f;
    private float targetFade = 1f;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // PropertyBlock einmal erstellen
        propertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        currentFade = Mathf.MoveTowards(
            currentFade,
            targetFade,
            Time.deltaTime * 3f);

        rend.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(FadeAmount, currentFade);
        rend.SetPropertyBlock(propertyBlock);
    }

    public void SetFade(float value)
    {
        targetFade = value;
    }
}