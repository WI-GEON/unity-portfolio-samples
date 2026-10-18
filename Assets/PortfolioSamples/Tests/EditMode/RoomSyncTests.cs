using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PortfolioSamples.RoomSync;

namespace PortfolioSamples.Tests
{
    public class RoomSyncTests
    {
        private sealed class FakeApi : IRoomApi
        {
            public Task<JoinResponse> JoinAsync(string code)
            {
                return Task.FromResult(new JoinResponse { Status = 200, SlotIndex = 1, RoomId = code });
            }
        }

        private sealed class FakeView : IRoomView
        {
            public int RenderCount;
            public void Render(RoomSlotState state) => RenderCount++;
        }

        private sealed class MemoryLog : INetLog
        {
            public readonly List<string> Lines = new List<string>();
            public void Rest(string action, string roomId, int status, int slotIndex) => Lines.Add("[REST] " + action + " " + status + " slot=" + slotIndex);
            public void Realtime(string action, string roomId, ResultCode code, int playerCount) => Lines.Add("[RT] " + action + " " + code + " players=" + playerCount);
            public void Warn(string message) => Lines.Add("[WARN] " + message);
        }

        private static RoomResult Result(params PlayerInfo[] players)
        {
            return new RoomResult { RoomId = "448709", Code = ResultCode.Ok, PlayerCount = players.Length, Players = players };
        }

        [Test]
        public void JoinResult_WithPlayers_FillsSlots()
        {
            var view = new FakeView();
            var client = new RoomClient(new FakeApi(), view, new MemoryLog());

            client.OnRoomResult(Result(new PlayerInfo("p1", "A", 0), new PlayerInfo("p2", "B", 1)));

            Assert.That(client.State.OccupiedCount, Is.EqualTo(2));
            Assert.That(client.State[1].Value.PlayerId, Is.EqualTo("p2"));
            Assert.That(view.RenderCount, Is.EqualTo(1));
        }

        [Test]
        public void LeaveResult_SnapshotRemovesLeftPlayer()
        {
            var client = new RoomClient(new FakeApi(), new FakeView(), new MemoryLog());
            client.OnRoomResult(Result(new PlayerInfo("p1", "A", 0), new PlayerInfo("p2", "B", 1), new PlayerInfo("p3", "C", 2)));

            client.OnRoomResult(Result(new PlayerInfo("p1", "A", 0), new PlayerInfo("p2", "B", 1)));   // p3 퇴장

            Assert.That(client.State.OccupiedCount, Is.EqualTo(2));
            Assert.That(client.State[2].HasValue, Is.False);
        }

        [Test]
        public void ResultWithoutPlayers_IsRejected_KeepsStateAndLogsWarning()
        {
            // 결과 코드만 오고 플레이어 정보가 비어 있는 경우
            var log = new MemoryLog();
            var view = new FakeView();
            var client = new RoomClient(new FakeApi(), view, log);
            client.OnRoomResult(Result(new PlayerInfo("p1", "A", 0)));

            client.OnRoomResult(new RoomResult { RoomId = "448709", Code = ResultCode.Ok });

            Assert.That(client.State.OccupiedCount, Is.EqualTo(1));
            Assert.That(view.RenderCount, Is.EqualTo(1));
            Assert.That(log.Lines[log.Lines.Count - 1], Does.Contain("players missing"));
        }

        [Test]
        public void PlayerCountMismatch_IsRejected()
        {
            var state = new RoomSlotState();
            var result = Result(new PlayerInfo("p1", "A", 0));
            result.PlayerCount = 2;

            string error;
            Assert.That(state.TryApply(result, out error), Is.False);
            Assert.That(error, Does.Contain("mismatch"));
        }

        [Test]
        public void DuplicatedSlot_IsRejected()
        {
            var state = new RoomSlotState();
            string error;

            Assert.That(state.TryApply(Result(new PlayerInfo("p1", "A", 0), new PlayerInfo("p2", "B", 0)), out error), Is.False);
        }

        [Test]
        public void Join_LogsRestResponse()
        {
            var log = new MemoryLog();
            var client = new RoomClient(new FakeApi(), new FakeView(), log);

            var res = client.JoinRoomAsync("448709").GetAwaiter().GetResult();

            Assert.That(res.Status, Is.EqualTo(200));
            Assert.That(log.Lines[0], Is.EqualTo("[REST] JOIN 200 slot=1"));
        }
    }
}
