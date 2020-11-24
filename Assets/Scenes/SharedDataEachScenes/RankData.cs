using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.SharedDataEachScenes
{
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        private List<T> target;
        public List<T> ToList() => target;

        public Serialization(List<T> target) => this.target = target;
    }
    
    [Serializable]
    public struct RankData
    {
        public int rank;
        public string userName;
        public int huntingCount;
        public int playMilliSecondTime;
    }
}
