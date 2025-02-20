using BrainHope.DataAcess.Contexts;
using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Repositry
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BrainHopeDbContext _dbcontext;
        public IChatRepository ChatRepository { get; private set; }

        private Hashtable _repsitories;
        public UnitOfWork(BrainHopeDbContext dbcontext)
        {
            this._dbcontext = dbcontext;
            _repsitories = new Hashtable();
            ChatRepository = new ChatRepository(_dbcontext);


        }
        public Repository<T> Repository<T>() where T : ModelBase
        {
            var Key = typeof(T).Name;
            if (!_repsitories.ContainsKey(Key))
            {
                var repo = new Repository<T>(_dbcontext);
                _repsitories.Add(Key, repo);
            }
            return _repsitories[Key] as Repository<T>;
        }



        public async Task<int> Complete()
        {
            return await _dbcontext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbcontext.Dispose();
        }

       
    }
}
