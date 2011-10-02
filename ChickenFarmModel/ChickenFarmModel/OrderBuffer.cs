using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFarmModel
{
    // Multicell buffer that deals with Retailers and the ChickenFarm thread trying to
    // access it at the same time.
    class OrderBuffer
    {
        public static String[] buffer;

        public OrderBuffer()
        {
            buffer = new String[Program.NUM_RETAILERS];
            for (int i = 0; i < Program.NUM_RETAILERS; i++)
            {
                buffer[i] = null;
            }
        }

        // producer method used to put something in the buffer
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

        // accessor method that tells whether or not the buffer is completely empty
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

        // accessor method that tells whether or not the buffer is full
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

        // consumer method used to remove something from the buffer
        public string Consume(int index)
        {
            string s = buffer[index];
            buffer[index] = null;
            return s;
        }
    }
}
