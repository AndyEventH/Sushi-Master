using TMPro;
using UnityEngine;

public class ClientsServedTextUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProText;

    private void OnEnable()
    {
        UpdateText(GameManager.Instance.RoundCount);
        GameManager.Instance.OnRoundCountChanged += HandleRoundCountChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRoundCountChanged -= HandleRoundCountChanged;
    }

    private void HandleRoundCountChanged(int newCount)
    {
        UpdateText(newCount);
    }

    private void UpdateText(int count)
    {
        textMeshProText.text = count.ToString();
    }
}
