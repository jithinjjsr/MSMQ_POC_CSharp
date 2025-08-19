using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
namespace MsgForwarder
{
    internal class Program
    {
        static void Main()
        {
            string sourceQueuePath = @".\Private$\MyTestQueue";
            string destinationQueuePath = @".\Private$\MySecondTestQueue";

            Console.WriteLine("Starting message processing test...");

            try
            {
                // Check if queues exist
                Console.WriteLine($"Checking if source queue exists: {MessageQueue.Exists(sourceQueuePath)}");
                Console.WriteLine($"Checking if destination queue exists: {MessageQueue.Exists(destinationQueuePath)}");

                using (var sourceQueue = new MessageQueue(sourceQueuePath))
                using (var destinationQueue = new MessageQueue(destinationQueuePath))
                {
                    sourceQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                    // Check message count
                    sourceQueue.Refresh();
                    Console.WriteLine($"Messages in source queue: {sourceQueue.GetAllMessages().Length}");

                    Console.WriteLine("Waiting for messages (10 second timeout)...");

                    try
                    {
                        Message message = sourceQueue.Receive(TimeSpan.FromSeconds(10));

                        Console.WriteLine("Message received!");
                        Console.WriteLine($"Message ID: {message.Id}");
                        Console.WriteLine($"Message Label: {message.Label}");

                        string originalMessage = message.Body.ToString();
                        Console.WriteLine($"Original message: {originalMessage}");

                        string modifiedMessage = originalMessage + " (modified by service JR)";

                        destinationQueue.Send(modifiedMessage, "Processed Message");

                        Console.WriteLine($"Message sent to destination queue: {modifiedMessage}");
                        Console.WriteLine("SUCCESS: Message processed successfully!");
                    }
                    catch (MessageQueueException ex)
                    {
                        Console.WriteLine($"MessageQueue Exception: {ex.Message}");
                        Console.WriteLine($"Error Code: {ex.MessageQueueErrorCode}");

                        if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                        {
                            Console.WriteLine("TIMEOUT: No messages received within 10 seconds");
                            Console.WriteLine("This means either:");
                            Console.WriteLine("1. No messages in the queue");
                            Console.WriteLine("2. Permission issues");
                            Console.WriteLine("3. Queue path is incorrect");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
