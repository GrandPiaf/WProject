using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBot : Character {


    public override void ResetCharacter() {
        base.ResetCharacter();
        attackDelay *= 1 - FindObjectOfType<GameManager>().score / 100;
    }

    // Update is called once per frame
    override public void Update() {
        base.Update();
            ESpell eSpell = (ESpell)Random.Range(0, (int)ESpell.COUNT);
            Spell spell = SpellBuilder.CreateSpell(eSpell);
            bool success = PlayCard(spell, GameManager.Instance.player);
            if (success) Debug.Log("Spell played : " + eSpell.ToString());
    }
}
