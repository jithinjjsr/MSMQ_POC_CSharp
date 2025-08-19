using System;
using System.Messaging;

class Receiver
{
    static void Main()
    {
        //string queuePath = @".\Private$\MyTestQueue";
        string queuePath = @".\Private$\MySecondTestQueue";
        using (MessageQueue mq = new MessageQueue(queuePath))
        {
            mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            mq.ReceiveCompleted += (sender, e) =>
            {
                try
                {
                    var msg = mq.EndReceive(e.AsyncResult);
                    Console.WriteLine("Received: " + msg.Body.ToString());
                }
                catch (MessageQueueException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                // Continue listening for the next message
                mq.BeginReceive();
            };

            // Start listening for messages
            mq.BeginReceive();

            Console.WriteLine("Listening for messages. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}