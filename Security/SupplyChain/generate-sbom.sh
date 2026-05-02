#!/usr/bin/env bash
# Generate CycloneDX SBOMs for a .NET solution and an npm project.
set -euo pipefail

OUT="${1:-./artifacts/sbom}"
mkdir -p "$OUT"

# .NET — solution-wide SBOM
if compgen -G "*.sln" > /dev/null; then
  dotnet tool install --global CycloneDX 2>/dev/null || true
  dotnet CycloneDX *.sln --output "$OUT" --json
fi

# npm — single project
if [[ -f package.json ]]; then
  npx --yes @cyclonedx/cyclonedx-npm --output-format json --output-file "$OUT/npm-bom.json"
fi

# OS packages of the runtime image (in CI for the built image)
if command -v syft >/dev/null; then
  syft "${IMAGE:-orders:dev}" -o cyclonedx-json="$OUT/image-bom.json"
fi

echo "SBOMs written to $OUT"
