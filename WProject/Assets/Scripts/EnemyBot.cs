using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBot : Character {

    public float attackDelay = 5f;
    float timer = 0f;

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        if (timer >= attackDelay) {
            timer = 0;
            ESpell eSpell = (ESpell)Random.Range(0, (int)ESpell.COUNT);
            Spell spell = SpellBuilder.CreateSpell(eSpell);
            bool success = PlayCard(spell, GameManager.Instance.player);
            if (success) Debug.Log("Spell played : " + eSpell.ToString());
        }
    }
}
