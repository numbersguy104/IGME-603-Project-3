using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI hugoTimeText;
    [SerializeField] private TextMeshProUGUI tenetTimeText;
    [SerializeField] private TextMeshProUGUI hugoPercentText;
    [SerializeField] private TextMeshProUGUI tenetPercentText;
    [SerializeField] private LayoutElement hugoBar;
    [SerializeField] private LayoutElement tenetBar;
    [SerializeField] private TextMeshProUGUI skillUseText;


    void Start()
    {
        if (CharacterStatsManager.Instance != null)
        {
            expText.text = "You earned " + DataTracker.Instance.expEarned + " total EXP and reached level " + CharacterStatsManager.Instance.hugo.level + ".";

            float hugoTime = DataTracker.Instance.hugoTime;
            float tenetTime = DataTracker.Instance.tenetTime;
            float totalTime = hugoTime + tenetTime;
            if (totalTime > 0.0f)
            {
                hugoTimeText.text = "You spent " + (int)hugoTime + " seconds in combat controlling Hugo.";
                tenetTimeText.text = "You spent " + (int)tenetTime + " seconds in combat controlling Tenet.";

                float hugoTimePortion = hugoTime / totalTime;
                float tenetTimePortion = tenetTime / totalTime;
                hugoBar.flexibleWidth = hugoTimePortion;
                tenetBar.flexibleWidth = tenetTimePortion;

                hugoPercentText.text = Mathf.RoundToInt(hugoTimePortion * 100.0f) + "%";
                tenetPercentText.text = Mathf.RoundToInt(tenetTimePortion * 100.0f) + "%";
            }

            string skillString = "";
            foreach (KeyValuePair<string, int> skillCount in DataTracker.Instance.GetSkillUses().OrderByDescending(i => i.Value))
            {
                skillString += skillCount.Key + ": " + skillCount.Value + "\n";
            }
            skillUseText.text = skillString;

            if (!Directory.Exists("Data Tracking"))
            {
                Directory.CreateDirectory("Data Tracking");
            }
            ScreenCapture.CaptureScreenshot("Data Tracking/data-" + System.DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss") + ".png");
        }
    }
}
