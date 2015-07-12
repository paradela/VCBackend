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
        //[TestMethod]
        //public void TestCenas()
        //{
        //    //[115 116 114 105 110 103 ]
        //    byte[] a = new byte[] { 0x73, 0x74, 0x72, 0x69, 0x6e, 0x67 };
        //    String s = System.Text.Encoding.UTF8.GetString(a, 0, a.Length);
        //    Assert.AreEqual("string", s);
        //}

        //[TestMethod]
        //public void TestLoadToken()
        //{
        //    Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
        //    byte[] emptyCard = new byte[]
        //    {
        //    0x60, 0x02, 0x01, 0xE4, 0x08, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
        //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
        //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        //    };
        //    VCardToken token = new VCardToken(emptyCard);
        //    DateTime date = DateTime.Now;
        //    var load = new LoadRequest(token);

        //    load.DateInitial = date;

        //    bool approve = tk.ApproveLoadTokenRequest(load);

        //    bool res = tk.LoadToken(load);
        //}

        //[TestMethod]
        //public void TestLoadCard()
        //{
        //    Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
        //    byte[] emptyCard = new byte[]
        //    {
        //    0x60, 0x02, 0x01, 0xE4, 0x08, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
        //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
        //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        //    };
        //    VCard token = new VCard(emptyCard);

        //    var load = new LoadRequest(token);
        //    load.Price = 10.0;

        //    bool approve = tk.ApproveLoadCardRequest(load);

        //    bool res = tk.LoadCard(load);

        //    double balance = tk.ReadCardBalance(token);
        //}

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

        private static string xml = 
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<tkmsg>" +
                  "<decode_reply>" + 
                    "<task type=\"Validate\" version=\"1.16.1907\" date_time=\"1436224846\" result=\"0\" usecs=\"286178\">" +
                      "<SAM>2148113452</SAM>" + 
                      "<before>" + 
                        "<card />" +
                      "</before>" +
                      "<after>" +
                        "<card sn=\"1\" sn_hi=\"39847944\" type=\"CTS512B\" group=\"CTS512B\" region=\"Lisboa\" layout=\"SeteColinas\" data_model=\"SeteColinas\">" +
                          "<issue>" + 
                            "<attribs></attribs>" + 
                          "</issue>" +
                          "<actions>" +
                            "<action type=\"Validate\" addr=\"1\">" +
                              "<product version=\"0\" ref=\"33592\" tariff=\"33592\" price=\"33592\" card_value=\"2065208\">" +
                                "<attribs></attribs>" +
                              "</product>" +
                            "</action>" +
                          "</actions>" +
                        "</card>" +
                      "</after>" +
                      "<trace>" +
                        "<data type=\"CARD_ATR\"></data>" +
                        "<data type=\"CALYPSO_TPDU_TX\">00 A4 04 00 0A 43 34 42 43 54 53 35 31 32 42</data>" +
                        "<data type=\"CALYPSO_TPDU_RX\">43 34 42 43 54 53 35 31 32 42 90 00</data>" +
                        "<data type=\"CALYPSO_TPDU_TX\">00 B0 00 04 06</data>" +
                        "<data type=\"CALYPSO_TPDU_RX\">08 08 01 00 00 00 90 00</data>" +
                        "<data type=\"CALYPSO_TPDU_TX\">00 B0 00 00 40</data>" +
                        "<data type=\"CALYPSO_TPDU_RX\">60 02 01 E4 08 08 01 00 00 00 0C 01 5D D8 39 D1 26 29 9F C3 84 36 0F A0 C5 05 F7 FE 00 1C 00 00 00 00 00 00 00 00 01 F4 00 00 00 00 00 00 00 00 00 00 00 00 A3 8A 00 01 F4 00 00 00 24 18 69 90 90 00</data>" +
                        "<data type=\"SAM_TPDU_TX\">F0 2A 9E 9A 15 60 02 08 08 00 00 00 01 00 1D 00 00 04 E7 44 9F 0E 40 00 00 5E</data>" +
                        "<data type=\"SAM_TPDU_RX\">0C 01 5D D8 90 00</data>" +
                        "<data type=\"SAM_TPDU_TX\">F0 2A 9E 9A 15 60 02 08 08 00 00 00 01 00 1F 00 00 02 00 01 F4 00 00 00 00 00</data>" +
                        "<data type=\"SAM_TPDU_RX\">A3 8A 90 00</data>" +
                        "<data type=\"SAM_TPDU_TX\">F0 2A 80 86 15 60 02 08 08 00 00 00 01 00 21 00 00 00 E7 44 98 A6 7F 0E 10 DA</data>" +
                        "<data type=\"SAM_TPDU_RX\">FC 19 C0 D3 20 15 00 81 90 00</data>" +
                        "<data type=\"SAM_TPDU_TX\">F0 2A 9E 9A 15 60 02 08 08 00 00 00 01 00 1F 00 00 02 02 01 F3 D7 80 80 1E C8</data>" +
                        "<data type=\"SAM_TPDU_RX\">A0 33 90 00</data>" +
                        "<data type=\"SAM_TPDU_TX\">F0 2A 9E 9A 13 60 02 08 08 00 00 00 01 00 1F 00 00 02 02 01 F3 D7 80 88</data>" +
                        "<data type=\"SAM_TPDU_RX\">87 46 90 00</data>" +
                        "<data type=\"CALYPSO_TPDU_TX\">00 D6 00 36 08 02 01 F3 D7 80 88 87 46</data>" +
                        "<data type=\"CALYPSO_TPDU_RX\">90 00</data>" +
                        "<data type=\"CALYPSO_TPDU_TX\">00 D6 00 24 12 00 02 01 F3 69 A2 BC 6B C0 44 00 F6 00 00 00 08 A0 33</data>" +
                        "<data type=\"CALYPSO_TPDU_RX\">90 00</data>" +
                      "</trace>" + 
                    "</task>" +
                  "</decode_reply>" +
                "</tkmsg>";

        [TestMethod]
        public void TestXmlParse()
        {
            Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();
            //TKTransactionData data = tk.ParseTKDecodeResult(xml);
        }
    }
}
