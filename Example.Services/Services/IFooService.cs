using System.Threading.Tasks;
using System.Collections.Generic;

namespace Example.Services
{
    public interface IFooService
    {
        /// <summary>
        /// Retrieves the <see cref="Foo"/> with the given ID.
        /// </summary>
        Task<Foo> GetById(int id);

        /// <summary>
        /// Retrieves all <see cref="Foo"/>s that exist in the service.
        /// </summary>
        Task<IList<Foo>> GetAll();

        /// <summary>
        /// Saves the given <see cref="Foo"/>.
        /// </summary>
        Task Save(Foo foo);

        /// <summary>
        /// Deletes the <see cref="Foo"/> with the given ID.
        /// </summary>
        Task Delete(int id);
    }
}
