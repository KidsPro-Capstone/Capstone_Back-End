﻿using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Generic;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class GameBoardMapRepository : BaseRepository<GameBoardMapPosition>, IGameBoardMapRepository
{
    public GameBoardMapRepository(AppDbContext context, ILogger<BaseRepository<GameBoardMapPosition>> logger) : base(context, logger)
    {
    }
}
