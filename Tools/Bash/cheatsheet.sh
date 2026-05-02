#!/usr/bin/env bash
# Bash idioms cheatsheet.

# Strict mode
set -euo pipefail
IFS=$'\n\t'

# Defaults
: "${LOG_LEVEL:=info}"
: "${HOME_DIR:=$HOME/orders}"

# Required
: "${API_KEY:?must be set}"

# String slicing & manipulation
s="hello world"
echo "${s^^}"            # HELLO WORLD
echo "${s,,}"            # hello world
echo "${s:0:5}"          # hello
echo "${s/world/bash}"   # hello bash
echo "${#s}"             # 11

# Arrays
arr=(one two three)
echo "${arr[@]}"          # one two three
echo "${#arr[@]}"         # 3
arr+=(four)
for e in "${arr[@]}"; do echo "$e"; done

# Associative array
declare -A m=([a]=1 [b]=2)
for k in "${!m[@]}"; do echo "$k=${m[$k]}"; done

# Read JSON
total=$(jq -r '.total' < orders.json)
echo "total=$total"

# Heredoc with no expansion
cat <<'EOF' > /tmp/script.sh
#!/usr/bin/env bash
echo "literal $vars"
EOF

# Trap on error and exit
on_error() { echo "Failed at line $1"; }
trap 'on_error $LINENO' ERR

# Numeric arithmetic
n=5; (( n++ )); echo $n  # 6
(( n > 3 )) && echo "n > 3"

# Conditionals
[[ -f file.txt ]] && echo "exists"
[[ "$s" =~ ^hello ]] && echo "starts with hello"
[[ "$s" == *world* ]] && echo "contains world"

# Functions
log() { printf '%(%FT%TZ)T %s\n' -1 "$*" >&2; }
log "starting"
