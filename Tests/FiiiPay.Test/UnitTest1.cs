using System;
using System.Collections.Generic;
using System.Diagnostics;
using FiiiPay.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiiiPay.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const int count = 10;

            var redPocket = new RedPocket() { Balance = 1000.12345678M, RemainCount = count };

            for (int i = 0; i < count; i++)
            {
                var v = GetRandomMoney(redPocket, 0.00000001M, 100000000);
                
                System.Diagnostics.Debug.Print(v.ToString());
            }
        }

        [TestMethod]
        public void TestMin()
        {
            var r = Min(8);
            var m = Max(8);

            Debug.Print(r.ToString());
            Debug.Print(m.ToString());
        }

        private decimal Min(int n)
        {
            var r = 10M;
            for (var index = 0; index < n; index++)
            {
                r /= 10;
            }

            return r;
        }

        private decimal Max(int n)
        {
            var r = 1M;
            for (var index = 0; index < n; index++)
            {
                r *= 10;
            }

            return r;
        }

        private string GetHeaders()
        {
            var password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "FiiiPay";
            var token = AES128.Encrypt(password, "nHf29ryC31hQXrzsSjM7bDYs8v7AT8n54tl3nHBrpB1TM9HhXJ48hOpQrzy9XrQB");

            return  token;
        }

        [TestMethod]
        public void Test1()
        {
            var token = GetHeaders();
            
            Debug.Print(token);
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void test()
        {
            var t = new TimeSpan(24,0,0).TotalMilliseconds;

            System.Diagnostics.Debug.Print(t.ToString());
        }

        private decimal GetRandomMoney(RedPocket redPackage, decimal min, int n)
        {
            // RemainCount 剩余的红包数量
            // Balance 剩余的钱
            if (redPackage.RemainCount <= 0) return 0;

            if (redPackage.RemainCount == 1)
            {
                redPackage.RemainCount--;
                return Math.Round(redPackage.Balance * n) / n;
            }

            Random r = new Random();
            decimal max = redPackage.Balance / redPackage.RemainCount * 2;
            decimal money = (decimal)r.NextDouble() * max;
            money = money <= min ? min : money;
            money = Math.Floor(money * n) / n;
            redPackage.RemainCount--;
            redPackage.Balance -= money;
            return money;
        }
    }

    class RedPocket
    {
        public int RemainCount { get; set; }

        public decimal Balance { get; set; }
    }
}
