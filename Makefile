.PHONY: test
test:
	dotnet test CribblyBackend.UnitTests

.PHONY: serve
serve:
	dotnet run --project src/CribblyBackend
	
.PHONY: build
build:
	dotnet publish src/CribblyBackend -c Release