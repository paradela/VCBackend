using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VCBackend.Models;
using VCBackend.Business_Rules;
using VCBackend.Repositories;
using System.Collections.Generic;

namespace VCBackend.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestUserCreation()
        {
            BRulesApi.CreateUser(
                "Jose Silva",
                "ea@asd.pt",
                "P4ssw0rd",
                "92924556",
                true,
                "9356678976543",
                "Rua Fixe de Baixo\nnº83 5esq",
                "5370-642",
                "Lisboa",
                "Portugal");

            IRepository<User> ur = new UserRepository();
            User user = ur.FindById(1);
            ICollection<Device> devices = user.Devices;

            foreach(Device d in devices)
                Assert.AreNotEqual(d.Type, DeviceType.MOBILE_DEVICE);

            Assert.AreEqual("Jose Silva", user.Name);
            Assert.AreNotEqual("P4ssw0rd", user.Password);
            
        }

    }
}
