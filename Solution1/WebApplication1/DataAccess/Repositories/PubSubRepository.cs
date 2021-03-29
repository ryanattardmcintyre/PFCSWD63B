using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Domain;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Grpc.Core;
using Google.Protobuf;
using Newtonsoft.Json;

namespace WebApplication1.DataAccess.Repositories
{
    public class PubSubRepository : IPubSubRepository
    {

        string projectId;
        string topicId;

        public PubSubRepository(IConfiguration config)
        {
            projectId = config.GetSection("AppSettings").GetSection("ProjectId").Value;
            topicId = config.GetSection("AppSettings").GetSection("TopicId").Value;
        }

        private Topic CreateTopic()
        {
            PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
            var topicName = TopicName.FromProjectTopic(projectId, topicId);
            Topic topic = null;

            try
            { 
                topic = publisher.GetTopic(topicName);
               
            }
            catch (RpcException e) when (e.Status.StatusCode == StatusCode.NotFound)
            {
               topic = publisher.CreateTopic(topicName);
            }
            return topic;
        }

        public void PublishEmail(string email, Blog b)
        {
            Topic t = CreateTopic();

            TopicName topicName = t.TopicName;
             var onTheFlyObject = new { email = email, blog = b };
             string serializedOnTheFlyObject = JsonConvert.SerializeObject(onTheFlyObject);
            Task<PublisherClient> task = PublisherClient.CreateAsync(topicName);
             task.Wait();
             PublisherClient publisher = task.Result;


             var pubsubMessage = new PubsubMessage
             {
                 // The data is any arbitrary ByteString. Here, we're using text.
                 Data = ByteString.CopyFromUtf8(serializedOnTheFlyObject
                     )
                 // The attributes provide metadata in a string-to-string dictionary.
                 //Attributes =
                 //{
                 //    { "year", "2020" },
                 //    { "author", "unknown" }
                 //}
             };

             Task<string> taskP = publisher.PublishAsync(pubsubMessage);
             taskP.Wait();
             string message = taskP.Result;

            //log message
        }
    }
}
