using BrainHope.DataAcess.Contexts;
using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
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
        private readonly BrainHopeDbContext dbcontext;

        private Hashtable _repsitories;
        public UnitOfWork(BrainHopeDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
            _repsitories = new Hashtable();


        }
        public Repository<T> Repository<T>() where T : ModelBase
        {
            var Key = typeof(T).Name;
            if (!_repsitories.ContainsKey(Key))
            {
                var repo = new Repository<T>(dbcontext);
                _repsitories.Add(Key, repo);
            }
            return _repsitories[Key] as Repository<T>;
        }


        public int Complete()
        {
            return dbcontext.SaveChanges();

        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }


    }
}
