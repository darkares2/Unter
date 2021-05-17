# Unter 

## Start zookeeper

	docker-compose -f zk.yml up -d

Please note the network name here, it must be the same in the following kafka yaml.

## Start kafka 

	docker-compose -f kafka.yml up -d


## Postgresql

	docker-compose -f .\postgresql.yml up -d
	docker-compose -f .\postgresql.yml exec postgres psql --dbname unter_database --username unter


## Services

### Udbyder Service

	cd ConsoleUdbyderService
	.\bin\Debug\netcoreapp3.1\ConsoleUdbyderService.exe


### Mægler Service

	cd ConsoleMaeglerService
	.\bin\Debug\netcoreapp3.1\ConsoleMaeglerService.exe

### Billet Service

	cd ConsoleBilletService
	.\bin\Debug\netcoreapp3.1\ConsoleBilletService.exe

### Faktura Service

	cd ConsoleFakturaService
	.\bin\Debug\netcoreapp3.1\ConsoleFakturaService.exe

### Kunde Adapter

	cd ConsoleKundeAdapter
	.\bin\Debug\netcoreapp3.1\ConsoleKundeAdapter.exe

Meld først en udbyder ledig, før tur udbedes.

### Udbyder Adapter

	cd ConsoleUdbyderAdapter
	.\bin\Debug\netcoreapp3.1\ConsoleUdbyderAdapter.exe

Meld ledig, send lokation. Sig ja til tur. Kør Vælg destination.



