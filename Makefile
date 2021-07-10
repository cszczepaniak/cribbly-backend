.PHONY: test
test:
	dotnet test CribblyBackend.UnitTests

.PHONY: serve
serve:
	dotnet run --project src/CribblyBackend

.PHONY: launch
launch:
	sh ./launch.sh