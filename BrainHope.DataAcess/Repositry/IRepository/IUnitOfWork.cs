using BrainHope.DataAcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Repositry.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        Repository<T> Repository<T>() where T : ModelBase;

        Task <int>  Complete();

        IChatRepository ChatRepository { get; }
        IPostRepository PostRepository { get; }
    }
}
