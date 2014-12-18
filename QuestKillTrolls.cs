using UnityEngine;
using System.Collections;
using System;

public class QuestKillTrolls : IComparable<QuestKillTrolls>, Quest
{

    const int HASTOKILL = 7;
    private int m_hasKilled = 0;

    private bool m_Completed = false;

    public string GetObjectiveText()
    {
        return "Kill all the trolls:  \n \t - " + m_hasKilled + "/" + HASTOKILL + " trolls killed";
    }

    public void CheckObjectives()
    {
        var objects = GameObject.FindGameObjectsWithTag("Troll");
        var objectCount = objects.Length;
        foreach (var obj in objects)
        {
            if (obj != null && m_hasKilled < HASTOKILL)
            {

                if (obj.GetComponent<TrollController>().CheckIfDead())
                {
                    //Debug.LogError("checked");
                    m_hasKilled += 1;
                }
            }
        }

        if (m_hasKilled >= HASTOKILL)
        {
            m_Completed = true;
        }

    }

    public bool Completed()
    {
        return m_Completed;
    }

    public int CompareTo(QuestKillTrolls other)
    {
        return 0;
    }

    public static string GetQuestText()
    {
        return "Wait don't kill me! Forgive us. We were only following orders. We heard some troubling reports about you and we were to kill you on sight by the superiors. Maybe you can redeem yourself by completing the task which we were sent to complete. This area is overrun by trolls, kill 7 of them to show them they're not welcome here.";
    }

    public static string GetQuestTextPending()
    {
        return "Have you killed the trolls yet?";
    }

    public static string GetQuestTextComplete()
    {
        return "You have killed the trolls? Good. Here is your reward!";
    }

}
