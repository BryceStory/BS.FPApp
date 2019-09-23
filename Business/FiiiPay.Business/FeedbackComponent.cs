using System;
using FiiiPay.Data;
using FiiiPay.Entities;

namespace FiiiPay.Business
{
    public class FeedbackComponent
    {
        public void Feedback(Guid AccountId, string Content, string Type)
        {
            new FeedBackDAC().Insert(new Feedback
            {
                AccountId = AccountId,
                Context = Content,
                HasProcessor = false,
                Timestamp = DateTime.UtcNow,
                Type = Type
            });
        }
    }
}
