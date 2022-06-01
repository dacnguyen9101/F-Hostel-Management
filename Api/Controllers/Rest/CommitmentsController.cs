﻿using Api.UserFeatures.Requests;
using Application.Interfaces;
using Application.Interfaces.IRepository;
using Domain.Entities;
using Domain.Entities.Commitment;
using Domain.Entities.Room;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Rest;

public class CommitmentsController : BaseRestController
{
    private readonly IGenericRepository<HostelEntity> _hostelRepository;
    private readonly ICommitmentServices _commitmentServices;
    private readonly IRoomServices _roomServices;
    private readonly ITenantServices _tenantServices;
    private readonly IJoiningCodeServices _joiningCodeServices;


    public CommitmentsController(
        IGenericRepository<HostelEntity> hostelRepository,
        ICommitmentServices commitmentServices,
        IJoiningCodeServices joiningCodeServices,
        IRoomServices roomServices,
        ITenantServices tenantServices)
    {
        _tenantServices = tenantServices;
        _hostelRepository = hostelRepository;
        _joiningCodeServices = joiningCodeServices;
        _commitmentServices = commitmentServices;
        _roomServices = roomServices;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCommitment(CreateCommitmentRequest comReq)
    {

        // check room status
        RoomEntity room = await _roomServices.GetAvailableRoomByIdAsync(comReq.RoomId);
        if (room == null)
        {
            return BadRequest("Room does not exist or already rented");
        }

        // not check hostel owner and owner from request
        HostelEntity hostel = await _hostelRepository.FirstOrDefaultAsync(hostel => hostel.Id.Equals(room.HostelId));

        if (!comReq.OwnerId.Equals(hostel.OwnerId))
        {
            return Unauthorized();
        }

        bool isDuplicated = await _commitmentServices.IsExist(comReq.CommitmentCode);
        if (isDuplicated)
        {
            return BadRequest("Commitment code duplicate");
        }

        // call service
        CommitmentEntity com = Mapper.Map<CommitmentEntity>(comReq);
        await _commitmentServices.CreateCommitment(com, room);

        // update room status
        await _roomServices.Rent(room);

        return Ok(com);
    }

    // owner conform commitment ==> com.status => approved
    [HttpPatch("owner/status")]
    public async Task<IActionResult> OwnerApprovedCommitment
        ([FromBody] OwnerApprovedCommitmentRequest req)
    {
        CommitmentEntity com =
            await _commitmentServices.GetPendingCommitmentById(req.CommitmentId);
        if (com == null)
        {
            return BadRequest();
        }

        await _commitmentServices.ApprovedCommitment(com);
        return Ok(com);
    }



    // tenant into commitment ==> com.status => done
    //[Authorize(nameof(Role.Tenant))]
    [HttpPatch("tenant/status")]
    public async Task<IActionResult> TenantDoneCommitment
    ([FromBody] TenantDoneCommitmentRequest req)
    {
        JoiningCode joiningCode = await _joiningCodeServices.
            GetJoiningCodeByDigits(req.SixDigitsJoiningCode);

        if (joiningCode == null)
        {
            return BadRequest("Joining code is not exists or expired");
        }

        bool isValid = _joiningCodeServices.ValidateJoiningCode(joiningCode);

        if (!isValid)
        {
            return BadRequest("Joining code is not exists or expired");
        }

        CommitmentEntity com =
            await _joiningCodeServices.GetCommitmentByJoiningCode(joiningCode);

        if (com == null)
        {
            return BadRequest();
        }

        await _commitmentServices.ActivatedCommitment(com, req.TenantId);
        bool isSuccess =  await _tenantServices.GetIntoRoom(com.RoomId, req.TenantId);
        if (!isSuccess)
        {
            return BadRequest("Get in to room fail");
        }
        return Ok(com);
    }

    // commitment expired ==> com.status => expired => remove all invoice schedules

    // create joining code
    [HttpPost("joiningCode")]
    public async Task<IActionResult> CreateJoiningCode
        ([FromBody] CreateJoiningCodeRequest req)
    {
        CommitmentEntity currentCom = await _commitmentServices.GetNotExpiredCommitmentById(req.CommitementId);
        if (currentCom == null)
        {
            return BadRequest("Commitment does not exist");
        }

        JoiningCode joiningCode = Mapper.Map<JoiningCode>(req);
        var response = await _joiningCodeServices.CreateJoiningCode(joiningCode);
        return Ok(response);
    }

    // get commitment by joining code
    [HttpGet("joiningCode/{SixDigitsCode}")]
    public async Task<IActionResult> GetCommitmentUsingJoiningCode([FromRoute] int SixDigitsCode)
    {
        JoiningCode joiningCode = await _joiningCodeServices.GetJoiningCodeByDigits(SixDigitsCode);
        if (joiningCode == null)
        {
            return BadRequest("Joining code is not exists or expired");
        }

        bool isValid = _joiningCodeServices.ValidateJoiningCode(joiningCode);

        if (!isValid)
        {
            return BadRequest("Joining code is not exists or expired");
        }

        CommitmentEntity commitment = await _joiningCodeServices.GetCommitmentByJoiningCode(joiningCode);

        return Ok(commitment);
    }
}
