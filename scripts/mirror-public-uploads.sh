#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage:
  scripts/mirror-public-uploads.sh [page-url-or-html-file]...

Mirrors public /uploads/... files referenced by one or more pages into ClubSite/wwwroot/uploads.
If no page is supplied, it mirrors uploads referenced by https://www.volleyballclub.de/.

This intentionally copies only public website files, not the database and not secrets.
USAGE
}

if [[ ${1:-} == "-h" || ${1:-} == "--help" ]]; then
  usage
  exit 0
fi

site_origin=${CLUBSITE_PUBLIC_ORIGIN:-https://www.volleyballclub.de}
project_dir=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
webroot="$project_dir/ClubSite/wwwroot"
workdir=$(mktemp -d)
trap 'rm -rf "$workdir"' EXIT

if [[ $# -eq 0 ]]; then
  set -- "$site_origin/"
fi

: > "$workdir/uploads.txt"
for source in "$@"; do
  html="$workdir/page-$(printf '%s' "$source" | sha256sum | awk '{print $1}').html"
  if [[ -f "$source" ]]; then
    cp "$source" "$html"
  else
    curl -fLsS --max-time 30 "$source" -o "$html"
  fi
  python3 - "$html" <<'PY' >> "$workdir/uploads.txt"
import re, sys, urllib.parse
html = open(sys.argv[1], encoding='utf-8', errors='ignore').read()
for match in re.findall(r'''["']((?:https?://[^/"']+)?/uploads/[^"'#?]+)''', html):
    path = urllib.parse.urlsplit(match).path
    if path.startswith('/uploads/'):
        print(path)
PY
done

sort -u "$workdir/uploads.txt" -o "$workdir/uploads.txt"
count=$(wc -l < "$workdir/uploads.txt" | tr -d ' ')
echo "Mirroring $count upload file(s) from $site_origin"

while IFS= read -r path; do
  [[ -n "$path" ]] || continue
  dest="$webroot$path"
  mkdir -p "$(dirname "$dest")"
  if [[ -s "$dest" ]]; then
    echo "exists $path"
    continue
  fi
  curl -fLsS --max-time 60 "$site_origin$path" -o "$dest"
  echo "downloaded $path"
done < "$workdir/uploads.txt"
