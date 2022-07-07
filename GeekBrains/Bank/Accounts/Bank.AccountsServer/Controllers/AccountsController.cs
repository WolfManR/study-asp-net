using App.Metrics;
using Bank.Accounts.Data;
using Bank.AccountsServer.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace Bank.AccountsServer.Controllers;

[ApiController]
public abstract class AccountsController : ControllerBase
{
    private readonly AccountsRepository _accountsRepository;
    private readonly IMetrics _metrics;

    protected AccountsController(AccountsRepository accountsRepository, IMetrics metrics)
    {
        _accountsRepository = accountsRepository;
        _metrics = metrics;
    }

    protected abstract string MetricsContext { get; }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _accountsRepository.Get();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var account = await _accountsRepository.Get(id);
        if (account is null) return NotFound();
        return Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string holder)
    {
        var id = await _accountsRepository.Create(holder);
        if (id <= 0) return BadRequest();

        _metrics.Measure.Counter.Increment(DatabaseMetricsRegistry.CreatedAccountsCounter(MetricsContext));

        return Ok(id);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _accountsRepository.Delete(id);
        return Ok();
    }
}