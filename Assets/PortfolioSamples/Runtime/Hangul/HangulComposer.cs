namespace PortfolioSamples.Hangul
{
    /// <summary>
    /// 유니코드 한글 음절 조합 · 분해.
    /// 음절 = 0xAC00 + (초성 × 21 + 중성) × 28 + 종성 (모두 0부터 시작, 받침 없음 = 0)
    /// </summary>
    public static class HangulComposer
    {
        public const int SyllableBase = 0xAC00;
        public const int SyllableLast = 0xD7A3;
        public const int JungCount = 21;
        public const int JongCount = 28;

        // 호환 자모 기준 순서표
        public const string Cho  = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";                  // 19
        public const string Jung = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";              // 21
        public const string Jong = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";    // 28 (0 = 받침 없음)

        public static char Compose(int cho, int jung, int jong = 0)
        {
            return (char)(SyllableBase + (cho * JungCount + jung) * JongCount + jong);
        }

        public static bool IsSyllable(char c) => c >= SyllableBase && c <= SyllableLast;

        public static bool TryDecompose(char syllable, out int cho, out int jung, out int jong)
        {
            cho = jung = jong = -1;
            if (!IsSyllable(syllable)) return false;

            int offset = syllable - SyllableBase;
            jong = offset % JongCount;
            jung = (offset / JongCount) % JungCount;
            cho = offset / (JongCount * JungCount);
            return true;
        }

        public static int ChoIndex(char jamo) => Cho.IndexOf(jamo);
        public static int JungIndex(char jamo) => Jung.IndexOf(jamo);

        public static int JongIndex(char jamo)
        {
            int i = Jong.IndexOf(jamo);
            return i > 0 ? i : -1;   // 0번(공백)은 받침 없음이라 자모로 취급하지 않는다
        }
    }
}
