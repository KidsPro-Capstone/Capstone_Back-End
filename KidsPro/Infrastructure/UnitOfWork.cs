﻿using Application;
using Application.Interfaces.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly ILogger<UnitOfWork> _logger;

    public IRoleRepository RoleRepository { get; }
    public IAccountRepository AccountRepository { get; }
    public IParentRepository ParentRepository { get; }
    public IStudentRepository StudentRepository { get; }
    public IStaffRepository StaffRepository { get; }
    public ITeacherRepository TeacherRepository { get; }

    public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger, IRoleRepository roleRepository,
        IAccountRepository accountRepository, IParentRepository parentRepository, IStudentRepository studentRepository, IStaffRepository staffRepository, ITeacherRepository teacherRepository)
    {
        _context = context;
        _logger = logger;
        RoleRepository = roleRepository;
        AccountRepository = accountRepository;
        ParentRepository = parentRepository;
        StudentRepository = studentRepository;
        StaffRepository = staffRepository;
        TeacherRepository = teacherRepository;
    }

    public async Task<int> SaveChangeAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_transaction == null)
                throw new Exception("Transaction is not initiate");
            await _transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when commit transaction.\nDate:{}", DateTime.UtcNow);
            throw new Exception("Transaction has not been created yet.");
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            if (_transaction == null)
                throw new Exception("Transaction is not initiate");
            await _transaction.RollbackAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when commit transaction.\nDate:{}", DateTime.UtcNow);
            throw new Exception("Transaction has not been created yet.");
        }
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        this._disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}