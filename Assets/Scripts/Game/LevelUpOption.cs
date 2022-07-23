using System;
using Player;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class LevelUpOption
    {
        public string optionAttributeName;
        public string optionTitle;
        public string optionText;
        public string optionIconName;
        public Sprite optionIcon;
        public Action<PlayerStatsController> OptionOnClickEvent = (PlayerStatsController playerRef) => {};
    }
}