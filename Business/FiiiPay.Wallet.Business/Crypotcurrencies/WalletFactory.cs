using System;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Foundation.Data;

namespace FiiiPay.Wallet.Business.Crypotcurrencies
{
    public abstract class WalletFactory
    {
        public abstract WalletEntity GetByAccountId(Guid accountId, int cryptoId);
        public abstract WalletEntity Insert(WalletEntity model);
        public abstract void Increase(long depositWalletId, decimal depositAmount);
        public abstract void DecreaseFreeze(long walletId, decimal amount);
        public abstract void Unfreeze(long walletId, decimal withdrawalAmount, decimal frozenBalance);
    }

    public class MerchantWalletFactory : WalletFactory
    {
        public override WalletEntity GetByAccountId(Guid accountId, int cryptoId)
        {
            var baseWallet = new MerchantWalletDAC().GetByAccountId(accountId, cryptoId);
            if (baseWallet == null)
                return null;
            return new WalletEntity
            {
                Id = baseWallet.Id,
                AccountId = baseWallet.MerchantAccountId,
                CryptoId = baseWallet.CryptoId,
                Balance = baseWallet.Balance,
                FrozenBalance = baseWallet.FrozenBalance,
                Address = baseWallet.Address,
                CryptoCode = baseWallet.CryptoCode
            };
        }

        public override WalletEntity Insert(WalletEntity walletEntity)
        {
            var merchantWallet = new MerchantWallet
            {
                MerchantAccountId = walletEntity.AccountId,
                CryptoId = walletEntity.CryptoId,
                Status = MerchantWalletStatus.Displayed,
                Balance = walletEntity.Balance,
                FrozenBalance = walletEntity.FrozenBalance,
                Address = walletEntity.Address,
                SupportReceipt = false,
                CryptoCode = walletEntity.CryptoCode
            };
            merchantWallet = new MerchantWalletDAC().Insert(merchantWallet);
            walletEntity.Id = merchantWallet.Id;
            return walletEntity;
        }

        public override void Increase(long depositWalletId, decimal depositAmount)
        {
            new MerchantWalletDAC().Increase(depositWalletId, depositAmount);
        }

        public override void DecreaseFreeze(long walletId, decimal amount)
        {
            new MerchantWalletDAC().DecreaseFreeze(walletId, amount);
        }

        public override void Unfreeze(long walletId, decimal withdrawalAmount, decimal frozenBalance)
        {
            new MerchantWalletDAC().Unfreeze(walletId, withdrawalAmount, frozenBalance);
        }
    }

    public class UserWalletFactory : WalletFactory
    {
        public override WalletEntity GetByAccountId(Guid accountId, int cryptoId)
        {
            var baseWallet = new UserWalletDAC().GetByAccountId(accountId, cryptoId);
            if (baseWallet == null)
                return null;
            return new WalletEntity
            {
                Id = baseWallet.Id,
                AccountId = baseWallet.UserAccountId,
                CryptoId = baseWallet.CryptoId,
                Balance = baseWallet.Balance,
                FrozenBalance = baseWallet.FrozenBalance,
                Address = baseWallet.Address,
                CryptoCode = baseWallet.CryptoCode
            };
        }

        public override WalletEntity Insert(WalletEntity walletEntity)
        {
            var userWallet = new UserWallet
            {
                UserAccountId = walletEntity.AccountId,
                CryptoId = walletEntity.CryptoId,
                CryptoCode = walletEntity.CryptoCode,
                Balance = walletEntity.Balance,
                FrozenBalance = walletEntity.FrozenBalance,
                Address = walletEntity.Address,
                HomePageRank = 0,
                PayRank = 0,
                ShowInHomePage = true
            };
            walletEntity.Id = new UserWalletDAC().Insert(userWallet);
            return walletEntity;
        }

        public override void Increase(long depositWalletId, decimal depositAmount)
        {
            new UserWalletDAC().Increase(depositWalletId, depositAmount);
        }

        public override void DecreaseFreeze(long walletId, decimal amount)
        {
            new UserWalletDAC().DecreaseFreeze(walletId, amount);
        }

        public override void Unfreeze(long walletId, decimal withdrawalAmount, decimal frozenBalance)
        {
            new UserWalletDAC().Unfreeze(walletId, withdrawalAmount, frozenBalance);
        }
    }
}