using System;

namespace PortfolioSamples.StageData
{
    [Serializable]
    public struct StageRecord
    {
        public int Chapter;       // 1 – 10
        public int Stage;         // 1 – 5
        public string TargetWord; // 목표 단어 (한글)
        public int MoveLimit;     // 이동 횟수 제한
    }

    /// <summary>스테이지 데이터를 담는 저장소. Unity에서는 ScriptableObject 등으로 구현한다.</summary>
    public interface IStageDatabase
    {
        void ReplaceAll(System.Collections.Generic.IReadOnlyList<StageRecord> records);
    }
}
