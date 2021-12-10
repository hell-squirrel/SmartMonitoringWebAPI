using System.Threading.Tasks;

namespace SmartMonitoring.Infrastructure.Interfaces
{
    public interface IDatabaseInitializer
    {
        /// <summary>
        /// Setup base database configuration
        /// </summary>
        /// <returns>Successful task result</returns>
        Task Setup();
    }
}
