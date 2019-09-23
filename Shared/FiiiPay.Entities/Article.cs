using System;

namespace FiiiPay.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Descdescription { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool ShouldPop { get; set; }
        public ArticleAccountType AccountType { get; set; }
        public bool? HasPushed { get; set; }
    }

    /// <summary>
    /// 给谁看的
    /// </summary>
    public enum ArticleAccountType
    {
        FiiiPos = 0,
        FiiiPay
    }
}
