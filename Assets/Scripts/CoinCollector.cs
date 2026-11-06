using TMPro;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    private int coinCount = 0;

    private void Start()
    {
        UpdateCoinText();
    }

    public void CollectCoin()
    {
        coinCount++;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinText == null)
        {
            return;
        }

        coinText.text = "Coins: " + coinCount + " / 6";
    }
}
