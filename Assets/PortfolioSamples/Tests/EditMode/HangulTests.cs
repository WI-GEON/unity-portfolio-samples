using NUnit.Framework;
using PortfolioSamples.Hangul;

namespace PortfolioSamples.Tests
{
    public class HangulTests
    {
        [Test]
        public void Compose_MieumOMieum_IsMom()
        {
            Assert.That(HangulComposer.Compose(6, 8, 16), Is.EqualTo('몸'));
            Assert.That((int)'몸', Is.EqualTo(0xBAB8));
        }

        [Test]
        public void Decompose_Mom_ReturnsIndices()
        {
            int cho, jung, jong;
            Assert.That(HangulComposer.TryDecompose('몸', out cho, out jung, out jong), Is.True);
            Assert.That(new[] { cho, jung, jong }, Is.EqualTo(new[] { 6, 8, 16 }));
        }

        [TestCase('ㅁ', 'ㅗ', '모')]
        [TestCase('모', 'ㅁ', '몸')]
        [TestCase('나', 'ㅁ', '남')]
        public void TryCombine_ValidPairs(char current, char incoming, char expected)
        {
            char next;
            Assert.That(HangulRule.TryCombine(current, incoming, out next), Is.True);
            Assert.That(next, Is.EqualTo(expected));
        }

        [TestCase('ㅁ', 'ㅁ')]   // 자음 + 자음
        [TestCase('몸', 'ㄱ')]   // 이미 받침 있음
        [TestCase('ㅗ', 'ㅁ')]   // 모음이 먼저 놓인 경우
        public void TryCombine_InvalidPairs(char current, char incoming)
        {
            char next;
            Assert.That(HangulRule.TryCombine(current, incoming, out next), Is.False);
        }

        [Test]
        public void GridMerge_Success_ReplacesTargetAndClearsSource()
        {
            var board = new GridBoard(4, 4);
            board.Place(new Cell(0, 0), 'ㅗ');
            board.Place(new Cell(0, 1), 'ㅁ');

            Assert.That(board.TryMerge(new Cell(0, 0), new Cell(0, 1)), Is.True);

            char letter;
            Assert.That(board.TryGet(new Cell(0, 1), out letter) && letter == '모', Is.True);
            Assert.That(board.TryGet(new Cell(0, 0), out letter), Is.False);
        }

        [Test]
        public void GridMerge_Failure_ChangesNothing()
        {
            var board = new GridBoard(4, 4);
            board.Place(new Cell(0, 0), 'ㄱ');
            board.Place(new Cell(0, 1), 'ㅁ');

            Assert.That(board.TryMerge(new Cell(0, 0), new Cell(0, 1)), Is.False);

            char letter;
            Assert.That(board.TryGet(new Cell(0, 0), out letter) && letter == 'ㄱ', Is.True);
            Assert.That(board.TryGet(new Cell(0, 1), out letter) && letter == 'ㅁ', Is.True);
        }
    }
}
