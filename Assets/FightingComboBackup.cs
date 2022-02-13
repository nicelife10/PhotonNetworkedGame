using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
public enum AttackType { heavy = 0, light = 1, kick = 2}

public class FightingComboBackup : MonoBehaviour
{
    [Header("Inputs")]
    public KeyCode heavyKey;
    public KeyCode lightKey;
    public KeyCode KickKey;

    [Header("Attacks")]
    public Attack heavyAttack;
    public Attack lightAttack;
    public Attack KickAttack;

    [Header("Combos")]
    public List<Combo> combos;
    public float comboLeeway = 0.2f;

    [Header("Components")]
    public Animator ani;

    public Attack currAttack = null;
    ComboInput lastInput = null;
    List<int> currentCombos = new List<int>();
    float timer = 0;
    float leeway = 0;
    bool skip = false;
    // Start is called before the first frame update
    void Start()
    {
        if(ani == null)
        {
            ani = GetComponent<Animator>();
        }
        PrimeCombos();
    }

    void PrimeCombos()
    {
        for(int i=0; i< combos.Count; i++)
        {
            Combo c = combos[i];
            c.onInputted.AddListener(() =>
            {
                //Call attack function with combo's attack
                skip = true;
                Attack(c.comboAttack);
                ResetCombos();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (currAttack != null)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                currAttack = null;
            }

            return;
        }

        if (currentCombos.Count > 0)
        {
            leeway += Time.deltaTime;
            if (leeway >= comboLeeway)
            {
                if (lastInput != null)
                {
                    Attack(getAttackFromType(lastInput.type));
                    lastInput = null;
                }

                ResetCombos();
            }
        }
        else
            leeway = 0;


        ComboInput input = null;

        if (Input.GetKeyDown(heavyKey))
            input = new ComboInput(AttackType.heavy);

        if (Input.GetKeyDown(lightKey))
            input = new ComboInput(AttackType.light);

        if (Input.GetKeyDown(KickKey))
            input = new ComboInput(AttackType.kick);


        if (input == null) return;
        lastInput = input;

        List<int> remove = new List<int>();
        for(int i = 0; i< currentCombos.Count; i++)
        {
            Combo c = combos[currentCombos[i]];
            if(c.continueCombo(input))
            {
                leeway = 0;
                //Do something
            }
            else
            {
                remove.Add(i);
            }
        }


        if (skip)
        {
            skip = false;
            return;
        }



        for (int i = 0; i< combos.Count; i++)
        {
            if (currentCombos.Contains(i)) continue;
            if (combos[i].continueCombo(input))
            {
                currentCombos.Add(i);
                leeway = 0;
            }
        }

        foreach (int i in remove)
            currentCombos.RemoveAt(i);

        if(currentCombos.Count <= 0)
        {
            Attack(getAttackFromType(input.type));
        }

    }


    void ResetCombos()
    {
        for (int i = 0; i < currentCombos.Count; i++)
        {
            Combo c = combos[currentCombos[i]];
            c.ResetCombo();
        }
        currentCombos.Clear();
    }

    void Attack(Attack att)
    {
        currAttack = att;
        timer = att.length;
        ani.Play(att.name, -1, 0);
    }
    Attack getAttackFromType(AttackType t)
    {
        if (t == AttackType.heavy)
            return heavyAttack;
        if (t == AttackType.light)
            return lightAttack;
        if (t == AttackType.kick)
            return KickAttack;

        return null;
    }
}

[System.Serializable]
public class Attack
{
    public string name;
    public float length;

}

[System.Serializable]
public class ComboInput
{
    public AttackType type;

    public ComboInput(AttackType t)
    {
        type = t;
    }

    public bool isSameAs(ComboInput test)
    {
        return (type == test.type);
    }

}
[System.Serializable]
public class Combo
{
    public string Name;
    public List<ComboInput> Inputs;
    public Attack comboAttack;
    public UnityEvent onInputted;    
    int currInput = 0;

    public bool continueCombo(ComboInput i)
    {
        if(Inputs[currInput].isSameAs(i))
        {
            currInput++;
            if(currInput >= Inputs.Count)// Finished the input and we should do the attack
            {
                onInputted.Invoke();
                currInput = 0;
            }
            return true;
        }
        else
        {
            currInput = 0;
            return false;
        }
    }

    public ComboInput currentComboInput()
    {
        if (currInput >= Inputs.Count)  return null;
        return Inputs[currInput];
    }

    public void ResetCombo()
    {
        currInput = 0;
    }
}

    */