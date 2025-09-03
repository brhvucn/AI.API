using AI.ChatGPTIntegration.Models.Config;
using AI.ChatGPTIntegration.Models.Requests;
using AI.ChatGPTIntegration.Services.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AI.ChatGPTIntegration.Controllers
{
    [Route("api/chatgpt")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        private readonly ILogger<ChatGPTController> _logger;
        private readonly IHttpService httpService;
        private readonly OpenAISettings openAISettings;

        public ChatGPTController(ILogger<ChatGPTController> logger, IHttpService httpService, IOptions<OpenAISettings> openAISettings)
        {
            _logger = logger;
            this.httpService = httpService;
            this.openAISettings = openAISettings.Value;
        }

        [HttpPost]
        [Route("simplechat")]
        public async Task<IActionResult> PerformSimpleChat(SimpleChatRequest request)
        {
            // Validate the request
            var validator = new SimpleChatRequest.SimpleChatRequestValidator();
            validator.ValidateAndThrow(request);
            //if we get this far, the request is valid
            //create a request to OpenAI's chat endpoint
            //anonymous object for the payload
            var payload = new
            {
                model = openAISettings.Model,
                messages = new object[]
               {
                    new {
                        role = "system",
                        content = "Du er en venlig programmerings-tutor som skal hjælpe med at kode C# op mod ChatGPT endpoint."
                    },
                    new {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = request.Message },                            
                        }
                    }
               }
            };

            //add authorization header
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {openAISettings.OpenAIKey}");            
            //send it
            var result = await this.httpService.PostAsync(openAISettings.OpenAIChatUrl, payload, headers);           
            string textResult = ParseOpenAIOutput(result);
            return new OkObjectResult(new {message = textResult});            
        }

        private string ParseOpenAIOutput(JsonDocument document)
        {
            var content = document.RootElement
                                .GetProperty("choices")[0]
                                .GetProperty("message")
                                .GetProperty("content")
                                .GetString();

            var cleanedContent = Regex.Replace(content, "^```json\\s*|\\s*```$", "", RegexOptions.Multiline);
            return cleanedContent;
        }
        
    }
}
