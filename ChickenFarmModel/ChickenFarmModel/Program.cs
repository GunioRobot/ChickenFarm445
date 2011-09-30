using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace ChickenFarmModel
{
    public delegate void priceCutEvent(Int32 price);

    class Program
    {
        public static Int32 NUM_RETAILERS = 10;
        private static Thread farmer;
        public static OrderBuffer orderBuffer;        

        static void Main(string[] args)
        {
            Random randNum = new Random();
            bool childIsAlive = true;
            /* TIME STAMP STUFF, MOVE LATER */
            /* TIME STAMP STUFF, MOVE LATER */
            //DateTime startTime = new DateTime();
            //DateTime endTime = new DateTime();
            //TimeSpan span = endTime.Subtract(startTime);
            /* TIME STAMP STUFF, MOVE LATER */
            /* TIME STAMP STUFF, MOVE LATER */

            ChickenFarm farmRamRod = new ChickenFarm();
            //create buffer?
            orderBuffer = new OrderBuffer();
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
        public static Int32 nPrice = 5;

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

                    //is this the formula?
                    Int32 numOfChickensToOrder = 700 /(nPrice);
                    Order newOrder = new Order();
                    newOrder.setAmount(numOfChickensToOrder);
                    Random randNum = new Random();
                    //what should be card number range????
                    newOrder.setCardNum(randNum.Next(1000, 9999));
                    newOrder.setThreadId(this.myId);
                    String encodedOrder = Encoder.encode(newOrder);
                    //need to lock somehow and check before it sets
                    //needs to include isAvailable
                    if (!Program.orderBuffer.IsFull())
                    {
                        lock (Program.orderBuffer)
                        {
                            Program.orderBuffer.setCell(encodedOrder);

                            //Console.Out.WriteLine("Retailer {0} has set a cell in the buffer", this.myId);
                            //TODO: time stamp for order placed
                        }
                    }


                    priceWasCut = false;
                   // Console.WriteLine("Retailer {0} changed the bool to {1}.", getId(), priceWasCut);
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

                    nPrice = newPrice;
                    priceWasCut = true;

            //}
                //Console.WriteLine("BOOL CHANGED TO: {0}", priceWasCut);
            //}
            // NEED A TIMESTAMP WHEN SUBMITTED
            // TIME PASSED WILL BE CALCUALTED AND SAVED
            // SHOULD SECOND PART OF THIS TRANSACTION OCCUR IN THE ORDERPROCESSING THREAD?
        }
    }
    class OrderProcessing
    {
        private Order pOrder;
        public const float TAXRATE = .08f;
        public const float SHIPPINGRATE = 2f;
        public void orderProcessing()
        {
            float tax, shipping;
            Int32 subtotal, total;
            if (pOrder.getAmount() <= (ChickenFarm.getNumChickens()-2))
            {
                if ((pOrder.getCardNum() > 999) && (pOrder.getCardNum() < 10000))
                {

                    subtotal = pOrder.getUnitPrice() * pOrder.getAmount();
                    tax = subtotal * TAXRATE;
                    shipping = pOrder.getAmount() * SHIPPINGRATE;
                    total = (Int32)(subtotal + tax + shipping);
                    ChickenFarm.setNumChickens(ChickenFarm.getNumChickens() - pOrder.getAmount());
                    //send receipt
                    Console.Out.WriteLine("Retailer {0} ordered {1} chickens for a total price of {2}",pOrder.getThreadId(),pOrder.getAmount(),total);
                }
            }
        }
        public void setOrder(Order pOrder)
        {
            this.pOrder = pOrder;
        }
    }
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
            //NEED A TIME STAMP FOR BEGINNING/END
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
                        for (int i = 0; i < Program.NUM_RETAILERS-1; i++)
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
            DateTime endTime = new DateTime();
            endTime = DateTime.Now;
            TimeSpan span = endTime.Subtract(startTime);
            System.Console.WriteLine("Total Elapsed Time for ChickenFarm Thread: {0} ms",span.TotalMilliseconds);
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
            //Console.Out.WriteLine(orderStr.Substring(0, (orderStr.IndexOf(';'))-1));
            //Console.Out.WriteLine(orderStr.Substring(0, 1));
            //Console.Out.WriteLine(orderStr.Substring(orderStr.IndexOf(';')+1, 4));
            //Console.Out.WriteLine(orderStr.Substring(orderStr.LastIndexOf(';') + 1));
            //Console.Out.WriteLine(orderStr.Substring(orderStr.IndexOf(';')+1, (orderStr.LastIndexOf(';'))-1));
            //Console.Out.WriteLine(orderStr.Substring((orderStr.LastIndexOf(';')) + 1));
		    order.setThreadId(Int32.Parse(orderStr.Substring(0, (orderStr.IndexOf(';')))));
            //order.setThreadId(Int32.Parse(orderStr.Substring(0, 1)));
		    //order.setCardNum(Int32.Parse(orderStr.Substring(orderStr.IndexOf(';')+1, (orderStr.LastIndexOf(';'))-1)));
            order.setCardNum(Int32.Parse(orderStr.Substring(orderStr.IndexOf(';') + 1, 4)));
	    	order.setAmount(Int32.Parse(orderStr.Substring((orderStr.LastIndexOf(';'))+1)));

            //Console.Out.WriteLine(order.ToString());

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
        //bool isFull = false;

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

        public bool IsEmpty()
        {
            bool result = true;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != null)
                {
                    result = false;
                    return result;
                }
            
            }
            return result;
        }
        public bool IsFull()
        {
            bool result = false;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }

        public string Consume(int index)
        {
            string s = buffer[index];
            buffer[index] = null;
            return s;
        }
    }
}
