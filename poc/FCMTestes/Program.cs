using System;

namespace FCMTestes
{
    class Program
    {
        protected Program()
        {
            
        }
        static void Main(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            Console.WriteLine("FCMTestes");
        }
    }
}
