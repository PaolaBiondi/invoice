# Jobs  
This project contains background jobs that are executed periodically or triggered by specific events. 
These jobs handle various tasks related to electronic invoicing, such as processing outgoing invoices and purging.  

## PurgeInvoiceService  

Purging occurs every day at 6 o'clock. 
The job is responsible for removing old or unnecessary invoice data from child tables created by this adapter. 
The purge offset **PurgeAfterMonths** is configured in the settings file, is time-based and its unit is in months, and can be modified. 
Any changes require restarting the job service. If the offset is not specified in the configuration, a 
default value is used to purge data older than one month.  

::: mermaid
flowchart LR 
	A[ever day at 6 o'clock]
	B["PurgeInvoiceService"]
	C[IPurgeRepository]
	DB[(Database)]
	A --> B
	B -->|PurgeAfterMonths| C
	C --> DB
:::  

## FetchInvoiceService  

The FetchInvoiceService is responsible for retrieving invoices ready for sending to an external source.  
Fetching interval **RefreshInterval** is configured in the settings file, is expressed in minutes, and can be modified. Any changes of settings require restarting the job service.  
If the interval is not present in the configuration, a default value of 5 minutes is used.  


::: mermaid
flowchart LR 
	A((RefreshInterval))
	B["FetchInvoiceService"]
	C[IInvoiceRepository]
	DB[(Database)]
	A --> B
	B --> C
	C --> DB
:::  

## Healthchecks  
The job service includes health checks to monitor its status and ensure it is functioning correctly. 
- PurgeInvoiceService is healthy if the lastrun is between interval of 2 refreshes (1 interval added if purging takes longer then usual)
- FetchInvoiceService is healthy if the lastrun is in the last 25 hours. 