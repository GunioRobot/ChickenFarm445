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
}
