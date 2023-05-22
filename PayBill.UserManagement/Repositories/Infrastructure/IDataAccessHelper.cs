namespace PayBill.UserManagement.Repositories.Infrastructure;

public interface IDataAccessHelper
{
    Task<int> ExecuteData<T>(string storedProcedure, T parameters);
    Task<List<T>> QueryData<T, TU>(string storedProcedure, TU parameters);
}