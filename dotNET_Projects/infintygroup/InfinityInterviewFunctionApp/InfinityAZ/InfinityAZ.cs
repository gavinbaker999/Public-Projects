using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;
using Microsoft.PowerPlatform.Dataverse.Client.Dynamics;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Linq;

namespace InfinityAZ
{
    public static class InfinityAZ
    {
        [FunctionName("InfinityAZ")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string info = "Dynamics 365 coonect failed.";
            string msg = "";

            const string clientId = "dd7b008a-23a2-4c58-93b4-35059d00b946";
            const string clientSecret = "lJ1XCDv_6ECAM2UU7QkEyXNIx~yp_6~0X1";
            const string environment = "orgdcfcc54b.crm11";
            var connectionString = @$"Url=https://{environment}.dynamics.com;AuthType=ClientSecret;ClientId={clientId};ClientSecret={clientSecret};RequireNewInstance=true";
            
            using var serviceClient = new ServiceClient(connectionString);
            if (serviceClient!=null && serviceClient.IsReady)
            {
                Guid recordId = new Guid();
                info = "Connected to Dynamics 365";

                //Dictionary<string, DataverseDataTypeWrapper> inData = new Dictionary<string, DataverseDataTypeWrapper>();
                //inData.Add("contactid", new DataverseDataTypeWrapper("C1000", DataverseFieldType.String));
                //inData.Add("fristname", new DataverseDataTypeWrapper("Gavin", DataverseFieldType.String));
                //inData.Add("lastname", new DataverseDataTypeWrapper("Baker", DataverseFieldType.String));
                //inData.Add("emailaddress1", new DataverseDataTypeWrapper("endhousesoftware999@gmail.com", DataverseFieldType.String));
                //recordId = serviceClient.CreateNewRecord("contact", inData);                                
                //if (recordId!=Guid.Empty) {
                //       info = "Connected to Dynamics 365 and created contact record.";
                //  } else {
                //        info = "Connected to Dynamics 365 and could NOT create contact record.";
                //  }

                var request = new CreateRequest();
                var newContact = new Entity("contact");
                newContact.Attributes.Add("contactid", Guid.NewGuid());
                newContact.Attributes.Add("firstname", "Gavin");
                newContact.Attributes.Add("lastname", "Baker");
                newContact.Attributes.Add("emailaddress1", "endhousesoftware999@gmail.com");
                request.Target = newContact;
                var response = (CreateResponse)serviceClient.Execute(request);
                recordId = response.id;

                // Fetch one contact record using FetchXML
                string fetchXML =
                    @"<fetch version='1.0' output-format='xml-platform' mapping='logical' count='1' distinct='false' returntotalrecordcount='true' >  
                        <entity name='contact'>  
                            <attribute name='contactid' />  
                            <attribute name='firstname' />  
                            <attribute name='lastname' />  
                            <attribute name='emailaddress1' />  
                        </entity>  
                    </fetch>";
                var queryResult = serviceClient.GetEntityDataByFetchSearchEC(fetchXML);
                if (queryResult != null)
                {
                    foreach (var c in queryResult.Entities)
                    {
                       msg = @$"{c.GetAttributeValue<string>("firstname")},{c.GetAttributeValue<string>("lastname")},{c.GetAttributeValue<string>("emailaddress1")},{c.GetAttributeValue<Guid>("contactid")}";
                    }

                    info = String.Format("Contact Record: {0} created and total records: {1}", recordId, queryResult.TotalRecordCount);
                }
            }
            else
            {
                // Display the last error.  
                info = @$"An error occurred: {serviceClient.LastError}";
                // Display the last exception message if any.   
                Console.WriteLine(serviceClient.LastException.Message);
                Console.WriteLine(serviceClient.LastException.Source);
                Console.WriteLine(serviceClient.LastException.StackTrace);
            }
            
            // Write the message to the Azure system bus queue
            await SendQueueMessage(msg);

            return new OkObjectResult(info);
        }

        public static async Task SendQueueMessage(string msg)
        {
            // connection string to your Service Bus namespace
            string connectionString = "Endpoint=sb://endhousesoftware.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=n2epOdHF2rE5VNQmVVoyHiYbu5oFT2hq4Q29HzHmD6c=";

            // name of your Service Bus queue
            string queueName = "ehsqueue";

            // the client that owns the connection and can be used to create senders and receivers
            ServiceBusClient client;

            // the sender used to publish messages to the queue
            ServiceBusSender sender;

            // Create the clients that we'll use for sending and processing messages.
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(queueName);

            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {msg}")))
            {
                // if it is too large for the batch
                throw new Exception($"The message is too large to fit in the batch.");
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A messages {msg} has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
