using BankingAPI.Interfaces;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingController : ControllerBase
    {
        private readonly ITransact _transact;

        public BankingController(ITransact transact)
        {
            _transact = transact;
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<ActionResult<TransactionResponse>> Deposit(DepositRequest request)
        {
            try
            {
                var result = await _transact.Deposit(request);
                return CreatedAtAction(nameof(GetTransactionByReference), new { referenceNumber = result.TransactionReferenceNumber }, result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("withdraw")]
        public async Task<ActionResult<TransactionResponse>> Withdraw(WithdrawRequest request)
        {
            try
            {
                var result = await _transact.Withdraw(request);
                return CreatedAtAction(nameof(GetTransactionByReference), new { referenceNumber = result.TransactionReferenceNumber }, result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<ActionResult<TransactionResponse>> Transfer(TransferRequest request)
        {
            try
            {
                var result = await _transact.Transfer(request);
                return CreatedAtAction(nameof(GetTransactionByReference), new { referenceNumber = result.TransactionReferenceNumber }, result);
            }
            catch (ArgumentException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // Flexible query endpoint (accepts optional filters, paging and sorting)
        [Authorize]
        [HttpPost("transactions/query")]
        public async Task<ActionResult<TransactionQueryResponse>> QueryTransactions(TransactionQueryRequest request)
        {
            try
            {
                var result = await _transact.QueryTransactions(request);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Authorize]
        [HttpGet("transactions/{referenceNumber:int}")]
        public async Task<ActionResult<TransactionResponse>> GetTransactionByReference(int referenceNumber)
        {
            try
            {
                var result = await _transact.GetTransactionByReference(referenceNumber);
                if (result == null) return NotFound($"Transaction {referenceNumber} not found");
                return Ok(result);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
