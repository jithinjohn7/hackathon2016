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
using System.Collections.Generic;

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
                // int length = (activity.Text ?? string.Empty).Length;


                // return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");

                //await connector.Conversations.ReplyToActivityAsync(reply);
		const string apiKey = "07db8c70ffb04568b68a46f511b014c6";
		const string queryUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";

		var client = new HttpClient {
			DefaultRequestHeaders = {
				{"Ocp-Apim-Subscription-Key", apiKey},
				{"Accept", "application/json"}
			}
		};
		var sentimentInput = new BatchInput {
            documents = new List<DocumentInput> {
				new DocumentInput {
					id = 1,
					text = activity.Text,
				}
			}
		};
		var json = JsonConvert.SerializeObject(sentimentInput);
		var sentimentPost = await client.PostAsync(queryUri, new StringContent(json, Encoding.UTF8, "application/json"));
		var sentimentRawResponse = await sentimentPost.Content.ReadAsStringAsync();
		var sentimentJsonResponse = JsonConvert.DeserializeObject<BatchResult>(sentimentRawResponse);
		dynamic sentimentScore = sentimentJsonResponse?.documents?.FirstOrDefault()?.Score ?? 0;
		
		string message;
		if (sentimentScore > 0.7)
		{
			message = $"That's great to hear!";
		}
		else if (sentimentScore < 0.3)
		{
			message = $"I'm sorry to hear that...";
		}
		else
		{
			message = $"I see...";
		}
		var reply = activity.CreateReply(message);
		await connector.Conversations.ReplyToActivityAsync(reply);
	}
	else
	{
		//add code to handle errors, or non-messaging activities
	}
	var response = Request.CreateResponse(HttpStatusCode.OK);
	return response;";
                    const string queryUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";

                    var client = new HttpClient
                    {
                        DefaultRequestHeaders = {
                {"Ocp-Apim-Subscription-Key", apiKey},
                {"Accept", "application/json"}
            }
                    };
                    var sentimentInput = new BatchInput
                    {
                        Documents = new List<DocumentInput> {
                new DocumentInput {
                    Id = 1,
                    Text = activity.Text,
                }
            }
                    };
                    var json = JsonConvert.SerializeObject(sentimentInput);
                    var sentimentPost = await client.PostAsync(queryUri, new StringContent(json, Encoding.UTF8, "application/json"));
                    var sentimentRawResponse = await sentimentPost.Content.ReadAsStringAsync();
                    var sentimentJsonResponse = JsonConvert.DeserializeObject<BatchResult>(sentimentRawResponse);
                    var sentimentScore = sentimentJsonResponse?.Documents?.FirstOrDefault()?.Score ?? 0;

                    string message;
                    if (sentimentScore > 0.7)
                    {
                        message = $"That's great to hear!";
                    }
                    else if (sentimentScore < 0.3)
                    {
                        message = $"I'm sorry to hear that...";
                    }
                    else
                    {
                        message = $"I see...";
                    }
                    var reply = activity.CreateReply(message);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    //add code to handle errors, or non-messaging activities
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;



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

        
    


