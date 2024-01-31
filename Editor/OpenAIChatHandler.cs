using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIChatHandler
{
    private const string apiUrl = "https://api.openai.com/v1/chat/completions";

    public static string SendChatMessage(string apiKey, string model, string[] messages, float temperature)
    {
        // Prepare the request data
        ChatRequest chatRequest = new ChatRequest(model, messages, temperature);

        // Convert the request data to JSON
        string requestData = JsonUtility.ToJson(chatRequest);

        // Create a UnityWebRequest to send the data to OpenAI
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request
        request.SendWebRequest();

        while (!request.isDone) { }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            return null;
        }
        else
        {
            // Parse the response
            string responseText = request.downloadHandler.text;
            ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(responseText);

            // Log the conversation and token usage
            //LogConversation(chatResponse);

            // Return the AI's response (chatResponse.choices[0].message)
            return chatResponse.choices[0].message.content;
        }
    }

    [System.Serializable]
    public class ChatRequest
    {
        public string model;
        public List<Message> messages;
        public float temperature;

        [System.Serializable]
        public class Message
        {
            public string role;
            public string content;
        }

        public ChatRequest(string model, string[] messages, float temperature)
        {
            this.model = model;
            this.messages = new List<Message>();
            foreach (var message in messages)
            {
                this.messages.Add(new Message
                {
                    role = "user",
                    content = message
                });
            }
            this.temperature = temperature;
        }
    }

    [System.Serializable]
    public class ChatResponse
    {
        public string id;
        public string @object;
        public long created;
        public string model;

        public Choice[] choices;

        [System.Serializable]
        public class Choice
        {
            public int index;
            public Message message;
            public string finish_reason;
        }

        [System.Serializable]
        public class Message
        {
            public string role;
            public string content;
        }

        public Usage usage;

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }
}
