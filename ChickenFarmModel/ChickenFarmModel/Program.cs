using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChickenFarmModel
{
    public delegate void priceCutEvent(Int32 price);

    class Program
    {
        private static Int32 NUM_RETAILERS = 5;

        static void Main(string[] args)
        {
            ChickenFarm farmRamRod = new ChickenFarm();
            Thread farmer = new Thread(new ThreadStart(farmRamRod.farmerFunc));
            farmer.Start();         // Start one farmer thread
            Retailer chickenstore = new Retailer();
            ChickenFarm.priceCut += new priceCutEvent(chickenstore.chickenOnSale);
            Thread[] retailers = new Thread[NUM_RETAILERS];
            for (int i = 0; i < NUM_RETAILERS; i++)
            {   // Start N retailer threads
                retailers[i] = new Thread(new ThreadStart(chickenstore.retailerFunc));
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
            }

            // farmer.Join(); // is this necessary?
            // For debugging purposes... TEST THIS STUFF
            // Should retailers join first?
            
        }
    }

    class Retailer
    {
        // for starting thread
        public void retailerFunc()
        {
            // Possibly put the priceCut += priceCutEvent here instead of Main
            // ChickenFarm.getChickenPrice();
        }

        // event handler for when the price is cut
        public void chickenOnSale(Int32 newPrice)
        {
            // TODO: evaluate price and order

            // NEED A TIMESTAMP WHEN SUBMITTED
            // TIME PASSED WILL BE CALCUALTED AND SAVED
            // SHOULD SECOND PART OF THIS TRANSACTION OCCUR IN THE ORDERPROCESSING THREAD?
        }
    }

    class ChickenFarm
    {
        private static Int32 chickenPrice;
        private static Int32 numChickens;
        private static Int32 priceCutCounter = 0;

        public static Int32 getNumChickens() { return numChickens; }
        public static Int32 getChickenPrice() { return chickenPrice; }

        private const float TAXRATE = 0.08f;
        private const Int32 SHIPPINGRATE = 2;

        public static event priceCutEvent priceCut;

        public void farmerFunc()
        {
            //NEED A TIME STAMP FOR BEGINNING/END

            for (Int32 i = 0; i < 50; i++)
            {
                Thread.Sleep(500); //?

                Int32 newPrice = PricingModel.reevaluatePrice();
                ChickenFarm.changePrice(newPrice);
            }
        }

        // Changes the price according to the PricingModel and emits the event callback
        public static void changePrice(Int32 price)
        {
            // TODO: Don't let anything else access chickenPrice (box it?)
            // The OrderProcessing threads will try to access chickenPrice
            if (price < chickenPrice)
            {
                priceCutCounter++;
                if (priceCut != null)
                {
                    priceCut(price);
                }
                // IF priceCutCounter == 10, TERMINATE THREAD
                // should this check occur in Main?
            }
            chickenPrice = price;
            // END TODO
        }

        public static void processOrder(Order order)
        {
            // TODO: check credit card validity

            // Calculate order price
            Int32 subtotal, total, tax, shipping;
            subtotal = order.getUnitPrice() * order.getAmount();
            tax = (Int32)(subtotal * TAXRATE);
            shipping = order.getAmount() * SHIPPINGRATE;
            total = subtotal + tax + shipping;

            // TODO: confirm this with retailer
        }

        private class PricingModel
        {
            public static Int32 reevaluatePrice()
            {
                return 5000 / ChickenFarm.getNumChickens();
            }
        }
    }

    class Encoder
    {
        public static string encode(Order order)
        {
            return (order.getThreadId() + ";" + order.getCardNum() + ";" + order.getAmount());
        }
    }

    class Decoder
    {
        public static Order decode(string orderStr){
		    Order order = new Order();
		    order.setThreadId(Int32.Parse(orderStr.Substring(0, orderStr.IndexOf(';')-1)));
		    order.setCardNum(Int32.Parse(orderStr.Substring(orderStr.IndexOf(';')+1, orderStr.LastIndexOf(';')-1)));
	    	order.setAmount(Int32.Parse(orderStr.Substring(orderStr.LastIndexOf(';')+1)));
            return order;
    	}
    }

    class Order
    {
        private Int32 threadId;
        private Int32 cardNum;
        private Int32 amount;
        private Int32 unitPrice;

        public Int32 getThreadId(){ return threadId; }
        public Int32 getCardNum(){ return cardNum; }
        public Int32 getAmount(){ return amount; }
        public Int32 getUnitPrice(){ return unitPrice; }
        public void setThreadId(int threadId){ this.threadId = threadId; }
        public void setCardNum(int cardNum){ this.cardNum = cardNum; }
        public void setAmount(int amount){ this.amount = amount; }
        public void setUnitPrice(Int32 unitPrice){ this.unitPrice = unitPrice; }
        public String ToString(){ return threadId + ";" + cardNum + ";" + amount; }
    }

    // Multicell buffer that deals with Retailers and the ChickenFarm thread trying to
    // access it at the same time.
    class OrderBuffer
    {

    }
}
