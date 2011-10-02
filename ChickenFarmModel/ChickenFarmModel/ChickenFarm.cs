using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace ChickenFarmModel
{
    /**
     * The ChickenFarm class has the method farmerFunc which is run as a Thread (started in Program.Main())
     * This thread is responsible for keeping track of how many chickens are in the system while
     * adjusting the price accordingly. It sends out a price cut event callback to the Retailer
     * threads whenever the price is cut.
     */
    class ChickenFarm
    {
        private static Int32 chickenPrice;
        private static Int32 numChickens = 2500;
        private static Int32 priceCutCounter = 0; // used to tell when 10 pricecuts have occurred
        private static Int32 newPrice = 5;
        private OrderProcessing orderProcessingObject;

        public static Int32 getNumChickens() { return numChickens; }
        public static Int32 getChickenPrice() { return chickenPrice; }
        public static Int32 getNewPrice() { return newPrice; }
        public static void setNumChickens(Int32 chickensToSet)
        {
            numChickens = chickensToSet;
        }

        private const float TAXRATE = 0.08f;
        private const Int32 SHIPPINGRATE = 2;

        public static event priceCutEvent priceCut;
        public void recountChickens(object sender, ElapsedEventArgs e)
        {
            numChickens += numChickens / 2;
        }

        public void farmerFunc()
        {
            // Timestamp used to mark when the thread begins.
            DateTime startTime = new DateTime();
            startTime = DateTime.Now;

            System.Timers.Timer eventTimer = new System.Timers.Timer();
            eventTimer.Elapsed += new ElapsedEventHandler(recountChickens);
            eventTimer.Interval = 50;
            eventTimer.AutoReset = true;
            eventTimer.Start();

            while (priceCutCounter < 10)
            {
                Thread.Sleep(100);

                Console.Out.WriteLine("ChickenFarm has {0} chickens left", numChickens);
                newPrice = PricingModel.reevaluatePrice();
                ChickenFarm.changePrice(newPrice);

                lock (Program.orderBuffer)
                {
                    if (Program.orderBuffer.IsFull())
                    {
                        orderProcessingObject = new OrderProcessing();
                        for (int i = 0; i < Program.NUM_RETAILERS - 1; i++)
                        {
                            String s = Program.orderBuffer.Consume(i);
                            if (s != null)
                            {
                                Order cfOrder = Decoder.decode(s);
                                orderProcessingObject.setOrder(cfOrder);
                                Thread orderProcessingThread = new Thread(new ThreadStart(orderProcessingObject.orderProcessing));
                                orderProcessingThread.Start();
                            }
                        }
                    }
                }
            }

            // Timestamp used to mark when the thread ends.
            DateTime endTime = new DateTime();
            endTime = DateTime.Now;
            // Time span between start time and end time (thread duration)
            TimeSpan span = endTime.Subtract(startTime);
            System.Console.WriteLine("Total Elapsed Time for ChickenFarm Thread: {0} ms", span.TotalMilliseconds);
            System.Console.WriteLine("ChickenFarm DIES! :(");

            // do not wait for OrderProcessing threads to terminate! This is a poor design
            // choice if this software was implemented in real life, but for the sake of
            // finishing the assignment on time, we will not deal with it.
        }

        // Changes the price according to the PricingModel and emits the event callback
        public static void changePrice(Int32 price)
        {
            if (price <= chickenPrice)
            {
                priceCutCounter++;
                if (priceCut != null)
                {
                    Console.WriteLine("Price cut event executed! New price: " + price);
                    priceCut(price);
                }
            }
            chickenPrice = price;
        }

        public static void processOrder(Order order)
        {
            // Calculate order price
            Int32 subtotal, total, tax, shipping;
            subtotal = order.getUnitPrice() * order.getAmount();
            tax = (Int32)(subtotal * TAXRATE);
            shipping = order.getAmount() * SHIPPINGRATE;
            total = subtotal + tax + shipping;
        }

        private class PricingModel
        {
            public static Int32 reevaluatePrice()
            {
                Int32 newPrice = 5000 / ChickenFarm.getNumChickens();
                if (newPrice < 1) newPrice++;
                return newPrice;
            }
        }
    }
}
