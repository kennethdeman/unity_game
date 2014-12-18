using UnityEngine;

public interface Quest
{
    string GetObjectiveText();
    void CheckObjectives();
    bool Completed();
}