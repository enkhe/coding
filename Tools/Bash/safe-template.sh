#!/usr/bin/env bash
# Safe Bash script template — strict mode, traps, getopts, usage.
set -euo pipefail
IFS=$'\n\t'

readonly script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
readonly tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT

log()  { printf '[%s] %s\n' "$(date -u +%FT%TZ)" "$*" >&2; }
die()  { log "ERROR: $*"; exit 1; }

usage() {
  cat <<EOF
Usage: $(basename "$0") [-v] -i INPUT -o OUTPUT

Options:
  -i PATH    Input path (required)
  -o PATH    Output path (required)
  -v         Verbose
  -h         Help
EOF
  exit 2
}

verbose=0
while getopts ":i:o:vh" opt; do
  case "$opt" in
    i) input="$OPTARG" ;;
    o) output="$OPTARG" ;;
    v) verbose=1 ;;
    h|*) usage ;;
  esac
done
: "${input:?-i required}"
: "${output:?-o required}"

(( verbose )) && set -x

[[ -f "$input" ]] || die "input not found: $input"

mkdir -p "$(dirname "$output")"
cp "$input" "$output"
log "OK: copied $input -> $output"
