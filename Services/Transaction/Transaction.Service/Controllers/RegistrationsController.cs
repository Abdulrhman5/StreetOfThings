using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.Service.Models;
using Transaction.Service.Events;
using Transaction.Service.Infrastructure;
using Transaction.Service.Dtos;
using TransactionType = Transaction.Service.Dtos.TransactionType;
using MediatR;
using Transaction.Core.Commands;
using Transaction.Core.Queries;
using Transaction.Core;

namespace Transaction.Service.Controllers
{
    [Route("api/[controller]")]
    public class RegistrationsController : MyControllerBase
    {

        private IMediator _mediator;

        public RegistrationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("create")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateRegistrationCommand newRegistrationDto)
        {
            var result = await _mediator.Send(newRegistrationDto);

            return StatusCode<CreateRegistrationResultDto>(result);
        }

        [Route("refresh")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRegistrationTokenCommand tokenRefreshCommand)
        {
            var result = await _mediator.Send(tokenRefreshCommand);

            return StatusCode(result);
        }

        [Route("cancel")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelRegistration([FromBody] CancelRegistrationCommand cancelRegistrationCommand)
        {
            var result = await _mediator.Send(cancelRegistrationCommand);
            return StatusCode(result, new
            {
                Message = "A registration has been canceled"
            });
        }

        [Route("MeAsRecipient")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsersRegistrationsOnOtherUsersObjects([FromQuery] PagingArguments pagingArguments)
        {
            var registrationsQuery = new RegisterationForUserQuery
            {
                RegistrationType = RegistrationForUserType.UserAsRecipient,
                CurrentPage = pagingArguments.CurrentPage,
                Size = pagingArguments.Size,
                StartObject = pagingArguments.StartObject,
                Total = pagingArguments.Total
            };

            var result = await _mediator.Send(registrationsQuery);
            return Ok(result);
        }

        [Route("MeAsOwner")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserObjectsTransactions([FromQuery] PagingArguments pagingArguments)
        {
            var registrationsQuery = new RegisterationForUserQuery
            {
                RegistrationType = RegistrationForUserType.UserAsOwner,
                CurrentPage = pagingArguments.CurrentPage,
                Size = pagingArguments.Size,
                StartObject = pagingArguments.StartObject,
                Total = pagingArguments.Total
            };

            var result = await _mediator.Send(registrationsQuery);
            return Ok(result);
        }
    }
}
