using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFarmModel
{
    class OrderProcessing
    {
        private Order pOrder;
        public const float TAXRATE = .08f;
        public const float SHIPPINGRATE = 2f;
        public void orderProcessing()
        {
            float tax, shipping;
            Int32 subtotal, total;
            if (pOrder.getAmount() <= (ChickenFarm.getNumChickens() - 2))
            {
                if ((pOrder.getCardNum() > 999) && (pOrder.getCardNum() < 10000))
                {

                    subtotal = pOrder.getUnitPrice() * pOrder.getAmount();
                    tax = subtotal * TAXRATE;
                    shipping = pOrder.getAmount() * SHIPPINGRATE;
                    total = (Int32)(subtotal + tax + shipping);
                    ChickenFarm.setNumChickens(ChickenFarm.getNumChickens() - pOrder.getAmount());
                    
                    // Order finished timestamp
                    DateTime endTime = DateTime.Now.ToUniversalTime();
                    TimeSpan span = endTime.Subtract(pOrder.getStartTime());

                    //send receipt (output to console)
                    Console.Out.WriteLine("Retailer {0} bought {1} chickens for a total of ${2} in {3}ms!", pOrder.getThreadId(), pOrder.getAmount(), total, span.TotalMilliseconds);
                }
            }
        }
        public void setOrder(Order pOrder)
        {
            this.pOrder = pOrder;
        }
    }
}
