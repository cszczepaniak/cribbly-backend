.PHONY: test
test:
	dotnet test CribblyBackend.UnitTests

.PHONY: serve
serve:
	dotnet run --project CribblyBackend