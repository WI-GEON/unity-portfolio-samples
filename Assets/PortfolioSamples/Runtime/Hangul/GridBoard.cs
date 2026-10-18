using System.Collections.Generic;

namespace PortfolioSamples.Hangul
{
    public struct Cell
    {
        public readonly int X;
        public readonly int Y;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => "(" + X + "," + Y + ")";
    }

    /// <summary>
    /// 논리 그리드. 화면의 UI 블록과 분리해, 결합 판정과 배치 상태만 다룬다.
    /// </summary>
    public sealed class GridBoard
    {
        private readonly Dictionary<(int, int), char> _cells = new Dictionary<(int, int), char>();

        public GridBoard(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }

        public void Place(Cell cell, char letter) => _cells[(cell.X, cell.Y)] = letter;

        public bool TryGet(Cell cell, out char letter) => _cells.TryGetValue((cell.X, cell.Y), out letter);

        /// <summary>
        /// from 블록을 to 블록에 합친다.
        /// 성공: to에 새 글자, from은 비움 / 실패: 아무것도 바꾸지 않음 (화면 블록은 원위치로 돌아감)
        /// </summary>
        public bool TryMerge(Cell from, Cell to)
        {
            char a, b;
            if (!TryGet(from, out a) || !TryGet(to, out b)) return false;

            char next;
            if (!HangulRule.TryCombine(b, a, out next)) return false;

            _cells[(to.X, to.Y)] = next;
            _cells.Remove((from.X, from.Y));
            return true;
        }
    }
}
