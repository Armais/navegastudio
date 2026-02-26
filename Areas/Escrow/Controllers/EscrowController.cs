using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NavegaStudio.Areas.Escrow.Helpers;
using NavegaStudio.Areas.Escrow.Models;
using NavegaStudio.Areas.Escrow.Services;

namespace NavegaStudio.Areas.Escrow.Controllers;

[Area("Escrow")]
public class EscrowController : Controller
{
    private readonly IEscrowService _escrowService;
    private readonly EthereumSettings _ethSettings;

    public EscrowController(IEscrowService escrowService, IOptions<EthereumSettings> ethSettings)
    {
        _escrowService = escrowService;
        _ethSettings = ethSettings.Value;
    }

    // ── API Endpoints ────────────────────────────────────────────

    [HttpGet("api/escrow/{id:int}")]
    public async Task<IActionResult> ApiGetEscrow(int id)
    {
        var escrow = await _escrowService.GetEscrowAsync(id);
        if (escrow == null) return NotFound(new { error = "Escrow not found" });
        return Ok(MapToDto(escrow));
    }

    [HttpGet("api/escrow/user/{address}")]
    public async Task<IActionResult> ApiGetUserEscrows(string address)
    {
        var escrows = await _escrowService.GetUserEscrowsAsync(address);
        return Ok(escrows.Select(MapToDto));
    }

    [HttpGet("api/escrow/all")]
    public async Task<IActionResult> ApiGetAllEscrows()
    {
        var escrows = await _escrowService.GetAllEscrowsAsync();
        return Ok(escrows.Select(MapToDto));
    }

    [HttpPost("api/escrow/create")]
    public async Task<IActionResult> ApiCreateEscrow([FromBody] CreateEscrowRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request", details = ModelState });

        try
        {
            var escrow = await _escrowService.CreateEscrowAsync(request);
            return Ok(MapToDto(escrow));
        }
        catch (Exception ex)
        {
            return HandleServiceException(ex);
        }
    }

    [HttpPost("api/escrow/{id:int}/ship")]
    public async Task<IActionResult> ApiShip(int id, [FromBody] ActionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request", details = ModelState });

        try
        {
            var escrow = await _escrowService.ConfirmShipmentAsync(id, request.Address);
            return Ok(MapToDto(escrow));
        }
        catch (Exception ex)
        {
            return HandleServiceException(ex);
        }
    }

    [HttpPost("api/escrow/{id:int}/confirm")]
    public async Task<IActionResult> ApiConfirm(int id, [FromBody] ActionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request", details = ModelState });

        try
        {
            var escrow = await _escrowService.ConfirmReceiptAsync(id, request.Address);
            return Ok(MapToDto(escrow));
        }
        catch (Exception ex)
        {
            return HandleServiceException(ex);
        }
    }

    [HttpPost("api/escrow/{id:int}/dispute")]
    public async Task<IActionResult> ApiDispute(int id, [FromBody] DisputeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request", details = ModelState });

        try
        {
            var escrow = await _escrowService.RaiseDisputeAsync(id, request.Address, request.Reason);
            return Ok(MapToDto(escrow));
        }
        catch (Exception ex)
        {
            return HandleServiceException(ex);
        }
    }

    [HttpPost("api/escrow/{id:int}/resolve")]
    public async Task<IActionResult> ApiResolve(int id, [FromBody] ResolveRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request", details = ModelState });

        try
        {
            var escrow = await _escrowService.ResolveDisputeAsync(id, request.Address, request.BuyerPercent);
            return Ok(MapToDto(escrow));
        }
        catch (Exception ex)
        {
            return HandleServiceException(ex);
        }
    }

    [HttpPost("api/escrow/{id:int}/cancel")]
    public async Task<IActionResult> ApiCancel(int id, [FromBody] ActionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Invalid request", details = ModelState });

        try
        {
            var escrow = await _escrowService.CancelEscrowAsync(id, request.Address);
            return Ok(MapToDto(escrow));
        }
        catch (Exception ex)
        {
            return HandleServiceException(ex);
        }
    }

    [HttpGet("api/escrow/contract-info")]
    public IActionResult ApiContractInfo()
    {
        return Ok(new
        {
            contractAddress = _ethSettings.ContractAddress,
            chainId = _ethSettings.ChainId,
            chainName = _ethSettings.ChainName,
            rpcUrl = _ethSettings.RpcUrl,
            isDemoMode = _escrowService.IsDemoMode
        });
    }

    // ── Exception handling ────────────────────────────────────────

    private IActionResult HandleServiceException(Exception ex) => ex switch
    {
        KeyNotFoundException => NotFound(new { error = ex.Message }),
        UnauthorizedAccessException => StatusCode(403, new { error = ex.Message }),
        InvalidOperationException => Conflict(new { error = ex.Message }),
        ArgumentException => BadRequest(new { error = ex.Message }),
        _ => StatusCode(500, new { error = "An internal error occurred" })
    };

    // ── DTO mapping ───────────────────────────────────────────────

    private static object MapToDto(EscrowTransaction e) => new
    {
        e.Id,
        e.OnChainId,
        e.Buyer,
        e.Seller,
        e.Arbiter,
        Amount = EthFormat.FormatEth(e.Amount),
        e.ArbiterFeeBps,
        e.Description,
        State = e.State.ToString(),
        e.DisputeReason,
        e.TxHash,
        CreatedAt = e.CreatedAt.ToString("o"),
        UpdatedAt = e.UpdatedAt.ToString("o"),
        Events = e.Events.OrderBy(ev => ev.Timestamp).Select(ev => new
        {
            ev.Action,
            ev.PerformedBy,
            ev.Details,
            ev.TxHash,
            Timestamp = ev.Timestamp.ToString("o")
        })
    };
}
