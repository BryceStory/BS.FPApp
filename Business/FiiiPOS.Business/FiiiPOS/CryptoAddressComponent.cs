using System;
using System.Collections.Generic;
using System.Linq;
using FiiiPay.Data;
using FiiiPay.Entities;
using FiiiPay.Entities.EntitySet;
using FiiiPay.Entities.Enums;
using FiiiPay.Foundation.Business.Agent;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using FiiiPay.Framework.Component;
using FiiiPay.Framework.Component.Exceptions;
using FiiiPay.Framework.Component.Properties;
using FiiiPay.Framework.Exceptions;

namespace FiiiPOS.Business.FiiiPOS
{
    public class CryptoAddressComponent
    {
        public List<CryptoAddressIndexES> GetMerchantCryptoAddress(Guid merchantAccountId)
        {
            var list = new CryptoAddressDAC().GetByAccountId(merchantAccountId);
            var cryptoList = new CryptocurrencyDAC().GetAllActived();

            cryptoList.MoveTop(t => t.Code == "FIII");

            var account = new MerchantAccountDAC().GetById(merchantAccountId);
            var pos = new POSDAC().GetById(account.POSId.Value);
            if (!pos.IsWhiteLabel)
            {
                cryptoList.RemoveAll(t => t.IsWhiteLabel == 1);
            }
            else
            {
                cryptoList.MoveTop(t => t.Code == pos.FirstCrypto);
            }

            return cryptoList.Select(e =>
            {
                return new CryptoAddressIndexES
                {
                    CryptoId = e.Id,
                    Code = e.Code,
                    NeedTag = e.NeedTag,
                    Count = list.Count(c => c.CryptoId == e.Id)
                };
            }).ToList();
        }

        public List<CryptoAddress> GetMerchantCryptoAddress(Guid merchantAccountId, int cryptoId)
        {
            var dac = new CryptoAddressDAC();

            return dac.GetByCryptoId(merchantAccountId, cryptoId);
        }

        public void AddAddress(Guid merchantAccountId, int cryptoId, string address, string tag, string remark)
        {
            var crypto = new CryptocurrencyDAC().GetById(cryptoId);
            CryptoAddressValidation.ValidateAddress(crypto.Code, address);
            if (!string.IsNullOrEmpty(tag))
            {
                CryptoAddressValidation.ValidateTag(crypto.Code, tag);
            }

            //var merchant = new MerchantAccountDAC().GetById(merchantAccountId);

            try
            {
                if (!new FiiiFinanceAgent().ValidateAddress(crypto.Code, address))
                    throw new CommonException(ReasonCode.GENERAL_ERROR, GeneralResources.EMInvalidAddress);
            }
            catch (FiiiFinanceException ex)
            {
                if (ex.ReasonCode == 20002)
                    throw new CommonException(ReasonCode.GENERAL_ERROR, GeneralResources.EMInvalidAddress);
            }

            var dac = new CryptoAddressDAC();
            var entities = new CryptoAddress
            {
                AccountId = merchantAccountId,
                AccountType = AccountType.Merchant,
                CryptoId = cryptoId,
                Address = address,
                Tag = tag,
                Alias = remark
            };
            dac.Insert(entities);
        }

        public void DeleteAddress(Guid merchantAccountId, long addressId)
        {
            var dac = new CryptoAddressDAC();

            var entities = dac.GetById(addressId);
            if (entities?.AccountId != merchantAccountId)
            {
                return;
            }

            dac.Delete(addressId);
        }
    }
}