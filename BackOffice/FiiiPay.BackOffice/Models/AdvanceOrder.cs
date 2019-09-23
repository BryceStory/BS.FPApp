using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.BackOffice.Models
{
   public class AdvanceOrders
    {
        public int Id { get; set; }
        [JsonProperty("BookUserName")]
        public string OrderName { get; set; } //订单人

        [JsonProperty("CellPhone")]
        public string Phone { get; set; } //电话
        [JsonProperty("MerchantCount")]
        public string Amount { get; set; } //数量
        [JsonProperty("UnitPrice")]
        public string Price { get; set; } //单价
        [JsonProperty("TotalAmount")]
        public string Totalpayment { get; set; } //全款
        [JsonProperty("Advance")]
        public string Advance { get; set; } //预付款
        [JsonProperty("TransferName")]
        public string TransferName { get; set; } //转账人姓名
        [JsonProperty("TransferCardNo")]
        public string TransferNumber { get; set; } //转账人卡号
        public string Paymentstatus { get; set; } //付款状态
        public string  Shippingstatus { get; set; } //发货状态
        [JsonProperty("BookTime")]
        public DateTime Time { get; set; } //时间
        [JsonProperty("Seller")]
        public string Salesperson { get; set; } //销售人
        [JsonProperty("Remark")]
        public string Remark { get; set; } //备注
        [JsonProperty("BookAmount")]
        public string Amountpaid { get; set; } //已付金额
    }  
}
