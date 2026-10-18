using System.Collections.Generic;

namespace PortfolioSamples.RoomSync
{
    /// <summary>
    /// 방 슬롯 상태. 서버가 보낸 결과를 "전체 스냅샷"으로 받아 통째로 교체한다.
    /// 일부만 고쳐 쓰지 않으므로, 나간 플레이어가 남는 일이 생기지 않는다.
    /// </summary>
    public sealed class RoomSlotState
    {
        public const int MaxSlots = 4;

        private readonly PlayerInfo?[] _slots = new PlayerInfo?[MaxSlots];

        public string RoomId { get; private set; }

        public PlayerInfo? this[int slot] => _slots[slot];

        public int OccupiedCount
        {
            get
            {
                int n = 0;
                foreach (var s in _slots) if (s.HasValue) n++;
                return n;
            }
        }

        /// <summary>결과가 유효할 때만 적용한다. 정보가 빠진 결과는 이전 상태를 유지하고 이유를 돌려준다.</summary>
        public bool TryApply(RoomResult result, out string error)
        {
            error = Validate(result);
            if (error != null) return false;

            for (int i = 0; i < MaxSlots; i++) _slots[i] = null;
            foreach (var p in result.Players) _slots[p.SlotIndex] = p;
            RoomId = result.RoomId;
            return true;
        }

        private static string Validate(RoomResult result)
        {
            if (result.Code != ResultCode.Ok) return "result code is " + result.Code;
            if (result.Players == null) return "players missing in result";
            if (result.PlayerCount != result.Players.Length)
                return "player count mismatch: " + result.PlayerCount + " vs " + result.Players.Length;

            var used = new HashSet<int>();
            foreach (var p in result.Players)
            {
                if (p.SlotIndex < 0 || p.SlotIndex >= MaxSlots) return "slot out of range: " + p.SlotIndex;
                if (!used.Add(p.SlotIndex)) return "duplicated slot: " + p.SlotIndex;
            }
            return null;
        }
    }
}
