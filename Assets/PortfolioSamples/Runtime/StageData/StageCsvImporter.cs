using System;
using System.Collections.Generic;
using PortfolioSamples.Hangul;

namespace PortfolioSamples.StageData
{
    public sealed class ImportReport
    {
        public readonly List<string> Errors = new List<string>();
        public int AppliedCount;
        public bool Succeeded => Errors.Count == 0;
    }

    /// <summary>
    /// CSV를 먼저 끝까지 검증하고, 오류가 하나도 없을 때만 저장소를 갱신한다.
    /// 오류는 행 번호와 함께 모두 모아 한 번에 보여준다.
    /// 열 구성: Chapter,Stage,TargetWord,MoveLimit (예시 열 구성)
    /// </summary>
    public static class StageCsvImporter
    {
        public const int ChapterCount = 10;
        public const int StagesPerChapter = 5;

        public static ImportReport Import(string csvText, IStageDatabase database)
        {
            var report = new ImportReport();
            var records = Parse(csvText, report.Errors);

            if (report.Errors.Count == 0) CheckPositions(records, report.Errors);

            if (!report.Succeeded) return report;   // 하나라도 틀리면 SO를 건드리지 않는다

            database.ReplaceAll(records);
            report.AppliedCount = records.Count;
            return report;
        }

        private static List<StageRecord> Parse(string csvText, List<string> errors)
        {
            var result = new List<StageRecord>();
            var lines = (csvText ?? string.Empty).Replace("\r\n", "\n").Split('\n');

            for (int i = 1; i < lines.Length; i++)          // 0행은 머리글
            {
                string line = lines[i].Trim();
                if (line.Length == 0) continue;
                int lineNo = i + 1;

                var cols = line.Split(',');
                if (cols.Length != 4)
                {
                    errors.Add(lineNo + "행: 열 개수가 4가 아님 (" + cols.Length + ")");
                    continue;
                }

                int chapter, stage, moveLimit;
                bool ok = true;
                if (!int.TryParse(cols[0].Trim(), out chapter)) { errors.Add(lineNo + "행: Chapter가 숫자가 아님"); ok = false; }
                if (!int.TryParse(cols[1].Trim(), out stage)) { errors.Add(lineNo + "행: Stage가 숫자가 아님"); ok = false; }
                if (!int.TryParse(cols[3].Trim(), out moveLimit) || moveLimit <= 0) { errors.Add(lineNo + "행: MoveLimit는 1 이상의 숫자여야 함"); ok = false; }

                string word = cols[2].Trim();
                if (word.Length == 0 || !IsAllSyllables(word)) { errors.Add(lineNo + "행: TargetWord는 한글 음절이어야 함"); ok = false; }

                if (ok)
                {
                    result.Add(new StageRecord { Chapter = chapter, Stage = stage, TargetWord = word, MoveLimit = moveLimit });
                }
            }
            return result;
        }

        private static void CheckPositions(List<StageRecord> records, List<string> errors)
        {
            var seen = new HashSet<int>();
            foreach (var r in records)
            {
                if (r.Chapter < 1 || r.Chapter > ChapterCount || r.Stage < 1 || r.Stage > StagesPerChapter)
                {
                    errors.Add(r.Chapter + "-" + r.Stage + ": 범위를 벗어난 위치");
                    continue;
                }
                if (!seen.Add(r.Chapter * 100 + r.Stage)) errors.Add(r.Chapter + "-" + r.Stage + ": 같은 위치가 두 번 나옴");
            }
        }

        private static bool IsAllSyllables(string s)
        {
            foreach (char c in s) if (!HangulComposer.IsSyllable(c)) return false;
            return true;
        }
    }
}
