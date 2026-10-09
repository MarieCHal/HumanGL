.DEFAULT_GOAL := run

PROJECT := HumanGL/HumanGL.csproj
DOTNET_DIR := $(CURDIR)/.dotnet
DOTNET = $$(if test -x "$(DOTNET_DIR)/dotnet"; then printf '%s' "$(DOTNET_DIR)/dotnet"; else printf '%s' dotnet; fi)

.PHONY: run install sdk build clean

run: install
	"$(DOTNET)" run --project $(PROJECT)

install: sdk
	"$(DOTNET)" restore $(PROJECT)

sdk:
	@set -eu; \
	if ! "$(DOTNET)" --list-sdks 2>/dev/null | grep -q '^8\.'; then \
		command -v curl >/dev/null 2>&1 || { echo "curl est requis pour télécharger le SDK .NET 8."; exit 1; }; \
		echo "Installation du SDK .NET 8 dans $(DOTNET_DIR)..."; \
		mkdir -p "$(DOTNET_DIR)"; \
		curl --fail --silent --show-error --location https://dot.net/v1/dotnet-install.sh -o "$(DOTNET_DIR)/dotnet-install.sh"; \
		bash "$(DOTNET_DIR)/dotnet-install.sh" --channel 8.0 --install-dir "$(DOTNET_DIR)" --no-path; \
	fi

build: install
	"$(DOTNET)" build $(PROJECT)

clean: sdk
	"$(DOTNET)" clean $(PROJECT)
