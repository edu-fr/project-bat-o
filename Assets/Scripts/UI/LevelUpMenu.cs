    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Game;
    using Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityRandom = UnityEngine.Random;
    using SystemRandom = System.Random;
    
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
            
            [Header("UI References")]
            public GameObject LevelUpUI;
            
            [Header("Player stats controller reference")]
            [SerializeField] private PlayerStatsController PlayerStats;
            
            [Header("Button references")]
            [SerializeField] private Button[] buttons;
            [SerializeField] private TextMeshProUGUI[] buttonsTitles;
            [SerializeField] private TextMeshProUGUI[] buttonsTexts;
            [SerializeField] private Image[] buttonsIcons;

            [Header("Game variables")]
            public static bool IsLevelingUp;

            [Header("Player related variables")] 
            public int blessingSlots;
            
            [Header("Odds")] [Tooltip("Percentage of appearing")]
            [SerializeField] [Range(0, 100)] private int abilitiesOdds = 0;
            [SerializeField] [Range(0, 100)] private int blessingOdds = 0;
            [SerializeField] [Range(0, 100)] private int rampageOdds = 0;

            private List<LevelUpOption> _statsOptions = new List<LevelUpOption>();
            private List<LevelUpOption> _abilitiesOptions = new List<LevelUpOption>();
            private List<LevelUpOption> _blessingsOptions = new List<LevelUpOption>();
            private List<LevelUpOption> _rampagesOptions = new List<LevelUpOption>();
            
            public void Start()
            {
                /* Player related settings */
                blessingSlots = 2;
                
                levelUpTextsJson = Resources.Load<TextAsset>("Level Up/Texts/LevelUpButtons");
                levelUpOptionList = JsonUtility.FromJson<LevelUpOptionList>(levelUpTextsJson.text);
                foreach (var option in levelUpOptionList.LevelUpOptions)
                {
                    option.OptionOnClickEvent = (Action<PlayerStatsController>)Delegate.CreateDelegate(
                        typeof(Action<PlayerStatsController>), null,
                  PlayerStats.GetLevelUpFunction(option.optionAttributeName));
                    option.optionIcon = icons.Find(x => x.name == option.optionIconName).sprite; ;
                    
                    /* Fill lists */ 
                    if (option.optionCategory == "Stats") _statsOptions.Add(option); else 
                    if (option.optionCategory == "Blessing") _blessingsOptions.Add(option); else 
                    if (option.optionCategory == "Ability") _abilitiesOptions.Add(option); else 
                    if (option.optionCategory == "Rampage") _rampagesOptions.Add(option); 
                    else throw new NullReferenceException("Not expected option category: " + option.optionCategory);
                }
            }

            private void Update()
            {
                // DEBUG
                if (!IsLevelingUp)
                    if (Input.GetKeyUp(KeyCode.L))
                        DebugLevelUp();
            }

            private void DebugLevelUp()
            {
                OpenLevelUpMenu();
            }
            
            public void OpenLevelUpMenu()
            {
                LevelUpUI.SetActive(true);
                Time.timeScale = 0f;
                IsLevelingUp = true;
                SetRandomOptions();
            }

            private void SetRandomOptions()
            {
                UnityRandom.InitState((int) DateTime.Now.Ticks);
                var randomNumber = UnityRandom.Range(1, 101);
                var buttonContents = new[] {new LevelUpOption(), new LevelUpOption(), new LevelUpOption()};
                
                /* First button */ // Possibilities (in order of priority): Rampage / Blessing / Stats
                buttonContents[0] = GetRampage(randomNumber) ?? GetBlessing(randomNumber);
                if (buttonContents[0] == null)
                {
                    var randomIndexToPick = UnityRandom.Range(0, _statsOptions.Count);
                    buttonContents[0] = _statsOptions[randomIndexToPick];
                }
                SetButton(buttonIndex: 0, buttonContents[0]);
                /***/
                
                /* Second button */ // Possibilities (in order of priority): Ability / Stats 
                randomNumber = UnityRandom.Range(1, 101);
                buttonContents[1] = GetAbility(randomNumber);
                if (buttonContents[1] == null) { // If no abilities are available, random pick a stats option
                    var randomIndexToPick2 = UnityRandom.Range(0, _statsOptions.Count);
                    buttonContents[1] = _statsOptions[randomIndexToPick2];
                    
                    while (buttonContents[1] == buttonContents[0]) // If it's the same option as the first button, re-roll
                    {
                        randomIndexToPick2 = UnityRandom.Range(0, _statsOptions.Count);
                        buttonContents[1] = _statsOptions[randomIndexToPick2];
                    }
                }
                SetButton(buttonIndex: 1, buttonContents[1]);
                /***/
                
                /* Third button */ // Possibilities: Stats
                var randomIndexToPick3 = UnityRandom.Range(0, _statsOptions.Count);
                buttonContents[2] = _statsOptions[randomIndexToPick3];
                    
                while (buttonContents[2] == buttonContents[0] || buttonContents[2] == buttonContents[1]) // If it's the same option as the first buttons, re-roll
                {
                    randomIndexToPick3 = UnityRandom.Range(0, _statsOptions.Count);
                    buttonContents[2] = _statsOptions[randomIndexToPick3];
                }
                SetButton(buttonIndex: 2, buttonContents[2]);
                /***/
            }

            private void SetButton(int buttonIndex, LevelUpOption option)
            {
                // Setting visuals
                buttonsTitles[buttonIndex].SetText(option.optionTitle);
                buttonsTexts[buttonIndex].SetText(option.optionText);
                buttonsIcons[buttonIndex].sprite = option.optionIcon;
                
                // Setting functionalities
                buttons[buttonIndex].onClick.RemoveAllListeners();
                buttons[buttonIndex].onClick.AddListener(() => option.OptionOnClickEvent(PlayerStats));
                buttons[buttonIndex].onClick.AddListener(CloseLevelUpMenu);
            }

            private LevelUpOption GetRampage(int randomNumber)
            {
                if (randomNumber > rampageOdds) return null; // Not getting a rampage on this roll
                print("Trying to get a rampage!");
                if (PlayerStats.currentElementalRampagesList[0] != PlayerStatsController.ElementalRampage.None) // Already have a rampage
                    return null; 
                return _rampagesOptions.Find(x => x.optionAttributeName == PlayerStats.GetAvailableRampage().ToString());
            }

            private LevelUpOption GetBlessing(int randomNumber)
            {
                if (randomNumber > blessingOdds) return null; // Not getting a blessing on this roll 
                print("Trying to get a blessing!");
                var blessingOptionToPick = new List<LevelUpOption>();
                var playerCurrentBlessings = PlayerStats.currentElementalBlessingsList.ToList();
                // removing None blessings
                playerCurrentBlessings.RemoveAll(x => x == PlayerStatsController.ElementalBlessing.None);
                
                // If the blessings that the player already have can be upgraded, they will be added to the possible blessings list
                foreach (var blessing in playerCurrentBlessings) // TODO: CHECK IF THIS IS WORKING
                {
                    var blessingLevel = PlayerStats.GetBlessingCurrentLevel(blessing);
                    if (blessingLevel == -1) // already at max level
                        continue;
                    // Finding the option that match the name of the blessing and current level
                    var option = _blessingsOptions.Find(x => x.optionLevel == blessingLevel && x.optionAttributeName == blessing.ToString());
                    if (option != null) blessingOptionToPick.Add(option); // If found, add to the list of possible options
                }
                
                // The player already have the max of blessings and both of then still can be upgraded 
                if (blessingOptionToPick.Count == blessingSlots)
                {
                    var randomIndex = UnityRandom.Range(0, blessingOptionToPick.Count);
                    return blessingOptionToPick[randomIndex]; // Return one of the options
                }

                // The player still have 1 or more blessings to learn
                var blessingsPlayerDontHave = // Currently contains all blessings names
                    _blessingsOptions.Select(blessing => blessing.optionAttributeName)
                        .ToList();
                foreach (var blessing in playerCurrentBlessings)
                    blessingsPlayerDontHave.Remove(blessing.ToString()); // Now contains only the blessings that the player still don't have
                
                // Add all LevelUpOption blessings that the player still don't have
                foreach (var blessingPlayerDontHave in blessingsPlayerDontHave) 
                {
                    var blessingOptionToAdd = _blessingsOptions.Find(x => x.optionAttributeName == blessingPlayerDontHave);
                    if (blessingOptionToAdd != null)
                        blessingOptionToPick.Add(blessingOptionToAdd);
                }

                // Return null if no more blessings can be learned/upgraded
                if (blessingOptionToPick.Count == 0) return null;
                
                // Random pick a blessing among the available ones 
                var randomIndexToPick = UnityRandom.Range(0, blessingOptionToPick.Count);
                return blessingOptionToPick[randomIndexToPick];
            }

            private LevelUpOption GetAbility(int randomNumber)
            {
                if (randomNumber > abilitiesOdds) return null; // Not getting a ability on this roll
                
                var abilitiesOptionsToPick = new List<LevelUpOption>();
                // Add to the options abilities that the player still didn't learn
                foreach (var ability in _abilitiesOptions)
                {
                    if (PlayerStats.GetAbilityCurrentLevel(ability.optionAttributeName) < 1) 
                        abilitiesOptionsToPick.Add(ability);
                }

                // Return null if no abilities are available
                if (abilitiesOptionsToPick.Count == 0) return null;
                
                // Random pick a ability among the available ones 
                var randomIndexToPick = UnityRandom.Range(0, abilitiesOptionsToPick.Count);
                return abilitiesOptionsToPick[randomIndexToPick];
            }

            private void CloseLevelUpMenu()
            {
                LevelUpUI.SetActive(false);
                Time.timeScale = 1f;
                IsLevelingUp = false;
            }

        }
    }
