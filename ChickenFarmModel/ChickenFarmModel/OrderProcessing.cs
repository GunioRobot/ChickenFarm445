using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFarmModel
{
    // The OrderProcessing class defines a method to be run as a thread whenever
    // the ChickenFarm receives an order and needs it to be processed
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
                    Console.Out.WriteLine("Retailer {0} bought {1} chickens for a total of ${2} at price {3} in {4}ms!", pOrder.getThreadId(), pOrder.getAmount(), total, pOrder.getUnitPrice(), span.TotalMilliseconds);
                }
            }
        }

        public void setOrder(Order pOrder)
        {
            this.pOrder = pOrder;
        }
    }
}
