namespace TemplatesReporter.Site.Data;

public class TemplatesRepository
{
    private static List<TemplateRecord> _templates;

    public TemplatesRepository() => _templates ??= new List<TemplateRecord>();

    public IEnumerable<TemplateRecord> Get() => _templates;
    public TemplateRecord? Get(string name) => _templates.FirstOrDefault(t => t.Name == name);
    public void Save(TemplateRecord templateRecord) => _templates.Add(templateRecord);

    public void Remove(string templateName)
    {
        if (_templates.FirstOrDefault(t => t.Name == templateName) is not { } template) return;

        _templates.Remove(template);
    }
}