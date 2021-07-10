.PHONY: test
test:
	dotnet test CribblyBackend.UnitTests

.PHONY: build
build:
	dotnet publish src/CribblyBackend -c Release

.PHONY: start
start:
	docker-compose -f docker-compose.yml --env-file ./config/dev.env up --build -d

.PHONY: start-prod
start:
	docker-compose -f docker-compose.prod.yml up --build -d

.PHONY: stop
stop:
	docker-compose stop

.PHONY: destroy
destroy:
	docker-compose down -v