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
        private static Int32 priceCutCounter = 0;
        private static Int32 newPrice = 5;

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
        public void recountChickenss(object sender, ElapsedEventArgs e)
        {
            numChickens += numChickens / 2;
        }
        public void farmerFunc()
        {
            // Timestamp used to mark when the thread begins.
            DateTime startTime = new DateTime();
            startTime = DateTime.Now;

            System.Timers.Timer eventTimer = new System.Timers.Timer();
            eventTimer.Elapsed += new ElapsedEventHandler(recountChickenss);
            eventTimer.Interval = 50;
            eventTimer.AutoReset = true;
            eventTimer.Start();

            while (priceCutCounter < 10)
            {
                System.Console.WriteLine("ChickenFarm starts and sleeps");
                Thread.Sleep(100); //?
                System.Console.WriteLine("ChickenFarm wakes up");



                // PUT THIS IN THE TIMER WITH CHICKEN COUNTER
                // PUT THIS IN THE TIMER WITH CHICKEN COUNTER

                Console.Out.WriteLine("ChickenFarm has {0} chickens left", numChickens);
                newPrice = PricingModel.reevaluatePrice();
                ChickenFarm.changePrice(newPrice);
                // PUT THIS IN THE TIMER WITH CHICKEN COUNTER
                // PUT THIS IN THE TIMER WITH CHICKEN COUNTER
                /*
                // check buffer code
                lock (OrderBuffer.buffer) //double check this
                {
                    if (!OrderBuffer.IsEmpty())
                    {
                        for (int i = 0; i < Program.NUM_RETAILERS; i++)
                        {
                            // CHANGE THIS TO CREATE AN ORDER PROCESSING THREAD!
                            System.Console.WriteLine(Decoder.decode(OrderBuffer.Consume(i)));
                            // CHANGE THIS TO CREATE AN ORDER PROCESSING THREAD!
                        }
                    }
                }
                 */
                // check buffer code
                lock (Program.orderBuffer) //double check this
                {
                    if (Program.orderBuffer.IsFull())
                    {
                        OrderProcessing orderProcessingObject = new OrderProcessing();
                        for (int i = 0; i < Program.NUM_RETAILERS - 1; i++)
                        {
                            // CHANGE THIS TO CREATE AN ORDER PROCESSING THREAD!
                            //System.Console.WriteLine(Decoder.decode(Program.orderBuffer.Consume(i)));
                            String s = Program.orderBuffer.Consume(i);
                            if (s != null)
                            {
                                Order cfOrder = Decoder.decode(s);
                                //OrderProcessing orderProcessingObject = new OrderProcessing();
                                orderProcessingObject.setOrder(cfOrder);
                                Thread orderProcessingThread = new Thread(new ThreadStart(orderProcessingObject.orderProcessing));
                                orderProcessingThread.Start();
                                //ORDERPROCESS START with cfOrder
                            }


                            // CHANGE THIS TO CREATE AN ORDER PROCESSING THREAD!
                        }
                        Console.Out.WriteLine("ChickenFarm consumed all cells in buffer");

                    }
                }

                System.Console.WriteLine("ChickenFarm ends iteration");
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
            // TODO: Don't let anything else access chickenPrice (box it?)
            // The OrderProcessing threads will try to access chickenPrice
            if (price <= chickenPrice)
            {
                priceCutCounter++;
                if (priceCut != null)
                {
                    Console.WriteLine("Price cut event executed!");
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
                Int32 newPrice = 5000 / ChickenFarm.getNumChickens();
                if (newPrice < 1) newPrice++;
                return newPrice;
            }
        }
    }
}
