using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrizeUI
{
    public class PrizeManager : MonoBehaviour
    {
        public Action<int,int> OnPrizePickedEvent;
        public Action OnGetMoreKeyEvent;
        public Action OnNoThanksEvent;
        public PrizeData PrizeData;
    }
}