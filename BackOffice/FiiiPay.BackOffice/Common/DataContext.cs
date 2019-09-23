using FiiiPay.BackOffice.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using FiiiPay.BackOffice.Models.FiiiPay;
using FiiiPay.Entities;

namespace FiiiPay.BackOffice.Common
{
    public class DataContext
    {
        public static T GetDbContext<T>() where T : SugarDBContext, new()
        {
            var key = "sugardb" + typeof(T).Name;
            T db = CallContext.GetData(key) as T;
            if (db == null)
            {
                db = System.Activator.CreateInstance<T>();
                CallContext.SetData(key, db);
            }
            return db;
        }
    }

    public abstract class SugarDBContext
    {
        public virtual SqlSugarClient DB { get; set; }
    }

    public class BOContext: SugarDBContext
    {
        public static string BoSqlString = System.Configuration.ConfigurationManager.ConnectionStrings["bosqlstring"].ConnectionString;

        public BOContext()
        {

        }
        private SqlSugarClient _db;
        public override SqlSugarClient DB
        {
            get
            {
                if (_db == null)
                    _db = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = BoSqlString,
                        DbType = DbType.SqlServer,
                        IsAutoCloseConnection = true,
                        IsShardSameThread = true
                    });
                return _db;
            }
        }
        public DbSet<AdvanceOrders> AdvanceOrderDb { get { return new DbSet<AdvanceOrders>(DB); } }
        public DbSet<Account> AccountDb { get { return new DbSet<Account>(DB); } }
        public DbSet<AccountRole> AccountRoleDb { get { return new DbSet<AccountRole>(DB); } }
        public DbSet<ActionLog> ActionLogDb { get { return new DbSet<ActionLog>(DB); } }
        public DbSet<Agent> AgentDb { get { return new DbSet<Agent>(DB); } }
        public DbSet<MemberVerifyRecord> MemberVerifyRecordDb { get { return new DbSet<MemberVerifyRecord>(DB); } }
        public DbSet<MerchantVerifyRecord> MerchantVerifyRecordDb { get { return new DbSet<MerchantVerifyRecord>(DB); } }
        public DbSet<Module> ModuleDb { get { return new DbSet<Module>(DB); } }
        public DbSet<ModulePermission> ModulePermissionDb { get { return new DbSet<ModulePermission>(DB); } }
        public DbSet<RoleAuthority> RoleAuthorityDb { get { return new DbSet<RoleAuthority>(DB); } }
        public DbSet<Salesperson> SalespersonDb { get { return new DbSet<Salesperson>(DB); } }
        public DbSet<Setting> SettingDb { get { return new DbSet<Setting>(DB); } }
    }
    public class FiiiPayContext: SugarDBContext
    {
        public static string FiiipaySqlString = System.Configuration.ConfigurationManager.ConnectionStrings["fiiipay"].ConnectionString;
        public FiiiPayContext()
        {
            //DB.Aop.OnLogExecuted = (sql, pars) =>
            //{
            //    Console.WriteLine(sql + "\r\n" +
            //    DB.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            //    Console.WriteLine();
            //};
        }
        private SqlSugarClient _db;
        public override SqlSugarClient DB
        {
            get
            {
                if (_db == null)
                    _db = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = FiiipaySqlString,
                        DbType = DbType.SqlServer,
                        IsAutoCloseConnection = true,
                        IsShardSameThread = true,
                        ConfigureExternalServices = new ConfigureExternalServices()
                        {
                            EntityService = (property, column) =>
                            {
                                if(property.Name == "Id")
                                {
                                    column.IsPrimarykey = true;
                                }
                            }
                        }
                    });
                return _db;
            }
        }
        public DbSet<Advertisings> AdvertisingDb { get { return new DbSet<Advertisings>(DB); } }
        public DbSet<Articles> ArticlesDb { get { return new DbSet<Articles>(DB); } }
        public DbSet<CryptoAddresses> CryptoAddressDb { get { return new DbSet<CryptoAddresses>(DB); } }
        public DbSet<CryptoTransactions> CryptoTransactionDb { get { return new DbSet<CryptoTransactions>(DB); } }
        public DbSet<Feedbacks> FeedbacksDb { get { return new DbSet<Feedbacks>(DB); } }
        public DbSet<Files> FileDb { get { return new DbSet<Files>(DB); } }
        public DbSet<MerchantAccounts> MerchantAccountDb { get { return new DbSet<MerchantAccounts>(DB); } }
        public DbSet<MerchantDeposits> MerchantDepositDb { get { return new DbSet<MerchantDeposits>(DB); } }
        public DbSet<MerchantProfiles> MerchantProfileDb { get { return new DbSet<MerchantProfiles>(DB); } }
        public DbSet<MerchantWallets> MerchantWalletDb { get { return new DbSet<MerchantWallets>(DB); } }
        public DbSet<MerchantWalletStatements> MerchantWalletStatementDb { get { return new DbSet<MerchantWalletStatements>(DB); } }
        public DbSet<MerchantWithdrawals> MerchantWithdrawalDb { get { return new DbSet<MerchantWithdrawals>(DB); } }
        public DbSet<MerchantWithdrawalFees> MerchantWithdrawalFeeDb { get { return new DbSet<MerchantWithdrawalFees>(DB); } }
        public DbSet<Orders> OrderDb { get { return new DbSet<Orders>(DB); } }
        public DbSet<OrderStatusTrackings> OrderStatusTrackingDb { get { return new DbSet<OrderStatusTrackings>(DB); } }
        public DbSet<POSs> POSDb { get { return new DbSet<POSs>(DB); } }
        public DbSet<Refunds> RefundDb { get { return new DbSet<Refunds>(DB); } }
        public DbSet<UserAccounts> UserAccountDb { get { return new DbSet<UserAccounts>(DB); } }
        public DbSet<UserDeposits> UserDepositDb { get { return new DbSet<UserDeposits>(DB); } }
        public DbSet<UserLoginLogs> UserLoginLogDb { get { return new DbSet<UserLoginLogs>(DB); } }
        public DbSet<UserProfiles> UserProfileDb { get { return new DbSet<UserProfiles>(DB); } }
        public DbSet<VerifyRecords> VerifyRecordDb { get { return new DbSet<VerifyRecords>(DB); } }
        public DbSet<UserWallets> UserWalletDb { get { return new DbSet<UserWallets>(DB); } }
        public DbSet<UserWalletStatements> UserWalletStatementDb { get { return new DbSet<UserWalletStatements>(DB); } }
        public DbSet<UserWithdrawals> UserWithdrawalDb { get { return new DbSet<UserWithdrawals>(DB); } }
        public DbSet<UserWithdrawalFees> UserWithdrawalFeeDb { get { return new DbSet<UserWithdrawalFees>(DB); } }
        public DbSet<InvestorAccounts> InvestorAccountDb { get { return new DbSet<InvestorAccounts>(DB); } }
        public DbSet<InvestorWalletStatements> InvestorWalletStatementDb { get { return new DbSet<InvestorWalletStatements>(DB); } }
        public DbSet<InviteRecords> InviteRecordDb { get { return new DbSet<InviteRecords>(DB); } }
        public DbSet<ProfitDetails> ProfitDetailDb { get { return new DbSet<ProfitDetails>(DB); } }
        public DbSet<BillerOrders> BillerOrderDb { get { return new DbSet<BillerOrders>(DB); } }
        public DbSet<MerchantInformations> MerchantInformationDb { get { return new DbSet<MerchantInformations>(DB); } }
        public DbSet<MerchantOwnersFigures> MerchantOwnersFigureDb { get { return new DbSet<MerchantOwnersFigures>(DB); } }
        public DbSet<MerchantRecommends> MerchantRecommendDb { get { return new DbSet<MerchantRecommends>(DB); } }
        public DbSet<MerchantCategorys> MerchantCategoryDb { get { return new DbSet<MerchantCategorys>(DB); } }
        public DbSet<StoreTypes> StoreTypeDb { get { return new DbSet<StoreTypes>(DB); } }
        public DbSet<MerchantSupportCryptos> MerchantSupportCryptoDb { get { return new DbSet<MerchantSupportCryptos>(DB); } }
        public DbSet<FiiipayMerchantProfiles> FiiipayMerchantProfileDb { get { return new DbSet<FiiipayMerchantProfiles>(DB); } }
        public DbSet<FiiipayMerchantVerifyRecords> FiiipayMerchantVerifyRecordDb { get { return new DbSet<FiiipayMerchantVerifyRecords>(DB); } }
        public DbSet<StorePaySettings> StorePaySettingDb { get { return new DbSet<StorePaySettings>(DB); } }
        public DbSet<StoreBanners> StoreBannerDb { get { return new DbSet<StoreBanners>(DB); } }
        public DbSet<UserTransactions> UserTransactionDb { get { return new DbSet<UserTransactions>(DB); } }
    }

    public class FoundationContext : SugarDBContext
    {
        public static string FoundationSqlString = System.Configuration.ConfigurationManager.ConnectionStrings["foundation"].ConnectionString;
        public FoundationContext()
        {

        }
        private SqlSugarClient _db;
        public override SqlSugarClient DB
        {
            get
            {
                if (_db == null)
                    _db = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = FoundationSqlString,
                        DbType = DbType.SqlServer,
                        IsAutoCloseConnection = true,
                        IsShardSameThread = true
                    });
                return _db;
            }
        }
        
        public DbSet<AppVersions> AppVersionDb { get { return new DbSet<AppVersions>(DB); } }
        public DbSet<Countries> CountryDb { get { return new DbSet<Countries>(DB); } }
        public DbSet<BlobRouters> BlobRouterDb { get { return new DbSet<BlobRouters>(DB); } }
        public DbSet<FiiiFinanceRouters> FiiiFinanceRouterDb { get { return new DbSet<FiiiFinanceRouters>(DB); } }
        public DbSet<ProfileRouters> ProfileRouterDb { get { return new DbSet<ProfileRouters>(DB); } }
        public DbSet<Cities> CityDb { get { return new DbSet<Cities>(DB); } }
        public DbSet<Cryptocurrencies> CryptocurrencyDb { get { return new DbSet<Cryptocurrencies>(DB); } }
        public DbSet<States> StateDb { get { return new DbSet<States>(DB); } }
        public DbSet<PriceInfos> PriceInfoDb { get { return new DbSet<PriceInfos>(DB); } }
        public DbSet<Currency> CurrencyDb { get { return new DbSet<Currency>(DB); } }
        public DbSet<Files> FileDb { get { return new DbSet<Files>(DB); } }
        public DbSet<MasterSettings> MasterSettingDb { get { return new DbSet<MasterSettings>(DB); } }
    }
    public class FiiiPayEnterpriseContext : SugarDBContext
    {
        public static string FiiiPayEnterpriseSqlString = System.Configuration.ConfigurationManager.ConnectionStrings["fiiiPayenterprise"].ConnectionString;
        public FiiiPayEnterpriseContext()
        {

        }
        private SqlSugarClient _db;
        public override SqlSugarClient DB
        {
            get
            {
                if (_db == null)
                    _db = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = FiiiPayEnterpriseSqlString,
                        DbType = DbType.SqlServer,
                        IsAutoCloseConnection = true,
                        IsShardSameThread = true
                    });
                return _db;
            }
        }

        public DbSet<GatewayAccount> AccountsDb { get { return new DbSet<GatewayAccount>(DB); } }
        public DbSet<GatewayProfile> ProfilesDb { get { return new DbSet<GatewayProfile>(DB); } }
        public DbSet<BalanceStatement> BalanceStatementDb { get { return new DbSet<BalanceStatement>(DB); } }
    }
    public class DbSet<T>:SimpleClient<T> where T : class, new()
    {
        public DbSet(SqlSugarClient context) : base(context)
        {

        }
        public List<T> GetByIds(dynamic[] ids)
        {
            return Context.Queryable<T>().In(ids).ToList(); ;
        }
    }
}