using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eInvoice.domain.Common
{
    /// <summary>
    /// Matching option field from legacy ICTSIBilling
    /// </summary>
    /// <remarks>
    /// Izrada računa...
    /// Račun se radi u dva koraka
    ///  - Definira se zaglavlje računa.Preko zaglavlja računa dobiju se ulazni parametri koji definiraju podatke za izdvajanji cijena za svaku tarifu
    ///  - Nakon definicije zaglavlja računa izvrši se kalkulacija cijene za rekapitulaciju iz specifikacije koja se nalazi u kontroli 'zaObracun'
    /// opcija - kvalifikator koji definira kakav je tip računa
    ///  1 - Brodske manipulacije
    ///  2 - Ostale nanipulacije
    ///  3 - Skladišnina
    ///  4 - Izvan svih kriterija - Ne ulazi u opcije 1,2,3 i 5
    ///  5 - Pool
    ///  6 - Skladište
    ///  7 - 
    ///  8 - Punjenje / pražnjenje - Ostale usluge
    ///  9 - Električna energija
    /// </remarks>
    public enum BillingMethodQualifier: short
    {
       Vessel = 1, // Brodske manipulacije
       Other = 2,  // Ostale manipulacije
       Storage = 3, // Skladistenje
       OutOfCriteria = 4, // Izvan svih kriterija - Ne ulazi u opcije 1,2,3 i 5
       Pool = 5, // Pool
       Warehouse = 6, // Skladište
       TheOtherInvoice= 7, // Bound to Ostali računu
       Services = 8, //  Punjenje / pražnjenje - Ostale usluge   
       Electricity = 9, //Električna energija
       SplitVesselBills = 11 // Bound to Brodske operacije -> Podjela troškova
    }
}
