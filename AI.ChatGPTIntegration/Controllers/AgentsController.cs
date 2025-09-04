using AI.ChatGPTIntegration.Agents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI.ChatGPTIntegration.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private SimpleAgent simpleAgent;
        public AgentsController(SimpleAgent simpleAgent)
        {
            this.simpleAgent = simpleAgent ?? throw new ArgumentNullException(nameof(simpleAgent));
        }

        [HttpPost]
        public async Task<IActionResult> RunAgentsAsync()
        {
            string result = await this.simpleAgent.RunAsync();
            return Ok(new { message = result });
        }
    }
}
