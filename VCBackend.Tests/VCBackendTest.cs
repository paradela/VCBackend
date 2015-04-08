using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VCBackend.Models;
using VCBackend.Models.Dto;
using VCBackend.Business_Rules;
using VCBackend.Business_Rules.Users;
using VCBackend.Repositories;
using System.Collections.Generic;

namespace VCBackend.Tests
{
    [TestClass]
    public class VCBackendTest
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
                Assert.AreNotEqual(d.Type, Device.MOBILE_DEVICE);

            Assert.AreEqual("Jose Silva", user.Name);
            Assert.AreNotEqual("P4$$w0rd", user.Password);
            
        }
        [TestMethod]
        public void TestLogin()
        {
            try
            {

                TokenDto token = BRulesApi.Login("ea@asd.pt", "P4$$w0rd", null);

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
            TokenDto token = BRulesApi.AddDevice(user, "Teste", "12312ASBD");
            user = ur.FindById(1);
            Assert.AreEqual(user.Devices.Count(), 2);
        }

        [TestMethod]
        public void TestGetAllDevices()
        {
            IRepository<User> ur = UserRepository.getRepositorySingleton();
            User user = ur.FindById(1);
            ICollection<DeviceDto> devices = BRulesApi.GetAllDevices(user);
            Assert.AreEqual(devices.Count(), 2);
            Assert.AreEqual(devices.ElementAt(0).Name, "Default");
            Assert.AreEqual(devices.ElementAt(1).Name, "Teste");
        }

        [TestMethod]
        public void TestRemoveDevice()
        {
            IRepository<User> ur = UserRepository.getRepositorySingleton();
            User user = ur.FindById(1);
            BRulesApi.RemoveDevice(user, "12312ASBD");
            user = ur.FindById(1);
            Assert.AreEqual(user.Devices.Count(), 1);
            Assert.AreEqual(user.Devices.First().Name, "Default");
        }
    }
}
