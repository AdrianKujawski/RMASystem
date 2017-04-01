using System;
using System.Security.Cryptography;
using RMASystem.Helpers;
using Xunit;

namespace RMASystem.Tests
{
    public class Hash_Password
    {
        bool execute(string password) {
            var hash = HashHelper.Encrypt(password, "kupa");
            var descryptedMessage = HashHelper.Decrypt(hash, "kupa");
	        return password == descryptedMessage;
        }

        [Fact]
        public void Hash_Password_And_Unhash() {
            var password = "HasloTestowe!@#$";

            var result = execute(password);

            Assert.True(result);
        }
    }
}
