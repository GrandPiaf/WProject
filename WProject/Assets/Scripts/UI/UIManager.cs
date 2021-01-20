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
        
    }

    private void Update()
    {
        sliderHealth.value = GameManager.Instance.player.stats.currentHealth;
        sliderArmor.value = GameManager.Instance.player.stats.armor.GetValue();
    }
}
