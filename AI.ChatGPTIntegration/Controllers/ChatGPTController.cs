using AI.ChatGPTIntegration.DAL;
using AI.ChatGPTIntegration.Models.Config;
using AI.ChatGPTIntegration.Models.Domain;
using AI.ChatGPTIntegration.Models.Requests;
using AI.ChatGPTIntegration.Services.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace AI.ChatGPTIntegration.Controllers
{
    /// <summary>
    /// Endpoints for interacting with OpenAI's ChatGPT and Embedding models.
    /// </summary>
    [Route("api/chatgpt")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        private readonly ILogger<ChatGPTController> _logger;
        private readonly IHttpService httpService;
        private readonly OpenAISettings openAISettings;
        private readonly FAQRepository faqRepository;

        public ChatGPTController(ILogger<ChatGPTController> logger, IHttpService httpService, IOptions<OpenAISettings> openAISettings, FAQRepository faqRepository)
        {
            _logger = logger;
            this.httpService = httpService;
            this.openAISettings = openAISettings.Value;
            this.faqRepository = faqRepository;
            //guard clauses, require the openai settings
            if(this.openAISettings == null || 
                string.IsNullOrEmpty(this.openAISettings.OpenAIKey) || 
                string.IsNullOrEmpty(this.openAISettings.OpenAIChatUrl) || 
                string.IsNullOrEmpty(this.openAISettings.OpenAIEmbeddingUrl))
            {
                this._logger.LogError("OpenAI settings are not configured properly in appsettings.json");
                throw new ArgumentException("OpenAI settings are not configured properly in appsettings.json");
            }
        }

        /// <summary>
        /// Will perform a simple chat operation against OpenAI's ChatGPT endpoint.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
                model = openAISettings.ChatModel,
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
            string textResult = ParseOpenAIChatOutput(result);
            return new OkObjectResult(new {message = textResult});            
        }

        [HttpPost]
        [Route("simpleimagerecognition")]
        public async Task<IActionResult> PerformSimpleImageRecognition()
        {
            //create a request to OpenAI's chat endpoint
            //first read the image and convert to base64 string (when we have it on disk). If we dont have it on disk, we can also use a public URL to an image.
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "workshop.jpg");
            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
            var base64Image = Convert.ToBase64String(imageBytes);
            //anonymous object for the payload
            var payload = new
            {
                model = openAISettings.ChatModel,
                messages = new object[]
               {
                    new {
                        role = "system",
                        content = "Du er en billedanalyse service som skal hjælpe mig med at beskrive billeder. Du må gerne svare som du er karakteren 'Jarvis' fra Ironman filmene."
                    },
                    new {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = "Hvad forestiller dette billede?" },
                            new { type = "image_url", image_url = new { url = "data:image/png;base64," + base64Image } }
                            //new { type = "image_url", image_url = new { url = "https://www.ucn.dk/media/lhfpc33k/to-studerende-star-op-af-vaeggen-og-smiler.jpg?width=720&height=470&mode=crop" } }
                        }
                    }
               }
            };

            //add authorization header
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {openAISettings.OpenAIKey}");
            //send it
            var result = await this.httpService.PostAsync(openAISettings.OpenAIChatUrl, payload, headers);
            string textResult = ParseOpenAIChatOutput(result);
            return new OkObjectResult(new { message = textResult });
        }

        /// <summary>
        /// Will get all answers from the FAQ database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getallanswers")]
        public IActionResult GetAllAnswers()
        {
            var answers = this.faqRepository.GetAll();
            return new OkObjectResult(answers);
        }

        /// <summary>
        /// Will save an answer in the FAQ database after generating an embedding for it.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("flow1_saveanswer_in_db")]
        public async Task<IActionResult> Flow1SaveAnswerInDatabase(FaqAnswerRequest request)
        {
            // Validate the request
            var validator = new FaqAnswerRequest.FaqAnswerRequestValidator();
            validator.ValidateAndThrow(request);
            //if we get this far, the request is valid
            //create a request to OpenAI's chat endpoint
            //anonymous object for the payload
            var payload = new
            {
                model = openAISettings.EmbeddingModel,
                input = request.Answer
            };

            //add authorization header
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {openAISettings.OpenAIKey}");
            //send it
            var result = await this.httpService.PostAsync(openAISettings.OpenAIEmbeddingUrl, payload, headers);
            var textResult = ParseOpenAIEmbeddingOutput(result);
            //create the domain entity and save it to the database
            FAQAnswer answer = new FAQAnswer(request.Answer, textResult);
            //use the repository to save it
            this.faqRepository.Save(answer);

            return new OkObjectResult(new { message = textResult });
        }

        /// <summary>
        /// Will perform a RAG operation to get a better answer to the user's question.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("flow2_generate_rag_answer")]
        public async Task<IActionResult> Flow2GenerateRAGAnswer(SimilarAnswersRequest request)
        {
            // Validate the request
            var validator = new SimilarAnswersRequest.SimilarAnswersRequestValidator();
            validator.ValidateAndThrow(request);
            //if we get this far, the request is valid
            //create a request to OpenAI's chat endpoint

            //add authorization header
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {openAISettings.OpenAIKey}");

            //step 1: get the embedding for the question
            //anonymous object for the payload
            var questionPayload = new
            {
                model = openAISettings.EmbeddingModel,
                input = request.Question
            };

            var result = await this.httpService.PostAsync(openAISettings.OpenAIEmbeddingUrl, questionPayload, headers);
            var questionResult = ParseOpenAIEmbeddingOutput(result);

            //get the top 5 similar answers from the database
            var relevantAnswers = this.faqRepository.SearchSimilar(questionResult, 5);
            //we have the top 5 answers, now build the new and enhanced prompt. 
            string systemPrompt = "Du er en hjælpsom FAQ service som kan hjælpe med at svare på spørgsmål omkring kæledyr. Hvis ikke du kender et svar kan du svare at det har du ikke data nok til at svare på.\n";
            string userPrompt = "Ud fra nedenstående svar skal du give et enkelt, sammehængende svar på spørgsmålet:\n";
            foreach(var answer in relevantAnswers)
            {
                userPrompt += $"Relateret svar: {answer.Answer}.\n";
            }
            userPrompt += $"Brugerens spørgsmål: {request.Question}";
            //log to console for visible confirmation
            this._logger.LogInformation("----- PROMPT -----");
            this._logger.LogInformation(userPrompt);
            this._logger.LogInformation("----- END PROMPT -----");

            //step 2 - build the new payload to the chatbot endpoint. Now with the "augmented" prompt
            var faqPayload = new
            {
                model = openAISettings.ChatModel,
                messages = new object[]
               {
                    new {
                        role = "system",
                        content = systemPrompt
                    },
                    new {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "text", text = userPrompt },
                        }
                    }
               }
            };
            //send it to chatgpt to generate the final answer (the G in RAG)
            var faqResult = await this.httpService.PostAsync(openAISettings.OpenAIChatUrl, faqPayload, headers);
            string textResult = ParseOpenAIChatOutput(faqResult);
            return new OkObjectResult(new { message = textResult });
        }

        /// <summary>
        /// Helpers to parse the output from OpenAI's chat endpoint.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private string ParseOpenAIChatOutput(JsonDocument document)
        {
            var content = document.RootElement
                                .GetProperty("choices")[0]
                                .GetProperty("message")
                                .GetProperty("content")
                                .GetString();

            var cleanedContent = Regex.Replace(content, "^```json\\s*|\\s*```$", "", RegexOptions.Multiline);
            return cleanedContent;
        }

        /// <summary>
        /// Helpers to parse the output from OpenAI's embedding endpoint.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private float[] ParseOpenAIEmbeddingOutput(JsonDocument document)
        {
            var content = document.RootElement.GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();
            return content;
        }

    }
}
