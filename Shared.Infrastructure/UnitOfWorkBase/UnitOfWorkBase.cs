using  Microsoft.Data.SqlClient;
using  Microsoft.EntityFrameworkCore;
using  Shared.Application.Multitenant;
using  Shared.Application.UnitOfWorkBase;
using  Shared.Domain.Contracts.EntityCommonData;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;
using Shared.Application.UnitOfWorkBase;

namespace Shared.Infrastructure.UnitOfWorkBase
{
    public class UnitOfWorkBase : IUnitOfWorkBase
    {
        protected readonly DbContext _context;
        protected readonly IExceptionManager _exceptionManager;
        protected readonly ITenantResolver _tenantResolver;

        public UnitOfWorkBase(DbContext context,
                                ITenantResolver tenantResolver,
                                IExceptionManager exceptionManager)
        {
            _context = context;
            _exceptionManager = exceptionManager;
            _tenantResolver = tenantResolver;
        }
        public ReturnBase<int> Save()
        {
            try
            {
                var saveResult = _context.SaveChanges();
                return ReturnBase<int>.Success(saveResult);
            }
            catch (Exception ex)
            {
                return ReturnBase<int>.Fail(ex, _exceptionManager);
            }
        }

        public async Task<ReturnBase<int>> SaveAsync()
        {
            try
            {
                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    if (entry.Entity is IAuditable entity)
                    {
                        Console.WriteLine($"In_Date: {entity.In_Date}");
                    }
                }
                var saveResult = await _context.SaveChangesAsync();
                return ReturnBase<int>.Success(saveResult);
            }
            catch (Exception ex)
            {
                var errors = new List<ReturnBaseError>();
                SqlException? sqlEx = ex switch
                {
                    SqlException sql => sql,
                    DbUpdateException dbEx => dbEx.InnerException as SqlException,
                    _ => ex.InnerException as SqlException ?? ex.GetBaseException() as SqlException
                };

                if (sqlEx != null)
                {
                    SqlError? primaryError = null;
                    foreach (SqlError error in sqlEx.Errors)
                    {
                        if (error.Number >= 50000)
                        {
                            primaryError = error;
                            break;
                        }
                        else if (primaryError == null && error.Number != 3621)
                        {
                            primaryError = error;
                        }
                    }

                    if (primaryError != null)
                    {
                        string? errorDesc = await GetErrorDescriptionFromMsgError(primaryError.Number);

                        string systemMessage = primaryError.Message;
                        var customMessage = errorDesc != null
                            ? new ReturnBaseError { ErrorMessage = errorDesc }
                            : _exceptionManager.GetErrorFromException(ex.InnerException);

                        errors.Add(new ReturnBaseError
                        {
                            SystemErrorMessage = systemMessage,
                            ErrorMessage = customMessage.ErrorMessage,
                            ErrorCode = primaryError.Number.ToString(),
                            Source = sqlEx.Source
                        });
                    }

                    else if (sqlEx.Errors.Count > 0)
                    {
                        var firstError = sqlEx.Errors[0];
                        string? errorDesc = await GetErrorDescriptionFromMsgError(firstError.Number);

                        string systemMessage = firstError.Message;
                        var customMessage = errorDesc != null
                            ? new ReturnBaseError { ErrorMessage = errorDesc }
                            : _exceptionManager.GetErrorFromException(ex.InnerException);

                        errors.Add(new ReturnBaseError
                        {
                            SystemErrorMessage = systemMessage,
                            ErrorMessage = customMessage.ErrorMessage,
                            ErrorCode = firstError.Number.ToString(),
                            Source = sqlEx.Source
                        });
                    }
                }
                else
                {
                    var customMessage = _exceptionManager.GetErrorFromException(ex.InnerException);
                    errors.Add(new ReturnBaseError
                    {
                        SystemErrorMessage = ex.Message,
                        ErrorMessage = customMessage.ErrorMessage
                    });
                }

                return ReturnBase<int>.Fail(errors);
            }

        }

        private async Task<string?> GetErrorDescriptionFromMsgError(int errorNumber)
        {
            string connectionString = _context.Database.GetDbConnection().ConnectionString;
            string query = "SELECT MsgError_Name FROM syst.MsgError WHERE MsgError_No = @ErrorNumber";

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ErrorNumber", errorNumber);

                    try
                    {
                        await connection.OpenAsync();
                        var result = await command.ExecuteScalarAsync();
                        return result as string;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error querying syst.MsgError: {ex.Message}");
                        return null;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
        }
    }
}