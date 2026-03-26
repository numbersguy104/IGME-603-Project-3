using System.Collections.Generic;
using UnityEngine;

public class SkillSettingUI : MonoBehaviour
{
    private List<SkillData> skillLearned;
    private List<SkillData> skillEquipped;

    public void GetSkills(bool isHugo)
    {
        skillLearned = (isHugo ? CharacterStatsManager.Instance.hugo : CharacterStatsManager.Instance.tenet).skillLearned;
        skillEquipped = (isHugo ? CharacterStatsManager.Instance.hugo : CharacterStatsManager.Instance.tenet).skillLearned;
    }
    public void UpdateSkill(bool isHugo)
    {
        GetSkills(isHugo);
    }
}
