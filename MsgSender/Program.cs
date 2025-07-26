using System;
using System.Messaging;

class Program
{
    static void Main()
    {
        string queuePath = @".\Private$\MyTestQueue";

        if (!MessageQueue.Exists(queuePath))
        {
            MessageQueue.Create(queuePath);
            Console.WriteLine("Queue created.");
        }
        string userInput = "N";
        do
        {
            Console.WriteLine("Enter the message to sent : ");
            string msg = Console.ReadLine();
            using (MessageQueue mq = new MessageQueue(queuePath))
            {
                mq.Send(msg);
                Console.WriteLine("Message sent to MSMQ.");
            }

            Console.WriteLine("Do you want to send enother message? (y/n) : ");
            userInput = Console.ReadLine();
        } while (userInput.ToLower().Trim() == "y" );
        
    }
}
