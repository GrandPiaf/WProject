using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Slider sliderHealth;
    [SerializeField] Slider sliderArmor;

    [SerializeField] GameObject interactionPanel;
    [SerializeField] TextMeshProUGUI interactionText;
    [SerializeField] TextMeshProUGUI bulletMagazineText;
    [SerializeField] TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = gameObject.GetComponent<UIManager>();
            return;
        }
    }

    private void Start()
    {
        interactionPanel.SetActive(false);
    }

    private void Update()
    {
        sliderHealth.value = GameManager.Instance.player.stats.currentHealth;
        sliderArmor.value = GameManager.Instance.player.stats.armor.GetValue();
    }

    public void ShowInteractionPanel(string displayText)
    {
        interactionPanel.SetActive(true);
        interactionText.text = displayText;
    }

    public void HideInteractionPanel()
    {
        interactionPanel.SetActive(false);
    }
}
