using System.Collections.Generic;
using NUnit.Framework;
using PortfolioSamples.StageData;

namespace PortfolioSamples.Tests
{
    public class StageCsvImporterTests
    {
        private sealed class FakeDatabase : IStageDatabase
        {
            public List<StageRecord> Records;
            public int WriteCount;

            public void ReplaceAll(IReadOnlyList<StageRecord> records)
            {
                Records = new List<StageRecord>(records);
                WriteCount++;
            }
        }

        private const string Header = "Chapter,Stage,TargetWord,MoveLimit\n";

        [Test]
        public void ValidCsv_IsApplied()
        {
            var db = new FakeDatabase();
            var report = StageCsvImporter.Import(Header + "1,1,나무,10\n1,2,마무리,12\n", db);

            Assert.That(report.Succeeded, Is.True);
            Assert.That(report.AppliedCount, Is.EqualTo(2));
            Assert.That(db.WriteCount, Is.EqualTo(1));
        }

        [Test]
        public void AnyError_StopsWholeImport_AndCollectsAllErrors()
        {
            var db = new FakeDatabase();
            var csv = Header + "1,1,나무,10\n1,x,말,10\n2,1,abc,10\n";

            var report = StageCsvImporter.Import(csv, db);

            Assert.That(report.Succeeded, Is.False);
            Assert.That(report.Errors.Count, Is.EqualTo(2));
            Assert.That(report.Errors[0], Does.StartWith("3행"));
            Assert.That(report.Errors[1], Does.StartWith("4행"));
            Assert.That(db.WriteCount, Is.EqualTo(0));   // 기존 데이터는 그대로
        }

        [Test]
        public void OutOfRangeOrDuplicatedPosition_IsRejected()
        {
            var db = new FakeDatabase();
            var csv = Header + "1,1,나무,10\n1,1,말,10\n11,1,몸,10\n";

            var report = StageCsvImporter.Import(csv, db);

            Assert.That(report.Errors.Count, Is.EqualTo(2));
            Assert.That(db.WriteCount, Is.EqualTo(0));
        }

        [Test]
        public void WrongColumnCount_IsReportedWithLineNumber()
        {
            var report = StageCsvImporter.Import(Header + "1,1,나무\n", new FakeDatabase());

            Assert.That(report.Errors[0], Does.StartWith("2행"));
        }
    }
}
