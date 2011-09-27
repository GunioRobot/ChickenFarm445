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
        public static Int32 NUM_RETAILERS = 5;
        private static Thread farmer;

        static void Main(string[] args)
        {
            Random randNum = new Random();
            bool childIsAlive = true;
            /* TIME STAMP STUFF, MOVE LATER */
            /* TIME STAMP STUFF, MOVE LATER */
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            TimeSpan span = endTime.Subtract(startTime);
            /* TIME STAMP STUFF, MOVE LATER */
            /* TIME STAMP STUFF, MOVE LATER */

            ChickenFarm farmRamRod = new ChickenFarm();
            farmer = new Thread(new ThreadStart(farmRamRod.farmerFunc));
            farmer.Start();         // Start one farmer thread
            Retailer chickenstore = new Retailer(-1);
            ChickenFarm.priceCut += new priceCutEvent(chickenstore.chickenOnSale);
            Thread[] retailers = new Thread[NUM_RETAILERS];
            for (int i = 0; i < NUM_RETAILERS; i++)
            {   // Start N retailer threads
                retailers[i] = new Thread(new ThreadStart(new Retailer(i).retailerFunc));
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
            }

            while (childIsAlive)
            {
                Thread.Sleep(randNum.Next(1, 100));

                childIsAlive = false;
                for (int i = 0; i < retailers.Length; i++)
                {
                    if (retailers[i].IsAlive) childIsAlive = true;
                }
            }
            // farmer.Join(); // is this necessary?
            // For debugging purposes... TEST THIS STUFF
            // Should retailers join first?
            
        }

        public static bool ChickenFarmIsAlive()
        {
            return farmer.IsAlive;
        }
    }

    class Retailer
    {
        public static bool[] priceWasCutArray;
        bool priceWasCut = false;
        Int32 myId;

        public Retailer(int id){
            myId = id;
            priceWasCutArray = new bool[Program.NUM_RETAILERS];
            for(int i = 0; i < Program.NUM_RETAILERS; i++){
                priceWasCutArray[i] = true;
            }
        }

        public Int32 getId()
        {
            return myId;
        }

        // for starting thread
        public void retailerFunc()
        {
            while (Program.ChickenFarmIsAlive())
            {
                Thread.Sleep(100);
                //if(getId() == 1)
                //System.Console.WriteLine("Retailers {0} starts loop", getId());
                if (priceWasCutArray[getId()])
                {
                    priceWasCut = false;
                    Console.WriteLine("Retailer {0} changed the bool to {1}.", getId(), priceWasCut);
                }
                //if(getId() == 1)
                //System.Console.WriteLine("Retailers {0} ends loop", getId());
                // Possibly put the priceCut += priceCutEvent here instead of Main
                // ChickenFarm.getChickenPrice();


                //System.Random randNum = new System.Random();

//                while (!OrderBuffer.setCell(orderString))
//                {
                    // Is this the wait we need?
//                    Thread.Sleep(randNum.Next(1, 100));
//                }
            }
        }

        // event handler for when the price is cut
        public void chickenOnSale(Int32 newPrice)
        {
            // TODO: evaluate price and order
            //lock (priceWasCutArray)
            //{
                //for (int i = 0; i < priceWasCutArray.Length; i++)
                //{
                    priceWasCut = true;
                //}
                //Console.WriteLine("BOOL CHANGED TO: {0}", priceWasCut);
            //}
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

            while (priceCutCounter < 10)
            {
                System.Console.WriteLine("ChickenFarm starts and sleeps");
                Thread.Sleep(100); //?
                System.Console.WriteLine("ChickenFarm wakes up");



                // PUT THIS IN THE TIMER WITH CHICKEN COUNTER
                // PUT THIS IN THE TIMER WITH CHICKEN COUNTER
                numChickens++;
                Int32 newPrice = PricingModel.reevaluatePrice();
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

                System.Console.WriteLine("ChickenFarm ends iteration");
            }

            // do not wait for OrderProcessing threads to terminate! This is a poor design
            // choice if this software was implemented in real life, but for the sake of
            // finishing the assignment on time, we will not deal with it.
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
        public override String ToString(){ return threadId + ";" + cardNum + ";" + amount; }
    }

    // Multicell buffer that deals with Retailers and the ChickenFarm thread trying to
    // access it at the same time.
    class OrderBuffer
    {
        public static String[] buffer;
        bool isFull = false;

        public OrderBuffer(){
            buffer = new String[Program.NUM_RETAILERS];
            for (int i = 0; i < Program.NUM_RETAILERS; i++)
            {
                buffer[i] = null;
            }
        }

        public bool setCell(String s)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == null)
                {
                    buffer[i] = s;
                    return true;
                }
            }
            return false;
        }

        public static bool IsEmpty()
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != null) return false;
            }
            return true;
        }

        public static string Consume(int index)
        {
            string s = buffer[index];
            buffer[index] = null;
            return s;
        }
    }
}
