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
        //bool isFull = false;

        public OrderBuffer()
        {
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
