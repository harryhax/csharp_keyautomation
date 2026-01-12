APP_NAME := csharp_gta_keyautomation
PROJECT := $(APP_NAME).csproj
CONFIG := Release
FRAMEWORK := net10.0
DIST_DIR := dist

RIDS := win-x64 osx-x64 osx-arm64

VERSION := $(shell dotnet msbuild $(PROJECT) -getProperty:Version -nologo)

PUBLISH_FLAGS := \
	--self-contained true \
	/p:PublishSingleFile=true \
	/p:IncludeNativeLibrariesForSelfExtract=true \
	/p:DebugType=None \
	/p:DebugSymbols=false \
	/p:PublishTrimmed=true \
	/p:TrimMode=partial

.PHONY: all clean $(RIDS)

all: clean $(RIDS)

clean:
	rm -rf $(DIST_DIR)

$(RIDS):
	dotnet publish $(PROJECT) \
		-c $(CONFIG) \
		-r $@ \
		-f $(FRAMEWORK) \
		$(PUBLISH_FLAGS) \
		-o $(DIST_DIR)/$@
	cd $(DIST_DIR) && zip -r $(APP_NAME)-$(VERSION)-$@.zip $@

