using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUI : MonoBehaviour {

    [SerializeField] Slider sliderHealth;
    [SerializeField] Slider sliderArmor;
    [SerializeField] Slider sliderMana;
    [SerializeField] Image shieldSprite;

    [SerializeField] Character character;

    [SerializeField] Sprite[] shieldIcons;


    private void Start() {
        if (shieldIcons.Length < 4)
            Debug.LogError("4 Shield icons must be referenced");
    }

    private void Update() {
        sliderHealth.value = (int)(100f * character.stats.currentHealth / character.stats.maxHealth.baseValue);
        sliderArmor.value = character.stats.armor.GetValue();
        sliderMana.value = 100* character.mana/character.manaMax;
        UpdateShieldDisplay();
    }

    private void UpdateShieldDisplay() {
        switch (character.shield) {
            case ESpellForce.None:
                shieldSprite.sprite = shieldIcons[0];
                break;
            case ESpellForce.Light:
                shieldSprite.sprite = shieldIcons[1];
                break;
            case ESpellForce.Simple:
                shieldSprite.sprite = shieldIcons[2];
                break;
            case ESpellForce.Strong:
                shieldSprite.sprite = shieldIcons[3];
                break;
            default:
                break;

        }
    }
}
