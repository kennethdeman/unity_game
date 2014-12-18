using UnityEngine;
using System.Collections;
using System;

public class HealthPotion : IComparable<HealthPotion>, Item
{
    const float HEALTH = 33f;
    const float MAGIC = 0f;

    private Texture2D m_Texture = (Texture2D)Resources.Load("MyGUIhealthPotion");

    public HealthPotion()
    {
        //Debug.LogError("HealthPotion created");
    }

    public Texture2D GetTexture()
    {
        //Debug.LogError("Draw HealthPotion");
        return m_Texture;
    }

    public void UseItem()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        Player.GetComponent<PlayerController>().AddHealth(HEALTH);
        Player.GetComponent<PlayerController>().AddMagicPower(MAGIC);
        Player.GetComponent<PlayerController>().SpawnHealthPotionParticles();
    }

    public int CompareTo(HealthPotion other)
    {
        return 0;
    }

    public int CompareTo(MagicPotion other)
    {
        return 0;
    }
}