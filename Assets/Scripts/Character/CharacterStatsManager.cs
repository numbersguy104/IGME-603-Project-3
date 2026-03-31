using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CharacterStatsManager : SingletonBehaviorDontDestroy<CharacterStatsManager>
{
    public PlayerCharacter hugo;
    public PlayerCharacter tenet;
    public List<PlayerCharacter> characters = new List<PlayerCharacter>();
    // [SerializeField] private PlayerData hugoData;
    // [SerializeField] private PlayerData tenetData;

    protected override void Init()
    {
        base.Init();
        hugo = new PlayerCharacter(Resources.Load<PlayerData>("Character/Hugo"));
        tenet = new PlayerCharacter(Resources.Load<PlayerData>("Character/Tenet"));
        characters.Clear();
        characters.Add(hugo);
        characters.Add(tenet);
    }
}
