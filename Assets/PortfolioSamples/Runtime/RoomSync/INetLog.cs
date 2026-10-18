namespace PortfolioSamples.RoomSync
{
    /// <summary>
    /// REST 응답과 실시간 수신을 같은 형식으로 남겨, 서버 로그와 시간순으로 맞춰 볼 수 있게 한다.
    /// </summary>
    public interface INetLog
    {
        void Rest(string action, string roomId, int status, int slotIndex);
        void Realtime(string action, string roomId, ResultCode code, int playerCount);
        void Warn(string message);
    }
}
