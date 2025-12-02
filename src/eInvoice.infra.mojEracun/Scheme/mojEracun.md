# MojERačun documentation  

[MojEračun manual](https://manual.moj-eracun.hr/documentation/getting-started/)  
[Validation](https://validate-demo.moj-eracun.hr/Validation/ValidateUBL)  

## Tools
dotnet tool install --global dotnet-xscgen  

Generate POCOs: 
```bash
xscgen .\maindoc\UBL-Invoice-2.1.xsd
```  

## Migrations 

From api project run:  
```bash
dotnet ef migrations add InitCommodityClassification -p ..\eInvoice.infra.efc\eInvoice.infra.efc.csproj

dotnet ef database update -p ..\eInvoice.infra.efc\eInvoice.infra.efc.csproj
```  

Tips, in package manager console as default project select eInvoice.infra.efc

## Test configuration  
In the test project add link to appsettings.development.json of the api project. Set copy to output directory.  
Add existing item -> Add as link

## Application monitoring  

There are 2 endpoints available for monitoring:  
- `/health` - liveness probe
- `/ping` - 3rd party readiness probe (mojEračun availability))  

### health  

The `/health` endpoint checks the overall app's status. It uses the `eInvoice.Jobs.JobsExtensions.HealthCheck` method. 
It returns: 
- Status200OK: healthy if the app is running and all checks pass.
- Status503ServiceUnavailable: unhealthy indicates that the health check determined that at leas one service was unhealthy.  

Steps determining status of the app: 
- execute a simple database query `select 1`,
- Calls each job’s custom *HealthCheck* method.  

For the details of the unhealthy services are contained in the log. 

### ping readiness  
The `/ping` endpoint checks the availability of the MojERačun 3rd party service. 
It uses the `eInvoice.infra.mojEracun.InvoiceSender.PingAsync` method.  
If service is available it returns
``` json  
{
    "Status": "ok",
    "Message": "Service is up"
}
```  

``` mermaid
flowchart TD
	A[InvoiceController -> ping request] --> B[Mediator]
	B -- PingEinvoiceAdapterHandler --> C[Typed client InvoiceSender]  
	C --> D[{EinvoiceServiceUri}/ping]
```

This probe is not included in the overall health check of the application to avoid unnecessary external calls. 

## Send invoice  

In legacy billing invoices are marked for fiscalization by setting InvoiceDetails.SendingStatus = 1: NotSent.  
Adapter cyclically tries to load such invoices. Cycle is set in adapter json configuration. 
All invoices where InvoiceDetails.SendingStatus = 1 and InvoiceAdded.SendingStatus != 2.  

``` json
 "RefreshInterval": 5
```  

## Send Invoice  

- manual request  [HttpPost("SendInvoice")]
- FetchInvoiceService
- mediator calls handler <ReadyInvoiceDto, bool>

## Paid invoice  

``` SQL
	[IsPaid] [bit] NOT NULL,
	[PaidConfirmationMessage] [nvarchar](max) NULL,
	[PaidFiscalizationDateTime] [datetimeoffset](7) NULL,
```

## Rejected invoice  

Adapter cyclically tries to load rejected invoices where IsRejected is set to true and RejectedFiscalizationDateTime is null. 
Cycle is set in adapter json configuration.  
``` json
 "RefreshInterval": 5
```

Reject reason type at the time of writting this documentation are:
”N” Data discrepancy that does not affect tax calculation
”U” Data discrepancy that affects tax calculation
”O” Other

Agreed with financial department, reject type is "O" with description *"Greška u dostavljenom računu"*.

According to financial department, invoice can be paid and then rejected. For this reason, dedicated columns are present in persistant repository.  
``` SQL
	[IsRejected] [bit] NOT NULL,
	[RejectedConfirmationMessage] [nvarchar](max) NULL,
	[RejectedFiscalizationDateTime] [datetimeoffset](7) NULL,
```

## Purging  

Configured in appsettings.json. Read by job service `PurgeInvoiceService` which fires command to execute `PurgingInvoiceHandler`.  
InvoiceDetails and Logs are purged after configured number of months.

``` json 
  "JobsSettings": {
    "RefreshInterval": 5,
    "PurgeAfterMonths":  6
  }
```

---
#Helpers  

[Fiskalizacija 2.0 porezna](https://porezna.gov.hr/fiskalizacija/bezgotovinski-racuni/bezgotovinski-racuni-novosti/o/fiskalizacija-informacijski-posrednici)  
[KPD](https://web.dzs.hr/App/klasus/default.aspx?lang=hr)  
[porezna](https://porezna.gov.hr/fiskalizacija/bezgotovinski-racuni/fiskalizacija-bezgotovinskih-racuna)  
[porezna doc](https://porezna.gov.hr/fiskalizacija/api/dokumenti/99)  
[webApplicationFactory](https://github.com/papugamichal/Course_IntegrationTestingNetCore_by_SteveGordon/tree/master/test/TennisBookings.Merchandise.Api.IntegrationTests)   
[poslovni proces](https://www.izracunko.com/blog/racuni/kako-odabrati-oznaku-tip-poslovnog-procesa-pri-slanju-eracuna/)  
[webinar](https://www.youtube.com/watch?v=89uvg8bgfWI)  
[webinar HR extension](https://youtu.be/9qiN-VJ9ZBQ)
[eIzvještavanje](https://portal.moj-eracun.hr/blog/eizvjestavanje-u-postupku-fiskalizacije/)

- Demo mojEračun portal  
[portal](https://www-demo.moj-eracun.hr/b2b/sentdocuments)
- UnitCode  
[Preporuke br. 20 UN/ECE-a „Oznake za jedinice mjere koje se koriste u međunarodnoj trgovini”](https://unece.org/trade/uncefact/cl-recommendations)