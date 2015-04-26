using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;

namespace VCBackend.Business_Rules.Accounts
{
    public class ProductManager : IManager
    {
        public ProductManager(UnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {
        }

        //public IList<Product> GetProducts()
        //{
        //    //TODO: Implement. Call TK Server to get all the available products.
        //    return new List<Product>();
        //}

        //public 
    }
}