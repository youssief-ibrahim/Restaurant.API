using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Services.PaymentSR;
using Stripe;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly IConfiguration config;
        public PaymentController(IPaymentService paymentService, IConfiguration config)
        {
            this.paymentService = paymentService;
            this.config = config;
        }
        [HttpPost("create-payment-intent/{cartId}")]
        public async Task<IActionResult> CreateOrUpdatePaymentIntent(int cartId)
        {
            var paymentIntent = await paymentService.CreateOrUpdatePaymentIntentAsync(cartId);
            return Ok(paymentIntent);
        }
        [HttpPost("test-confirm/{intentId}")]
        public async Task<IActionResult> TestConfirm(string intentId)
        {
            await paymentService.ConfirmPaymentAsync(intentId);
            return Ok();
        }
        //[HttpPost("webhook")]
        //public async Task<IActionResult> StripeWebhook()
        //{
        //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        //    Event stripeEvent;
        //    try
        //    {
        //        stripeEvent = EventUtility.ConstructEvent(
        //            json,
        //            Request.Headers["Stripe-Signature"],
        //            config["Stripe:WebhookSecret"]
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        //return BadRequest("Invalid Stripe webhook");
        //        var err = ex.Message;
        //        return BadRequest(err);
        //    }
          
        //    if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
        //    {
        //        var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
        //        await paymentService.ConfirmPaymentAsync(paymentIntent.Id);
        //    }
        //    else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
        //    {
        //        var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
        //        await paymentService.FailPaymentAsync(paymentIntent.Id);
        //    }
        //    return Ok();
        //}

    }
}
