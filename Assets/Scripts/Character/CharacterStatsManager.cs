using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CharacterStatsManager : MonoBehaviour
{
    public static CharacterStatsManager Instance;

    public PlayerCharacter hugo;
    public PlayerCharacter tenet;
    public List<PlayerCharacter> characters = new List<PlayerCharacter>();
    [SerializeField] private PlayerData hugoData;
    [SerializeField] private PlayerData tenetData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            hugo = new PlayerCharacter(hugoData);
            tenet = new PlayerCharacter(tenetData);
            characters.Add(hugo);
            characters.Add(tenet);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
