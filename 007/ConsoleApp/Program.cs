﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        private static bool moreItemsToAdd = true;
        static void Main(string[] args)
        {
            // A bounded collection. It can hold no more
            // than 100 items at once.
            BlockingCollection<Data> dataItems = new BlockingCollection<Data>(100);
            
            // A simple blocking consumer with no cancellation.
            Task.Run(() =>
                {
                    while (!dataItems.IsCompleted)
                    {

                        Data data = null;
                        // Blocks if dataItems.Count == 0.
                        // IOE means that Take() was called on a completed collection.
                        // Some other thread can call CompleteAdding after we pass the
                        // IsCompleted check but before we call Take.
                        // In this example, we can simply catch the exception since the
                        // loop will break on the next iteration.
                        try
                        {
                            data = dataItems.Take();
                        }
                        catch (InvalidOperationException) { }

                        if (data != null)
                        {
                            Process(data);
                        }
                    }
                    Console.WriteLine("\r\nNo more items to take.");
                });

                // A simple blocking producer with no cancellation.
                Task.Run(() =>
                {
                    while (moreItemsToAdd)
                    {
                        Data data = GetData();
                        // Blocks if numbers.Count == dataItems.BoundedCapacity
                        dataItems.Add(data);
                    }
                    // Let consumer know we are done.
                    dataItems.CompleteAdding();
                });
            }

        public static Data GetData() => new Data("Sample");

        public static void Process (Data data)
        {
            Console.WriteLine($"Processing....{data.Property}");
        }
    }

    public class Data
    {
        public Data(string property)
        {
            Property = property;
        }
        public string Property { get; set; }
    }
}
