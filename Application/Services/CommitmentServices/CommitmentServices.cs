﻿using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IRepository;
using Domain.Entities.Commitment;
using Domain.Entities.Room;
using Domain.Enums;

namespace Application.Services.CommitmentServices;


public class CommitmentServices : ICommitmentServices
{
    public readonly IGenericRepository<CommitmentEntity> _commitmentRepository;


    public CommitmentServices(
        IGenericRepository<CommitmentEntity> commitmentRepository
        )
    {
        _commitmentRepository = commitmentRepository;
    }

    public async Task CreateCommitment(CommitmentEntity commitment, RoomEntity room)
    {
        commitment.RoomId = room.Id;
        commitment.CommitmentStatus = CommitmentStatus.Pending;
        // save commitment
        await _commitmentRepository.CreateAsync(commitment);
    }

    public async Task<CommitmentEntity> GetNotExpiredCommitmentByRoom(Guid roomId)
    {
        CommitmentEntity com = await _commitmentRepository
            .FirstOrDefaultAsync(com =>
            com.RoomId.Equals(roomId)
            && !com.Status.Equals(CommitmentStatus.Expired.ToString())
            );
        return com ??
           throw new NotFoundException("Commitment Not Found Or Already Expired");
    }

    public async Task<IList<CommitmentEntity>> GetCommitmentForTenant(Guid roomId, Guid tenantId)
    {
        var coms = await _commitmentRepository.WhereAsync(com =>
            com.RoomId.Equals(roomId) && com.TenantId.Equals(tenantId));
        return coms;
    }

    public async Task ApprovedCommitment(CommitmentEntity commitment)
    {
        commitment.CommitmentStatus = CommitmentStatus.Approved;
        await _commitmentRepository.UpdateAsync(commitment);
    }
    public async Task ActivatedCommitment(CommitmentEntity commitment, Guid tenantId)
    {
        // There is no main person in the contract
        if (commitment.Tenant == null)
        {
            commitment.TenantId = tenantId;
            commitment.CommitmentStatus = CommitmentStatus.Active;
            await _commitmentRepository.UpdateAsync(commitment);
        }
    }

    public async Task<CommitmentEntity> GetCommitment(Guid commitmentId)
    {
        CommitmentEntity com = await _commitmentRepository.FindByIdAsync(commitmentId);
        return com ??
          throw new NotFoundException("Commitment Not Found Or Already Expired");

    }

    public async Task<CommitmentEntity> GetNotExpiredCommitment(Guid Id)
    {
        CommitmentEntity com = await _commitmentRepository
            .FirstOrDefaultAsync(com =>
            com.Id.Equals(Id)
            && !com.Status.Equals(CommitmentStatus.Expired.ToString())
            );
        return com ??
           throw new NotFoundException("Commitment Not Found Or Already Expired");
    }

    public async Task<CommitmentEntity> GetApprovedOrActiveCommitment(Guid Id)
    {
        CommitmentEntity com = await _commitmentRepository
            .FirstOrDefaultAsync(com =>
            com.Id.Equals(Id)
            && (com.Status.Equals(CommitmentStatus.Active.ToString()) ||
                com.Status.Equals(CommitmentStatus.Approved.ToString()))
            );
        return com ??
           throw new NotFoundException("Commitment Not Found Or Already Expired");
    }

    public async Task UpdatePendingCommitment(CommitmentEntity updatedCommitment)
    {
        await _commitmentRepository.UpdateAsync(updatedCommitment);
    }

    public async Task<CommitmentEntity> GetCommitment(Guid Id, CommitmentStatus status)
    {
        CommitmentEntity com = await _commitmentRepository
            .FirstOrDefaultAsync(com =>
            com.Id.Equals(Id)
            && com.Status.Equals(status.ToString())
            );
        return com ??
            throw new NotFoundException("Commitment Not Found");
    }

    public async Task<int> CountForHostel(Guid hostelId)
    {
        var list = await _commitmentRepository.WhereAsync(com =>
        com.HostelId.Equals(hostelId));
        return list.Count;
    }
}
