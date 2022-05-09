using System;
using System.Threading.Tasks;

namespace SnakeWPF.Persistence
{
    public interface ISnakeDataAccess
    {
        public Task<SnakeTable> LoadAsync(String path);

        public Task SaveAsync(String path, SnakeTable table);
    }
}
