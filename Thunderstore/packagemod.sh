#/usr/bin/env bash

dotnet build --configuration Release ../UltraYoshi/UltraYoshis.csproj

zip UltraYoshis.zip manifest.json icon.png UltraYoshis.dll custompropsbundle README.md Changes.md

mv UltraYoshis.zip ../