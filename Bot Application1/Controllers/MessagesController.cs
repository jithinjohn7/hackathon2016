using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;
                string  message = activity.Text;
                Task<dynamic> re = MakeRequestToTASAPI(message);
                dynamic res  =  await re;
                // return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                Activity reply = activity.CreateReply($"{res.Score}");

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        async Task<dynamic> MakeRequestToTASAPI(string message)
        {
            try
            {
                string myComment = message.Replace(' ', '+');
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{3c8a96c7-6639-4739-ac98-da2e7203e93f}");
                var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?" + myComment;

                byte[] byteData = Encoding.UTF8.GetBytes("{body}");
                HttpResponseMessage response;
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("< your content type, i.e. application/json >");
                    response = await client.PostAsync(uri, content);
                }
                dynamic product = null;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    product = JsonConvert.DeserializeObject(data);
                    return product.Score;
                }
                else
                {
                    message = product.ReasonPhrase;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return message;
        }

            

        }
    }