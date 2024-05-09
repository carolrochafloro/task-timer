using Npgsql.Replication.PgOutput.Messages;
using System.Transactions;

namespace task_timer.Repositories;

public interface IUnitOfWork
{
    IAppTasksRepository TasksRepository { get; }
    ICategoriesRepository CategoriesRepository { get; } 
    Task CommitAsync();

}
