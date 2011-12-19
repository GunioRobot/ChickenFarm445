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
        Int32 myId;
        public static Int32 nPrice = 5000;

        public Retailer(int id)
        {
            myId = id;
            priceWasCutArray = new bool[Program.NUM_RETAILERS];
            for (int i = 0; i < Program.NUM_RETAILERS; i++)
            {
                priceWasCutArray[i] = false;
            }
        }

        public Int32 getId()
        {
            return myId;
        }

        // method to be run as the 'retailer thread'
        public void retailerFunc()
        {
            while (Program.ChickenFarmIsAlive())
            {
                //sleep on startup of the loop (whenever the retailer finishes ordering)
                Thread.Sleep(100);

                if (priceWasCutArray[getId()])
                {
                    Random randNum = new Random();

                    // set the number of chickens to order
                    Int32 numOfChickensToOrder = 980 / (nPrice);
                    Order newOrder = new Order();
                    newOrder.setUnitPrice(nPrice);
                    newOrder.setAmount(numOfChickensToOrder);
                    //Sets the order credit card number between 1000 and 9999
                    newOrder.setCardNum(randNum.Next(1000, 9999));
                    newOrder.setThreadId(this.myId);

                    //encode the order as a string
                    String encodedOrder = Encoder.encode(newOrder);

                    // keep sleeping until the buffer has an open cell
                    while (Program.orderBuffer.IsFull())
                    {
                        Thread.Sleep(randNum.Next(10, 100));
                    }

                    // lock the buffer so the thread can input its encoded order
                    lock (Program.orderBuffer)
                    {
                        Program.orderBuffer.setCell(encodedOrder);
                    }

                    // indicate that the ordering has been submitted and that the thread
                    // shouldn't order again until the price is cut again
                    priceWasCutArray[getId()] = false;
                }
            }
        }

        // event handler for when the price is cut
        public void chickenOnSale(Int32 newPrice)
        {
            nPrice = newPrice;
            lock (priceWasCutArray)
            {
                for(int i = 0; i < priceWasCutArray.Length; i++) priceWasCutArray[i] = true;
            }
        }
    }
}
