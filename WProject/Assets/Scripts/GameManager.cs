using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    internal Player player;

    public EnemyBot[] enemies;

    public float timer;

    //TEST
    public Dropdown testDropDown;

    private void Awake() {

        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(Instance);
            Instance = gameObject.GetComponent<GameManager>();
            return;
        }

        player = FindObjectOfType<Player>();
    }

    private void Start() {

        #region TEST
        List<string> list = new List<string>();
        for (int i = 0; i < (int)ESpell.COUNT; i++) {
            list.Add(((ESpell)i).ToString());
        }
        testDropDown.AddOptions(list);
        #endregion

    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.E)) {
            player.stats.TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            player.stats.armor.AddModifier(10);
        }
    }

    //TEST
    public void PlayTestCard() {

        string s = testDropDown.options[testDropDown.value].text;
        for (int i = 0; i < (int)ESpell.COUNT; i++) {
            ESpell eSpell = (ESpell)i;

            if (s == eSpell.ToString()) {
                Spell spell = SpellBuilder.CreateSpell(eSpell);
                player.PlayCard(spell, enemies[0]);
                return;
            }
        }
    }
}



