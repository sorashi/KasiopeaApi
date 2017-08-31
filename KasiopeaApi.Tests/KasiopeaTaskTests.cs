using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace KasiopeaApi.Tests
{
    [TestFixture]
    public class KasiopeaTaskTests
    {
        public KasiopeaInterface KasiopeaInterface {
            get {
                if (kasiopeaInterface != null) {
                    Assert.IsTrue(kasiopeaInterface.LoggedIn);
                    return kasiopeaInterface;
                }
                kasiopeaInterface = new KasiopeaInterface();
                Assert.IsTrue(kasiopeaInterface.Login(Credentials.Email, Credentials.Password).Result);
                Assert.IsTrue(kasiopeaInterface.LoggedIn);
                return kasiopeaInterface;
            }
        }

        private KasiopeaInterface kasiopeaInterface;
        private KasiopeaTask testTask;

        private KasiopeaTask TestTask => testTask ?? (testTask =
                                             KasiopeaTask.FromUrl("/archiv/2016/doma/A", KasiopeaInterface).Result);

        [Test]
        public async Task TestTaskFail() {
            await TestTask.GetInputReaderAsync(KasiopeaTask.InputVersion.Easy, KasiopeaInterface);
            TestTask.GetOutputWriter().WriteLine("This is a bad answer");
            Assert.AreEqual(KasiopeaTask.OutputCheckResult.Fail,
                await TestTask.PostOutputAsync(KasiopeaTask.InputVersion.Easy, KasiopeaInterface));
        }

        [Test]
        public async Task TestTaskSuccess() {
            var inputVersion = KasiopeaTask.InputVersion.Easy;
            for (var v = 0; v < 2; v++) {
                var reader = await TestTask.GetInputReaderAsync(inputVersion, KasiopeaInterface);
                var writer = TestTask.GetOutputWriter();
                // solve the task successfuly
                var t = int.Parse(reader.ReadLine());
                for (; t > 0; t--) {
                    reader.ReadLine();
                    var nums = reader.ReadLine().Split(' ').Select(int.Parse).ToArray();
                    int index = -1, max = int.MinValue;
                    for (var i = 0; i < nums.Length; i++)
                        if (nums[i] > max) {
                            max = nums[i];
                            index = i + 1;
                        }
                    writer.WriteLine(index);
                }
                var res = await TestTask.PostOutputAsync(inputVersion, KasiopeaInterface);
                Assert.AreEqual(KasiopeaTask.OutputCheckResult.Success, res);
                inputVersion = KasiopeaTask.InputVersion.Hard;
            }
        }
    }
}