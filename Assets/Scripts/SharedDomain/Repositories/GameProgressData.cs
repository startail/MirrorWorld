using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SharedDomain.Repositories
{
    // 進行状況の保存に必要なすべてのデータを含む
    [Serializable] 
    public class GameProgressData
    {
        // ProgressDataManagerのGameProgress Enumに対応する値
        public int gameMainProgressFlags; 
        
        public DateTime lastSaveTime;
    }
}