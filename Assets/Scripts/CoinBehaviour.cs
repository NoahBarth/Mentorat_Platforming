using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * 180f * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Coin Triggered by " + other.name);
        CoinCollector coinCollector = other.GetComponentInParent<CoinCollector>();
        coinCollector.CollectCoin();
        Destroy(gameObject);
    }
}
