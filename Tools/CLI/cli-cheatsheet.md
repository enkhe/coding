# CLI cheatsheet

## ripgrep

```
rg pattern                       # recurse
rg -i pattern                    # case-insensitive
rg --type cs pattern             # one type
rg -t md -t mdx pattern          # multiple types
rg --files | rg foo              # filenames matching
rg -l pattern                    # files only
rg -A 2 -B 1 pattern             # context
rg -F 'literal $string'          # fixed string (no regex)
rg --no-ignore pattern           # ignore .gitignore
rg -g '!**/bin/**' -g '!**/obj/**'   # globs
rg --hidden pattern              # include hidden
rg -P 'lookahead(?=here)'        # PCRE2
```

## fd

```
fd                               # all files in cwd
fd '\.cs$'                       # regex
fd -e cs -E bin                  # extension include + exclude
fd -t d node_modules             # type=directory
fd -x rm {}                      # exec on each match
fd -X tar czf out.tgz            # batch exec
```

## jq

```
jq '.users[]'                    # array iteration
jq '.users[] | .name'            # extract field
jq -r '.name'                    # raw string output
jq 'select(.active==true)'       # filter
jq 'map(.amount) | add'          # sum
jq '.[] | {id, total}'           # reshape
jq -s 'length' file.json         # slurp into one
```

## fzf

```
ls | fzf                                       # pick a file
git branch | fzf | xargs git checkout          # pick a branch
fzf --preview 'bat --color=always {}'          # preview
history | fzf                                  # search history
```

## git useful one-liners

```
git log --since=2.weeks --author=you@example.com --oneline
git log -p src/Orders/Domain/Order.cs
git diff main...HEAD --stat
git rev-parse --short HEAD
```

## curl

```
curl -fsSL url                                 # fail-fast, silent, follow redirects
curl -X POST -H 'Content-Type: application/json' -d '{"a":1}' url
curl -u user:pass url
curl --resolve host:443:1.2.3.4 https://host/  # DNS override for testing
```
