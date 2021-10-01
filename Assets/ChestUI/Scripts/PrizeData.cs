using System.Collections;
using System.Collections.Generic;
using Fardin;
using NaughtyAttributes;
using UnityEngine;

namespace PrizeUI
{
    [CreateAssetMenu(menuName = "ChestUI/PrizeData", fileName = "PrizeData")]
    public class PrizeData : ScriptableObject
    {
        [System.Serializable]
        public struct Prize
        {
            [Range(0, 100)] public int value;
        }

        public Prize[] Prizes;

        public void ShuffleList()
        {
            Prizes.Shuffle();
        }
    }
}