using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VCBackend.Models;
using VCBackend.Business_Rules;
using VCBackend.Business_Rules.Users;
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
                "P4$$w0rd");

            IRepository<User> ur = UserRepository.getRepositorySingleton();
            User user = ur.FindById(1);
            ICollection<Device> devices = user.Devices;

            foreach(Device d in devices)
                Assert.AreNotEqual(d.Type, DeviceType.MOBILE_DEVICE);

            Assert.AreEqual("Jose Silva", user.Name);
            Assert.AreNotEqual("P4$$w0rd", user.Password);
            
        }
        [TestMethod]
        public void TestLogin()
        {
            try
            {

                String token = BRulesApi.Login("ea@asd.pt", "P4$$w0rd", null);

            }
            catch (InvalidCredentialsException)
            {
                throw new AssertFailedException();
            }
            
        }

        [TestMethod]
        public void TestAddDevice()
        {
            IRepository<User> ur = UserRepository.getRepositorySingleton();
            User user = ur.FindById(1);
            String token = BRulesApi.AddDevice(user, "Teste", "12312ASBD");
            //using (var db = new VCardContext())
            //{
            //    User user = db.Users.First();
            //    Device d = new Device("1231231kljnk123");
            //    d.Name = "Name";
            //    d.Token = "asd,lçal,dç,,admakndbhjb";
            //    user.Devices.Add(d);
            //    db.SaveChanges();
            //}
        }
    }
}
