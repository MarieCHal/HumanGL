.DEFAULT_GOAL := run

PROJECT := HumanGL/HumanGL.csproj

.PHONY: run install build clean

run:
	dotnet run --project $(PROJECT)

install:
	@command -v dotnet >/dev/null 2>&1 || { echo "Installez le SDK .NET 8, puis relancez make install."; exit 1; }
	@dotnet --list-sdks | grep -q '^8\.' || { echo "Le SDK .NET 8 est requis. Installez-le, puis relancez make install."; exit 1; }
	dotnet restore $(PROJECT)

build:
	dotnet build $(PROJECT)

clean:
	dotnet clean $(PROJECT)
