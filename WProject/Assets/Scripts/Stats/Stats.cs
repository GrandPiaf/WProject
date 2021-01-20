using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
	public Stat maxHealth;          // Maximum amount of health
	public int currentHealth { get; protected set; }    // Current amount of health

	public Stat damage;
	public Stat armor;
	public Stat mana;
	public ESpellForce shield;

	public event System.Action OnHealthReachedZero;

	public virtual void Awake()
	{
		currentHealth = maxHealth.GetValue();
	}

	
	// Damage the character
	public void TakeDamage(int damage)
	{

		int armorDamage = Mathf.Min(armor.GetValue(), damage);
		int healthDamage = Mathf.Min(currentHealth, damage - armorDamage);
		armor.AddModifier(-armorDamage);
		currentHealth -= healthDamage;
		Debug.Log(transform.name + " takes " + armorDamage + " armor damage and " + healthDamage + " health damage");

		// If we hit 0. Die.
		if (currentHealth <= 0)
		{
			if (OnHealthReachedZero != null)
			{
				OnHealthReachedZero();
			}
		}
	}

	// Heal the character.
	public void Heal(int amount)
	{
		currentHealth += amount;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.GetValue());
	}
}
