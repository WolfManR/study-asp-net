using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using TemplatesReporter.AuthenticationRules;
using TemplatesReporter.Mail.Core;
using TemplatesReporter.MailsSender.Data;

namespace TemplatesReporter.MailsSender.Controllers;

[Route("emailsend")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EmailSendSchedulerController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly EmailsRepository _emailsRepository;
    private readonly AuthorizationHelper _authorizationHelper;
    private readonly EmailConfiguration _domainConfiguration;

    public EmailSendSchedulerController(
        IEmailService emailService,
        IOptions<EmailConfiguration> domainConfiguration,
        EmailsRepository emailsRepository,
        AuthorizationHelper authorizationHelper)
    {
        _emailService = emailService;
        _emailsRepository = emailsRepository;
        _authorizationHelper = authorizationHelper;
        _domainConfiguration = domainConfiguration.Value;
    }

    [HttpPost("immediately")]
    public async Task<IActionResult> SendImmediately([FromBody] EmailMessage message)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (!_authorizationHelper.TryGetUserIdFromToken(token, out var userId)) return Unauthorized("Wrong userId");

        _emailService.Send(message, _domainConfiguration);
        _emailsRepository.AddEmailToSend(new Email()
        {
            SendState = SendStates.Send,
            Message = message,
            SendDate = DateTime.Now,
            UserId = userId.ToString()
        });
        return Ok();
    }

    [HttpPost("scheduled/on/{sendDate:datetime}")]
    public async Task<IActionResult> Schedule([FromBody] EmailMessage message, [FromRoute] DateTime sendDate)
    {
        if (sendDate < DateTime.Now) return await SendImmediately(message);

        var token = await HttpContext.GetTokenAsync("access_token");
        if (!_authorizationHelper.TryGetUserIdFromToken(token, out var userId)) return Unauthorized("Wrong userId");

        _emailsRepository.AddEmailToSend(new Email()
        {
            Message = message,
            SendDate = sendDate,
            UserId = userId.ToString()
        });
        return Ok();
    }
}