namespace TemplatesReporter.Mail.Core;

public interface ITemplateBuilder
{
    Task<string> Build(string template, object model);
}