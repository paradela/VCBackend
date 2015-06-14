using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;
using System.Data.Entity;

namespace VCBackend.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private ModelContainer context;
        private Repository<User> userRepository;
        private Repository<Device> deviceRepository;
        private Repository<AccessTokens> accessTokensRepository;
        private Repository<Account> accountRepository;
        private Repository<VCard> vcardRepository;
        private Repository<VCardToken> tokenRepository;
        private Repository<PaymentRequest> paymentRepository;

        public UnitOfWork()
        {
            context = new ModelContainer();
        }

        public Repository<User> UserRepository
        {
            get
            {

                if (this.userRepository == null)
                {
                    this.userRepository = new Repository<User>(context);
                }
                return userRepository;
            }
        }

        public Repository<Device> DeviceRepository
        {
            get
            {

                if (this.deviceRepository == null)
                {
                    this.deviceRepository = new Repository<Device>(context);
                }
                return deviceRepository;
            }
        }

        public Repository<AccessTokens> AccessTokensRepository
        {
            get
            {
                if (this.accessTokensRepository == null)
                {
                    this.accessTokensRepository = new Repository<AccessTokens>(context);
                }
                return accessTokensRepository;
            }
        }

        public Repository<Account> AccountRepository
        {
            get
            {
                if (this.accountRepository == null)
                {
                    this.accountRepository = new Repository<Account>(context);
                }
                return accountRepository;
            }
        }

        public Repository<VCard> VCardRepository
        {
            get
            {
                if (this.vcardRepository == null)
                {
                    this.vcardRepository = new Repository<VCard>(context);
                }
                return vcardRepository;
            }
        }

        public Repository<VCardToken> VCardTokenRepository
        {
            get
            {
                if (this.tokenRepository == null)
                {
                    this.tokenRepository = new Repository<VCardToken>(context);
                }
                return tokenRepository;
            }
        }

        public Repository<PaymentRequest> PaymentRepository
        {
            get
            {
                if (this.paymentRepository == null)
                {
                    this.paymentRepository = new Repository<PaymentRequest>(context);
                }
                return paymentRepository;
            }
        }

        public DbContextTransaction TransactionBegin()
        {
            return context.Database.BeginTransaction();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}