#!/usr/bin/env bash
# Parallel processing — xargs and GNU parallel patterns.
set -euo pipefail

# 1) xargs -P (limit concurrency, no extra dep)
ls *.png | xargs -P 8 -n 1 -I{} sh -c 'optipng "{}" >/dev/null'

# 2) Process N items in parallel via while-read + background + wait
sem=0
max=8
while IFS= read -r url; do
  curl -fsSL -o "out/$(basename "$url")" "$url" &
  sem=$((sem + 1))
  if (( sem >= max )); then
    wait -n   # wait for any one to finish
    sem=$((sem - 1))
  fi
done < urls.txt
wait

# 3) GNU parallel — best for complex commands
parallel -j 8 --bar 'identify -format "%w %h %f\n" {}' ::: *.jpg
