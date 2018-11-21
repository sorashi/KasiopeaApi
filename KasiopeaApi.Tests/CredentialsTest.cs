using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KasiopeaApi.Tests
{
    [TestFixture]
    class CredentialsTest
    {
        [Test]
        public void CredentialsPresentTest() {
            if(Credentials.Email == null || Credentials.Password == null) {
                Assert.Fail("The environment variables kasiopea_email and kasiopea_password are not populated an therefore this project cannot be unit-tested." +
                    "Please add these env. variables.");
            }
        }
    }
}
