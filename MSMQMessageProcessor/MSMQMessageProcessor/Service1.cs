using System;
using System.Diagnostics;
using System.Messaging;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQMessageProcessor
{
    public partial class Service1 : ServiceBase
    {
        private string sourceQueuePath = @".\Private$\MyTestQueue";
        private string destinationQueuePath = @".\Private$\MySecondTestQueue";
        private MessageQueue sourceQueue;
        private MessageQueue destinationQueue;
        private bool isRunning = false;
        private Task processingTask;
        private CancellationTokenSource cancellationTokenSource;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("MSMQProcessor", "OnStart called", EventLogEntryType.Information);

            try
            {
                InitializeQueues();
                EventLog.WriteEntry("MSMQProcessor", "Queues initialized successfully", EventLogEntryType.Information);

                isRunning = true;
                cancellationTokenSource = new CancellationTokenSource();

                EventLog.WriteEntry("MSMQProcessor", "Starting processing task...", EventLogEntryType.Information);
                processingTask = Task.Run(() => ProcessMessages(cancellationTokenSource.Token));

                EventLog.WriteEntry("MSMQProcessor", "Service started successfully", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("MSMQProcessor", $"OnStart failed: {ex.Message}\nStack: {ex.StackTrace}", EventLogEntryType.Error);
                throw;
            }
        }

        protected override void OnStop()
        {
            isRunning = false;
            cancellationTokenSource?.Cancel();

            // Wait for processing task to complete
            processingTask?.Wait(5000);

            // Clean up resources
            sourceQueue?.Close();
            destinationQueue?.Close();
        }

        private void InitializeQueues()
        {
            // Create source queue if it doesn't exist
            if (!MessageQueue.Exists(sourceQueuePath))
            {
                MessageQueue.Create(sourceQueuePath);
            }

            // Create destination queue if it doesn't exist
            if (!MessageQueue.Exists(destinationQueuePath))
            {
                MessageQueue.Create(destinationQueuePath);
            }

            // Initialize queue objects
            sourceQueue = new MessageQueue(sourceQueuePath);
            destinationQueue = new MessageQueue(destinationQueuePath);

            // Set formatter for reading messages
            sourceQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
        }

        private void ProcessMessages(CancellationToken cancellationToken)
        {
            EventLog.WriteEntry("MSMQProcessor", "ProcessMessages method started", EventLogEntryType.Information);

            int loopCount = 0;

            while (isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    loopCount++;
                    if (loopCount % 100 == 0) // Log every 100 iterations
                    {
                        EventLog.WriteEntry("MSMQProcessor", $"Processing loop iteration: {loopCount}", EventLogEntryType.Information);
                    }

                    sourceQueue.MessageReadPropertyFilter.SetAll();

                    Message message = sourceQueue.Receive(TimeSpan.FromSeconds(1));

                    if (message != null)
                    {
                        EventLog.WriteEntry("MSMQProcessor", "Message received from source queue", EventLogEntryType.Information);

                        string originalMessage = message.Body.ToString();
                        string modifiedMessage = originalMessage + " (modified by service JR)";

                        destinationQueue.Send(modifiedMessage, "Processed Message");

                        EventLog.WriteEntry("MSMQProcessor", $"Message processed: {originalMessage} -> {modifiedMessage}", EventLogEntryType.Information);
                    }
                }
                catch (MessageQueueException ex)
                {
                    if (ex.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                    {
                        EventLog.WriteEntry("MSMQProcessor", $"MSMQ Error: {ex.Message} (Code: {ex.MessageQueueErrorCode})", EventLogEntryType.Error);
                    }
                    // Don't log timeout exceptions as they're normal
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("MSMQProcessor", $"Unexpected error: {ex.Message}\nStack: {ex.StackTrace}", EventLogEntryType.Error);
                    Thread.Sleep(1000);
                }
            }

            EventLog.WriteEntry("MSMQProcessor", "ProcessMessages method ended", EventLogEntryType.Information);
        }
    }
}
