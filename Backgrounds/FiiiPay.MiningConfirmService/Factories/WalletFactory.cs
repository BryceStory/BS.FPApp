using System;
using FiiiPay.Data;
using FiiiPay.Entities;

namespace FiiiPay.MiningConfirmService.Factories
{
    public abstract class WalletFactory
    {
        public abstract Wallet GetByAccountId(Guid accountId, int cryptoId);
        public abstract Wallet Insert(Wallet model);
        public abstract void Increase(long depositWalletId, decimal depositAmount);
        public abstract void DecreaseFreeze(long walletId, decimal amount);
        public abstract void Unfreeze(long walletId, decimal withdrawalAmount);
    }

    public class MerchantWalletFactory : WalletFactory
    {
        public override Wallet GetByAccountId(Guid accountId, int cryptoId)
        {
            var baseWallet = new MerchantWalletDAC().GetByAccountId(accountId, cryptoId);
            if (baseWallet == null)
                return null;
            return new Wallet
            {
                Id = baseWallet.Id,
                AccountId = baseWallet.MerchantAccountId,
                CryptoId = baseWallet.CryptoId,
                Balance = baseWallet.Balance,
                FrozenBalance = baseWallet.FrozenBalance,
                Address = baseWallet.Address
            };
        }

        public override Wallet Insert(Wallet wallet)
        {
            var merchantWallet = new MerchantWallet
            {
                MerchantAccountId = wallet.AccountId,
                CryptoId = wallet.CryptoId,
                Status = MerchantWalletStatus.Displayed,
                Balance = wallet.Balance,
                FrozenBalance = wallet.FrozenBalance,
                Address = wallet.Address,
                SupportReceipt = false,
            };
            merchantWallet = new MerchantWalletDAC().Insert(merchantWallet);
            wallet.Id = merchantWallet.Id;
            return wallet;
        }

        public override void Increase(long depositWalletId, decimal depositAmount)
        {
            new MerchantWalletDAC().Increase(depositWalletId, depositAmount);
        }

        public override void DecreaseFreeze(long walletId, decimal amount)
        {
            new MerchantWalletDAC().DecreaseFreeze(walletId, amount);
        }

        public override void Unfreeze(long walletId, decimal withdrawalAmount)
        {
            new MerchantWalletDAC().Unfreeze(walletId, withdrawalAmount);
        }
    }

    public class UserWalletFactory : WalletFactory
    {
        public override Wallet GetByAccountId(Guid accountId, int cryptoId)
        {
            var baseWallet = new UserWalletDAC().GetByAccountId(accountId, cryptoId);
            if (baseWallet == null)
                return null;
            return new Wallet
            {
                Id = baseWallet.Id,
                AccountId = baseWallet.UserAccountId,
                CryptoId = baseWallet.CryptoId,
                Balance = baseWallet.Balance,
                FrozenBalance = baseWallet.FrozenBalance,
                Address = baseWallet.Address
            };
        }

        public override Wallet Insert(Wallet wallet)
        {
            var userWallet = new UserWallet
            {
                UserAccountId = wallet.AccountId,
                CryptoId = wallet.CryptoId,
                Balance = wallet.Balance,
                FrozenBalance = wallet.FrozenBalance,
                Address = wallet.Address,
                HomePageRank = 0,
                PayRank = 0,
                ShowInHomePage = true
            };
            wallet.Id = new UserWalletDAC().Insert(userWallet);
            return wallet;
        }

        public override void Increase(long depositWalletId, decimal depositAmount)
        {
            new UserWalletDAC().Increase(depositWalletId, depositAmount);
        }

        public override void DecreaseFreeze(long walletId, decimal amount)
        {
            new UserWalletDAC().DecreaseFreeze(walletId, amount);
        }

        public override void Unfreeze(long walletId, decimal withdrawalAmount)
        {
            new UserWalletDAC().Unfreeze(walletId, withdrawalAmount);
        }
    }
}
