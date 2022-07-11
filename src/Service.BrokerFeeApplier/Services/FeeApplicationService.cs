using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.BrokerFeeApplier.Domain.Models.FireblocksWithdrawals;
using Service.BrokerFeeApplier.Grpc;
using Service.BrokerFeeApplier.Grpc.Models;
using Service.BrokerFeeApplier.Postgres;
using Service.BrokerFeeApplier.Settings;

namespace Service.BrokerFeeApplier.Services
{
    public class FeeApplicationService: IFeeApplicationService
    {
        private readonly ILogger<FeeApplicationService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public FeeApplicationService(ILogger<FeeApplicationService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<GetFeeApplicationsResponse> GetFeeApplications(GetFeeApplicationsRequest request)
        {
            _logger.LogInformation("Receive GetFeeApplicationsRequest: {JsonRequest}", request.ToJson());

            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var query = context.FeeApplication.AsQueryable();

                if (request.LastId > 0)
                {
                    query = query.Where(e => e.Id < request.LastId);
                }

                var applications = await query
                    .OrderByDescending(e => e.Id)
                    .Take(request.BatchSize)
                    .ToListAsync();

                var response = new GetFeeApplicationsResponse
                {
                    Success = true,
                    Collection = applications.Select(e => new FireblocksFeeApplication()
                    {
                       Type = e.Type,
                       Amount = e.Amount,
                       AssetSymbol = e.AssetSymbol,
                       Comment = e.Comment,
                       DestinationAddress = e.DestinationAddress,
                       DestinationTag = e.DestinationTag,
                       EventDate= e.EventDate,
                       ExternalId = e.ExternalId,
                       FeeAmount = e.FeeAmount,
                       FeeApplicationIdInMe = e.FeeApplicationIdInMe,
                       FeeAssetSymbol=e.FeeAssetSymbol,
                       InternalNote = e.InternalNote,
                       Network=e.Network,
                       Status=e.Status,
                       TransactionId=e.TransactionId,
                    }).ToList(),
                    IdForNextQuery = applications.Count > 0 ? applications.Min(d => d.Id) : 0
                };

                response.Collection.Count.AddToActivityAsTag("response-count-items");
                _logger.LogInformation("Return GetFeeApplication response count items: {count}",
                    response.Collection.Count);
                return response;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Cannot get GetFeeApplications take: {takeValue}, LastId: {LastId}",
                    request.BatchSize, request.LastId);
                return new GetFeeApplicationsResponse { Success = false, ErrorMessage = exception.Message };
            }
        }
    }
}
