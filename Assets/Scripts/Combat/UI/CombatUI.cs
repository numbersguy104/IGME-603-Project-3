using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Utility;

public class CombatUI : SingletonBehavior<CombatUI>
{
    [SerializeField] private TMP_Text currentTurn;
    [SerializeField] private Button attackButton;
    private UnityAction attackAction = () => PlayerController_Combat.Instance.Attack();
    [SerializeField] private Button moveButton;
    private UnityAction moveAction = () => PlayerController_Combat.Instance.RequestMove();
    [SerializeField] private SkillPanelUI skillPanelUI;
    //[SerializeField] private Image playerHPBar;
    [SerializeField] private Transform playerStatuses;
    //[SerializeField] private Image enemyHPBar;
    [SerializeField] private Transform enemyStatuses;
    [SerializeField] private Button endTurnButton;
    private UnityAction endTurnAction = () => PlayerController_Combat.Instance.EndPlayerTurn();
    [SerializeField] private Image characterBackground;
    [SerializeField] private GameObject characterBackgroundPlaceholder;
    [SerializeField] private Button hugoButton;
    private UnityAction hugoAction = () => Instance.ChangeCharacter(Characters.HUGO);
    [SerializeField] private Button tenetButton;
    private UnityAction tenetAction = () => Instance.ChangeCharacter(Characters.TENET);
    [SerializeField] private GameObject healthDisplayPrefab;
    [SerializeField] private GameObject[] tooltips;

    public UnityEvent OnCombatInfoUpdated;
    public UnityEvent OnNotifiedWin;
    public UnityEvent OnNotifiedLose;
    
    
    public void NotifyWin() => OnNotifiedWin?.Invoke();
    public void NotifyLose() => OnNotifiedLose?.Invoke();
    
    private void OnEnable()
    {
        attackButton.onClick.AddListener(attackAction);
        moveButton.onClick.AddListener(moveAction);
        endTurnButton.onClick.AddListener(endTurnAction);
        
        hugoButton.onClick.AddListener(hugoAction);
        tenetButton.onClick.AddListener(tenetAction);
    }

    private void OnDisable()
    {
        attackButton.onClick.RemoveListener(attackAction);
        moveButton.onClick.RemoveListener(moveAction);
        endTurnButton.onClick.RemoveListener(endTurnAction);
        
        hugoButton.onClick.RemoveListener(hugoAction);
        tenetButton.onClick.RemoveListener(tenetAction);
    }

    public void Init(List<PlayerCharacter_Combat> playerCharacters, List<Enemy_Combat> enemyCharacters)
    {
        //Create health bars
        foreach (PlayerCharacter_Combat player in CombatManager.Instance.playerCharacters_Combat)
        {
            HealthBarUI bar = Instantiate(healthDisplayPrefab, playerStatuses).GetComponent<HealthBarUI>();
            bar.SetTarget(player);
            bar.UpdateHealthValue(player.CurrentHealth, player.MaxHealth);
            bar.UpdateIcon(player.entity.characterImage.sprite);

        }
        foreach (Enemy_Combat enemy in CombatManager.Instance.enemies_Combat)
        {
            HealthBarUI bar = Instantiate(healthDisplayPrefab, enemyStatuses).GetComponent<HealthBarUI>();
            bar.SetTarget(enemy);
            bar.UpdateHealthValue(enemy.CurrentHealth, enemy.MaxHealth);
            bar.UpdateIcon(enemy.entity.characterImage.sprite);
        }

        UpdateCombatInfo();

        foreach (var character in playerCharacters)
        {
            character.OnTakeDamage += UpdateCombatInfo;   
            character.OnStatusUpdated += UpdateCombatInfo;
        }
        foreach (var enemy in enemyCharacters)
        {
            enemy.OnTakeDamage += UpdateCombatInfo;
            enemy.OnStatusUpdated += UpdateCombatInfo;
        }
    }

    public void UpdateCombatInfo()
    {
        UpdateHP();
        UpdateStatus();
        OnCombatInfoUpdated?.Invoke();

        PlayerCharacter_Combat currentCharacter = PlayerController_Combat.Instance.currentCharacter;
        moveButton.interactable = currentCharacter.movesAvailable > 0.01f;

        //Combat UI guidance
        hugoButton.GetComponent<Outline>().enabled = false;
        tenetButton.GetComponent<Outline>().enabled = false;
        endTurnButton.GetComponent<Outline>().enabled = false;

        if (currentCharacter.attacksAvailable == 0)
        {
            if (PlayerController_Combat.Instance.currentCharacterName == Characters.HUGO
                && PlayerController_Combat.Instance.teamMembers[1].attacksAvailable > 0)
            {
                tenetButton.GetComponent<Outline>().enabled = true;
            }
            else if (PlayerController_Combat.Instance.currentCharacterName == Characters.TENET
                && PlayerController_Combat.Instance.teamMembers[0].attacksAvailable > 0)
            {
                hugoButton.GetComponent<Outline>().enabled = true;
            }
            else
            {
                endTurnButton.GetComponent<Outline>().enabled = true;
            }
        }
    }

    public void UpdateHP()
    {
        void UpdateCharHealth(Character_Combat character, int characterIndex, Transform teamStatuses)
        {
            HealthBarUI health = teamStatuses.GetChild(characterIndex).GetComponent<HealthBarUI>();
            health.UpdateHealthValue(character.CurrentHealth, character.MaxHealth);
        }

        for (int i = 0; i < CombatManager.Instance.playerCharacters_Combat.Count; i++)
        {
            UpdateCharHealth(CombatManager.Instance.playerCharacters_Combat[i], i, playerStatuses);
        }
        for (int i = 0; i < CombatManager.Instance.enemies_Combat.Count; i++)
        {
            UpdateCharHealth(CombatManager.Instance.enemies_Combat[i], i, enemyStatuses);
        }
    }

    public void UpdateStatus()
    {
        void UpdateCharStatus(Character_Combat character, int characterIndex, Transform teamStatuses)
        {
            StatusSlotUI[] statusSlots = teamStatuses.GetChild(characterIndex).GetComponentsInChildren<StatusSlotUI>();
            List<Status> statuses = new List<Status>();
            foreach (var status in character.statuses)
            {
                if(!status.statusData.hideInStatusUI)
                    statuses.Add(status);
            }
            for (int i = 0; i < statusSlots.Length; i++)
            {
                if (i < statuses.Count)
                {
                    statusSlots[i].SetVisible(true);
                    statusSlots[i].UpdateStatus(statuses[i]);
                }
                else
                {
                    statusSlots[i].SetVisible(false);
                }
            }
        }

        for (int i = 0; i < CombatManager.Instance.playerCharacters_Combat.Count; i++)
        {
            PlayerCharacter_Combat player = CombatManager.Instance.playerCharacters_Combat[i];
            UpdateCharStatus(player, i, playerStatuses);
        }
        for (int i = 0; i < CombatManager.Instance.enemies_Combat.Count; i++)
        {
            Enemy_Combat enemy = CombatManager.Instance.enemies_Combat[i];
            UpdateCharStatus(enemy, i, enemyStatuses);
        }
        
        // Update available Moves
        moveButton.interactable = (PlayerController_Combat.Instance.currentCharacter.movesAvailable > 0);
        attackButton.interactable = (PlayerController_Combat.Instance.currentCharacter.attacksAvailable > 0);
    }

    public void SwitchTurn(bool isPlayerTurn)
    {
        if(isPlayerTurn)
            currentTurn.SetText($"Player's Turn");
        else
            currentTurn.SetText($"Enemy's Turn");
    }

    //Commented out for now since there's only one character implemented
    public void ChangeCharacter(Characters character)
    {
         //If trying to switch to the already active character, do nothing
         PlayerCharacter_Combat activeCharacter = PlayerController_Combat.Instance.currentCharacter;
         List<PlayerCharacter_Combat> team = PlayerController_Combat.Instance.teamMembers;
         if ((character == Characters.HUGO && activeCharacter == team[0]) ||
             (character == Characters.TENET && activeCharacter == team[1]))
             return;

         PlayerController_Combat.Instance.SwitchCharacter();

         if (character == Characters.HUGO)
         {
             characterBackground.color = new Color(0.5f, 0.0f, 0.0f);
         }
         else
         {
             characterBackground.color = new Color(0.0f, 0.5f, 0.375f);
         }
         
         OnCombatInfoUpdated?.Invoke();
    }

    public void SetPanelVisible(bool visible)
    {
        characterBackground.gameObject.SetActive(visible);
        characterBackgroundPlaceholder.SetActive(!visible);
    }

    public void EndSelecting()
    {
        PlayerController_Combat.Instance.EndAllSelecting();
    }

    public void HideTooltips()
    {
        foreach (GameObject tooltip in tooltips)
        {
            tooltip.SetActive(false);
        }
    }
}
