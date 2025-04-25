namespace SOSInternet.Core.Interfaces
{
    /// <summary>
    /// Interfaccia per il riavvio del router 
    /// </summary>
    public interface IRouterRebooter
    {
        /// <summary>
        /// Riavvia il router
        /// </summary>
        /// <returns>Task che rappresenta l'operazione asincrona di riavvio</returns>
        Task RebootAsync();
    }
}
