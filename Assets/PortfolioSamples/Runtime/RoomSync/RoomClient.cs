using System.Threading.Tasks;

namespace PortfolioSamples.RoomSync
{
    public interface IRoomApi
    {
        Task<JoinResponse> JoinAsync(string code);
    }

    public interface IRoomView
    {
        void Render(RoomSlotState state);
    }

    /// <summary>
    /// 방 입장(REST)과 방 상태 수신(실시간)을 잇는 클라이언트.
    /// 여기서는 표준 Task를 쓰며, Unity 프로젝트에서는 UniTask로 바꿔 써도 된다.
    /// </summary>
    public sealed class RoomClient
    {
        private readonly IRoomApi _api;
        private readonly IRoomView _view;
        private readonly INetLog _log;

        public RoomClient(IRoomApi api, IRoomView view, INetLog log)
        {
            _api = api;
            _view = view;
            _log = log;
        }

        public RoomSlotState State { get; } = new RoomSlotState();

        public async Task<JoinResponse> JoinRoomAsync(string code)
        {
            var res = await _api.JoinAsync(code);
            _log.Rest("JOIN", code, res.Status, res.SlotIndex);
            return res;
        }

        /// <summary>실시간 메시지를 받았을 때 호출된다.</summary>
        public void OnRoomResult(RoomResult msg)
        {
            int count = msg.Players == null ? -1 : msg.Players.Length;
            _log.Realtime("RESULT", msg.RoomId, msg.Code, count);

            if (!State.TryApply(msg, out var error))
            {
                _log.Warn("room result ignored: " + error);
                return;
            }

            _view.Render(State);
        }
    }
}
