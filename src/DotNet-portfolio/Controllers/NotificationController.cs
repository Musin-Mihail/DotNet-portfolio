using System;
using DotNet_portfolio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNet_portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            IMessageProducer messageProducer,
            ILogger<NotificationController> logger
        )
        {
            _messageProducer = messageProducer;
            _logger = logger;
        }

        /// <summary>
        /// Receives a message and sends it to the RabbitMQ queue.
        /// </summary>
        /// <param name="message">A simple string message from the request body.</param>
        [HttpPost]
        public IActionResult SendNotification([FromBody] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message cannot be empty.");
            }

            try
            {
                _logger.LogInformation(
                    "API received request to send notification: {Message}",
                    message
                );
                _messageProducer.SendMessage(message);
                return Ok(new { status = "Notification sent to queue successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification via API.");
                return StatusCode(
                    500,
                    "An internal error occurred while sending the notification."
                );
            }
        }
    }
}
