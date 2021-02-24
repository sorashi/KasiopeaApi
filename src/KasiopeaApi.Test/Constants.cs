using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KasiopeaApi.Test
{
    static class Constants
    {
        public static string TestBase => Environment.GetEnvironmentVariable("KASIOPEA_TEST_BASE");
        public static string TestEmail => Environment.GetEnvironmentVariable("KASIOPEA_TEST_EMAIL");
        public static string TestPassword => Environment.GetEnvironmentVariable("KASIOPEA_TEST_PASSWORD");
    }
}
