.PHONY: test
test:
	dotnet test tests/CribblyBackend.UnitTests
	dotnet test tests/CribblyBackend.Core.UnitTests

.PHONY: build
build:
	dotnet publish src/CribblyBackend -c Release

.PHONY: start
start:
	docker-compose -f docker-compose.yml --env-file ./config/dev.env up --build -d

.PHONY: stop
stop:
	docker-compose stop

.PHONY: destroy
destroy:
	docker-compose down -v
