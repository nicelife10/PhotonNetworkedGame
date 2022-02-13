using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AttackType { RightPunch = 0, RightKick = 1, LeftPunch = 2, LeftKick = 3, SpecialAttack = 4}

public class FightingCombo : MonoBehaviour
{
    [Header("Inputs")]
    public KeyCode LeftPunchKey;
    public KeyCode LeftKickKey;
    public KeyCode RightPunchKey;
    public KeyCode RightKickKey;
    public KeyCode SpecialAttackKey;

    [Header("Attacks")]
    public Attack LeftPunchAttack;
    public Attack LeftKickAttack;
    public Attack RightPunchAttack;
    public Attack RightKickAttack;
    public Attack SpecialAttack;

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

        if (Input.GetKeyDown(LeftPunchKey))
            input = new ComboInput(AttackType.LeftPunch);

        if (Input.GetKeyDown(LeftKickKey))
            input = new ComboInput(AttackType.LeftKick);

        if (Input.GetKeyDown(RightPunchKey))
            input = new ComboInput(AttackType.RightPunch);

        if (Input.GetKeyDown(RightKickKey))
            input = new ComboInput(AttackType.RightKick);

        if (Input.GetKeyDown(SpecialAttackKey))
            input = new ComboInput(AttackType.SpecialAttack);


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
        if (t == AttackType.LeftPunch)
            return LeftPunchAttack;
        if (t == AttackType.LeftKick)
            return LeftKickAttack;

        if (t == AttackType.RightPunch)
            return RightPunchAttack;
        if (t == AttackType.RightKick)
            return RightKickAttack;

        if (t == AttackType.SpecialAttack)
            return SpecialAttack;

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

