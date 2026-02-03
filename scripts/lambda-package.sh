#!/usr/bin/env bash
set -euo pipefail

dotnet lambda package \
  --project-location JobScraper \
  --configuration Release \
  --framework net10.0 \
  --output-package bin/Release/net10.0/JobScraper.zip
