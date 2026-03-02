using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct StatusWithTurns
{
    public StatusData status;
    public int turns;
}
[CreateAssetMenu(fileName = "Status", menuName = "ScriptableObject/Status")]
public class StatusData : ScriptableObject
{
    public string statusName;
    public Sprite statusIcon;

    [RuntimeInitializeOnLoadMethod]
    void Register()
    {
        StatusFactory.RegisterStatus(statusName);
    }
}