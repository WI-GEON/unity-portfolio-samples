using System;

namespace PortfolioSamples.RoomSync
{
    public enum ResultCode
    {
        Ok,
        RoomFull,
        RoomNotFound,
        Error
    }

    [Serializable]
    public struct PlayerInfo
    {
        public string PlayerId;
        public string Nickname;
        public int SlotIndex;
        public bool IsReady;

        public PlayerInfo(string playerId, string nickname, int slotIndex, bool isReady = false)
        {
            PlayerId = playerId;
            Nickname = nickname;
            SlotIndex = slotIndex;
            IsReady = isReady;
        }
    }

    /// <summary>
    /// 실시간 서버가 보내는 방 상태 결과 (예시 메시지 구조).
    /// 결과 코드만이 아니라 현재 인원과 플레이어 정보 전체를 함께 보낸다.
    /// </summary>
    [Serializable]
    public struct RoomResult
    {
        public string RoomId;
        public ResultCode Code;
        public int PlayerCount;
        public PlayerInfo[] Players;   // 현재 방에 있는 플레이어 전체
    }

    public struct JoinResponse
    {
        public int Status;      // HTTP 상태 코드
        public int SlotIndex;
        public string RoomId;
    }
}
