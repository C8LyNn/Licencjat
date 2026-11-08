using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public string unitLevel;

    public int damage;
    public int defense;

    public int maxHP;
    public int currentHP;

    public TextMeshPro nameText;
    public TextMeshPro healthText;



    private void Start()
    {
        nameText.text = unitName;
        healthText.text = currentHP + "/" + maxHP;
        BattleSystem.updateHealth += CheckHealth;
    }

    private void OnDestroy()
    {
        BattleSystem.updateHealth -= CheckHealth;
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckHealth()
    {
        healthText.text = currentHP + "/" + maxHP;

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
