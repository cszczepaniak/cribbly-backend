.PHONY: test
test:
	dotnet test tests/CribblyBackend.UnitTests
	dotnet test tests/CribblyBackend.Core.UnitTests

.PHONY: serve
serve:
	dotnet run --project src/CribblyBackend

.PHONY: build
build:
	dotnet build src/CribblyBackend