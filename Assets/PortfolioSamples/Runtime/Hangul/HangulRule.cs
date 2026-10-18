namespace PortfolioSamples.Hangul
{
    /// <summary>블록 두 개가 만났을 때 한 글자로 합쳐질 수 있는지 판정한다.</summary>
    public static class HangulRule
    {
        /// <param name="current">놓여 있던 블록의 글자 (자음 또는 음절)</param>
        /// <param name="incoming">들어오는 블록의 자모</param>
        public static bool TryCombine(char current, char incoming, out char next)
        {
            next = '\0';

            // 초성 + 중성 → 받침 없는 음절 (ㅁ + ㅗ → 모)
            int cho = HangulComposer.ChoIndex(current);
            int jung = HangulComposer.JungIndex(incoming);
            if (cho >= 0 && jung >= 0)
            {
                next = HangulComposer.Compose(cho, jung);
                return true;
            }

            // 받침 없는 음절 + 자음 → 받침 있는 음절 (모 + ㅁ → 몸)
            int c, v, j;
            if (HangulComposer.TryDecompose(current, out c, out v, out j) && j == 0)
            {
                int jong = HangulComposer.JongIndex(incoming);
                if (jong > 0)
                {
                    next = HangulComposer.Compose(c, v, jong);
                    return true;
                }
            }

            return false;
        }
    }
}
