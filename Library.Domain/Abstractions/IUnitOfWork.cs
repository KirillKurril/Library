using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<Author> SingerRepository { get; }
        IRepository<Book> SongRepository { get; }
        public Task SaveAllAsync();     
    }
}
