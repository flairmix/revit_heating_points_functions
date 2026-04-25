#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION="$SCRIPT_DIR/Fill_ADSK_Parameters.sln"
PROJECT_DIR="$SCRIPT_DIR/Fill_ADSK_Parameters"
DLL_PATH="$PROJECT_DIR/bin/Release/Fill_ADSK_Parameters.dll"
MANIFEST_PATH="$PROJECT_DIR/Fill_ADSK_Parameters_2025.addin"

echo "Building Fill_ADSK_Parameters for Revit 2025..."
dotnet build "$SOLUTION" -c Release

echo
echo "Build completed."
echo "DLL: $DLL_PATH"
echo "Manifest: $MANIFEST_PATH"
