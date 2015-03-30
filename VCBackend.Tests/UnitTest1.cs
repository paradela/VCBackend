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
            IEnumerable<User> users = ur.List;
            Device i = new PartialDevice();
            PartialDevice a = new PartialDevice();
            a.Token = "asd";



            
        }
    }
}
