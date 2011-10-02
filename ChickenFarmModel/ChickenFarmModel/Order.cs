using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFarmModel
{
    // the order class describes a set of order objects that the retailer
    // creates when ordering chickens from the ChickenFarm thread
    class Order
    {
        public static String myDateFormat = "MM/dd/yyyy hh:mm:ss.ffff tt";
        private Int32 threadId;
        private Int32 cardNum;
        private Int32 amount;
        private Int32 unitPrice;
        private DateTime startTime;

        public Order()
        {
            startTime = DateTime.Now;
            startTime = startTime.ToUniversalTime();
        }

        public Int32 getThreadId() { return threadId; }
        public Int32 getCardNum() { return cardNum; }
        public Int32 getAmount() { return amount; }
        public Int32 getUnitPrice() { return unitPrice; }
        public DateTime getStartTime() { return startTime; }
        public void setThreadId(int threadId) { this.threadId = threadId; }
        public void setCardNum(int cardNum) { this.cardNum = cardNum; }
        public void setAmount(int amount) { this.amount = amount; }
        public void setUnitPrice(Int32 unitPrice) { this.unitPrice = unitPrice; }
        public void setStartTime(DateTime startTime) { this.startTime = startTime; }
        public override String ToString() { return threadId + ";" + cardNum + ";" + amount + "," + unitPrice + "=" + startTime.ToString(Order.myDateFormat); }
    }
}
