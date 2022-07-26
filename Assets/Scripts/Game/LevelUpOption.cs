using System;
using Player;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class LevelUpOption
    {
        [Tooltip("Need to have the same exact name of the Player Stats Controller ENUMS or matching level up methods")]
        public string optionAttributeName; 
        public int optionLevel;
        public string optionTitle;
        public string optionText;
        public string optionIconName;
        public Sprite optionIcon;
        public string optionCategory;
        public Action<PlayerStatsController> OptionOnClickEvent = (PlayerStatsController playerRef) => {};
    }
}