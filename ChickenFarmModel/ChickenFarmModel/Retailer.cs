using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChickenFarmModel
{
    class Retailer
    {
        public static bool[] priceWasCutArray;
        bool priceWasCut = false;
        Int32 myId;
        public static Int32 nPrice = 5;

        public Retailer(int id)
        {
            myId = id;
            priceWasCutArray = new bool[Program.NUM_RETAILERS];
            for (int i = 0; i < Program.NUM_RETAILERS; i++)
            {
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
                    Int32 numOfChickensToOrder = 700 / (nPrice);
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
}
