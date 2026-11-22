using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public RefreshTokenRepository(
            ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(UserRefreshToken refreshToken)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await connection.ExecuteAsync("[dbo].[USM_RefreshToken_Create]", new
            {
                refreshToken.UserId,
                refreshToken.TokenHash,
                refreshToken.ExpiresAtUtc
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<UserRefreshToken?> GetByHashAsync(
            string tokenHash)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            return await connection.QuerySingleOrDefaultAsync<UserRefreshToken>(
                "[dbo].[USM_RefreshToken_GetByHash]",
                new
                {
                    TokenHash = tokenHash
                }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int refreshTokenId)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            await connection.ExecuteAsync("[dbo].[USM_RefreshToken_Delete]",
                new
                {
                    RefreshTokenId = refreshTokenId
                }, commandType: CommandType.StoredProcedure);
        }
    }
}