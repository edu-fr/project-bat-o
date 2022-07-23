    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Game;
    using Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    namespace UI
    {
        public class LevelUpMenu : MonoBehaviour
        {
            /* Level up texts from Json */
            private TextAsset levelUpTextsJson;
            [Serializable]
            public class LevelUpOptionList
            {
                public LevelUpOption[] LevelUpOptions;
            }
            public LevelUpOptionList levelUpOptionList = new LevelUpOptionList();
            /***/
            
            /* Icons */
            [Serializable]
            public class Icon
            {
                public string name;
                public Sprite sprite;
            }

            [SerializeField] private List<Icon> icons;

            /***/
            
            public GameObject LevelUpUI;
            [SerializeField] private List<Button> buttons;
            [SerializeField] private List<TextMeshProUGUI> buttonsTitles;
            [SerializeField] private List<TextMeshProUGUI> buttonsTexts;
            [SerializeField] private List<Image> buttonsIcons;
            
            public static bool IsLevelingUp;
            
            [SerializeField] private PlayerStatsController PlayerStatsController;

            
            public void Start()
            {
                levelUpTextsJson = Resources.Load<TextAsset>("Level Up/Texts/LevelUpButtons");
                levelUpOptionList = JsonUtility.FromJson<LevelUpOptionList>(levelUpTextsJson.text);
                foreach (var option in levelUpOptionList.LevelUpOptions)
                {
                    option.OptionOnClickEvent = (Action<PlayerStatsController>)Delegate.CreateDelegate(
                        typeof(Action<PlayerStatsController>), null,
                  PlayerStatsController.GetLevelUpFunction(option.optionAttributeName));
                    option.optionIcon = icons.Find(x => x.name == option.optionIconName).sprite;
                }

                DebugSetOptions();
            }
            
            public void OpenLevelUpMenu()
            {
                LevelUpUI.SetActive(true);
                Time.timeScale = 0f;
                IsLevelingUp = true;
            }
            
            public void FirstPowerUpClicked()
            {
                // ApplyPowerUp(Options[0]);   
                CloseLevelUpMenu();
            }
            
            public void SecondPowerUpClicked()
            {
                // ApplyPowerUp(Options[1]);   
                CloseLevelUpMenu();
            }
            
            public void ThirdPowerUpClicked()
            {
                // ApplyPowerUp(Options[2]);  
                CloseLevelUpMenu();
            }

            public void DebugSetOptions()
            {
                for (var i = 0; i < buttons.Count; i++)
                {
                    buttonsTitles[i].SetText(levelUpOptionList.LevelUpOptions[i].optionTitle);
                    buttonsTexts[i].SetText(levelUpOptionList.LevelUpOptions[i].optionText);
                    buttonsIcons[i].sprite = levelUpOptionList.LevelUpOptions[i].optionIcon;
                    var iCopy = i;
                    buttons[i].onClick.AddListener(() => levelUpOptionList.LevelUpOptions[iCopy].OptionOnClickEvent(PlayerStatsController));
                }
            }
            
            private void CloseLevelUpMenu()
            {
                LevelUpUI.SetActive(false);
                Time.timeScale = 1f;
                IsLevelingUp = false;
            }

        }
    }
