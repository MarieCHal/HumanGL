.DEFAULT_GOAL := run

PROJECT := HumanGL/HumanGL.csproj
DOTNET_DIR := $(CURDIR)/.dotnet
DOTNET = $$(if test -x "$(DOTNET_DIR)/dotnet"; then printf '%s' "$(DOTNET_DIR)/dotnet"; else printf '%s' dotnet; fi)

# Affiche les diagnostics uniquement si la commande échoue.
define quiet
@log=$$(mktemp); \
trap 'rm -f "$$log"' EXIT HUP INT TERM; \
$(1) >"$$log" 2>&1 || { status=$$?; cat "$$log" >&2; exit $$status; }
endef

.PHONY: run install sdk build clean

run: build
	@"$(DOTNET)" run --no-build --no-restore --project $(PROJECT)

install: sdk
	$(call quiet,"$(DOTNET)" restore $(PROJECT))

sdk:
	@set -eu; \
	if ! "$(DOTNET)" --list-sdks 2>/dev/null | grep -q '^8\.'; then \
		command -v curl >/dev/null 2>&1 || { echo "curl est requis pour télécharger le SDK .NET 8."; exit 1; }; \
		mkdir -p "$(DOTNET_DIR)"; \
		curl --fail --silent --show-error --location https://dot.net/v1/dotnet-install.sh -o "$(DOTNET_DIR)/dotnet-install.sh"; \
		log=$$(mktemp); \
		trap 'rm -f "$$log"' EXIT HUP INT TERM; \
		bash "$(DOTNET_DIR)/dotnet-install.sh" --channel 8.0 --install-dir "$(DOTNET_DIR)" --no-path >"$$log" 2>&1 || { status=$$?; cat "$$log" >&2; exit $$status; }; \
	fi; \
	if ! "$(DOTNET)" --list-sdks 2>/dev/null | grep -q '^8\.'; then \
		echo "Installation .NET 8 incomplète. Vérifiez l'espace disponible (df -h) et votre quota (quota -s)."; \
		exit 1; \
	fi

build: install
	$(call quiet,"$(DOTNET)" build --no-restore $(PROJECT))

clean: sdk
	$(call quiet,"$(DOTNET)" clean $(PROJECT))
