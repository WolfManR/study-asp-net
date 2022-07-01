using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplatesReporter.AuthenticationRules;
using TemplatesReporter.Mail.Core;
using TemplatesReporter.MailsSender.Data;

namespace TemplatesReporter.MailsSender.Controllers;

[Route("mails")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MailsController : ControllerBase
{
    private readonly EmailsRepository _emailsRepository;
    private readonly AuthorizationHelper _authorizationHelper;

    public MailsController(EmailsRepository emailsRepository, AuthorizationHelper authorizationHelper)
    {
        _emailsRepository = emailsRepository;
        _authorizationHelper = authorizationHelper;
    }

    [HttpGet("foruser")]
    public async Task<IActionResult> GetUserEmails()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (!_authorizationHelper.TryGetUserIdFromToken(token, out var userId)) return Unauthorized("Wrong userId");

        var userMails = _emailsRepository.UserMails(userId.ToString());
        var data = userMails.Select(email => new MailInfo()
        {
            Message = email.Message,
            SendDate = email.SendDate,
            SendState = email.SendState.Name,
            SendStateDescription = email.SendState.Description
        }).ToList();

        return Ok(data);
    }
}

public class MailInfo
{
    public DateTime SendDate { get; init; }
    public EmailMessage Message { get; init; }
    public string SendState { get; init; }
    public string SendStateDescription { get; init; }
}