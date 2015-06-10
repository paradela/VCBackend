using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VCBackend.Models;
using VCBackend.Models.Dto;
using VCBackend.Repositories;
using System.Collections.Generic;
using VCBackend.ExternalServices.Ticketing;

namespace VCBackend.Tests
{
    [TestClass]
    public class VCBackendTest
    {
        [TestMethod]
        public void TestLoadToken()
        {
            Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
            byte[] emptyCard = new byte[]
            {
            0x60, 0x02, 0x01, 0xE4, 0x08, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            VCardToken token = new VCardToken(emptyCard);
            DateTime date = DateTime.Now;
            var load = new LoadRequest(token);

            load.DateInitial = date;

            bool approve = tk.ApproveLoadTokenRequest(load);

            bool res = tk.LoadToken(load);
        }

        [TestMethod]
        public void TestLoadCard()
        {
            Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
            byte[] emptyCard = new byte[]
            {
            0x60, 0x02, 0x01, 0xE4, 0x08, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            VCard token = new VCard(emptyCard);

            var load = new LoadRequest(token);
            load.Price = 10.0;

            bool approve = tk.ApproveLoadCardRequest(load);

            bool res = tk.LoadCard(load);
        }

        //[TestMethod]
        //public void TestUserCreation()
        //{
        //    BRulesApi.CreateUser(
        //        "Jose Silva",
        //        "ea@asd.pt",
        //        "P4$$w0rd");

        //    UnitOfWork uw = new UnitOfWork();
        //    User user = uw.UserRepository.GetByID(1);
        //    ICollection<Device> devices = user.Devices;

        //    foreach (Device d in devices)
        //        Assert.IsTrue(d is Default);

        //    Assert.AreEqual("Jose Silva", user.Name);
        //    Assert.AreNotEqual("P4$$w0rd", user.Password);
            
        //}
        //[TestMethod]
        //public void TestLogin()
        //{
        //    try
        //    {

        //        TokenDto token = BRulesApi.Login("ea@asd.pt", "P4$$w0rd", null);

        //    }
        //    catch (InvalidCredentialsException)
        //    {
        //        throw new AssertFailedException();
        //    }
            
        //}

        //[TestMethod]
        //public void TestAddDevice()
        //{
        //    UnitOfWork uw = new UnitOfWork();
            
        //    TokenDto token = BRulesApi.AddDevice(1, "Teste", "12312ASBD");
        //    User user = uw.UserRepository.GetByID(1);
        //    Assert.AreEqual(user.Devices.Count(), 2);
        //}

        //[TestMethod]
        //public void TestGetAllDevices()
        //{
        //    ICollection<DeviceDto> devices = BRulesApi.GetAllDevices(1);
        //    Assert.AreEqual(devices.Count(), 2);
        //    Assert.AreEqual(devices.ElementAt(0).Name, "Default");
        //    Assert.AreEqual(devices.ElementAt(1).Name, "Teste");
        //}

        //[TestMethod]
        //public void TestRemoveDevice()
        //{
        //    BRulesApi.RemoveDevice(1, "12312ASBD");
        //    UnitOfWork uw = new UnitOfWork();
        //    User user = uw.UserRepository.GetByID(1);
        //    Assert.AreEqual(user.Devices.Count(), 1);
        //    Assert.AreEqual(user.Devices.First().Name, "Default");
        //}

    }
}
