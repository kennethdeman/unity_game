using UnityEngine;
using System.Collections;
using System;

public class MagicPotion : IComparable<MagicPotion>, Item
{
    const float HEALTH = 0f;
    const float MAGIC = 33f;

    private Texture2D m_Texture = (Texture2D)Resources.Load("MyGUImanaPotion");

    public MagicPotion()
    {
        //Debug.LogError("MagicPotion created");
    }

    public Texture2D GetTexture()
    {
        //Debug.LogError("Draw MagicPotion");
        return m_Texture;
    }

    public void UseItem()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        Player.GetComponent<PlayerController>().AddHealth(HEALTH);
        Player.GetComponent<PlayerController>().AddMagicPower(MAGIC);
        Player.GetComponent<PlayerController>().SpawnMagicPotionParticles();
    }

    public int CompareTo(MagicPotion other)
    {
        return 0;
    }

    public int CompareTo(HealthPotion other)
    {
        return 0;
    }
}