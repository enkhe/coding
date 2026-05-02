# Linux Cheatsheet

## File ops

```bash
ls -lah                           # human sizes, hidden, list
cp -a src dst                     # archive (preserve perms/links)
mv -i a b                         # interactive (prompt overwrite)
rm -rf -- "$path"                 # destructive — `--` to stop flag parsing
mkdir -p a/b/c                    # parents
ln -s target link                 # symlink
stat file                         # full metadata
file file                         # mime/type
```

## Search

```bash
find . -name '*.cs' -mtime -1     # modified in last 24h
find . -size +100M                # big files
rg pattern                        # ripgrep (preferred)
grep -RIn pattern .               # recursive, line numbers, skip binary
```

## Text manipulation

```bash
awk -F, '{print $1, $3}' file.csv          # cols 1 & 3
sed -i.bak 's/foo/bar/g' file              # in-place + backup
cut -d',' -f1,3 file.csv
sort -u | uniq -c | sort -rn               # frequency
tr 'A-Z' 'a-z' < in > out                  # case fold
column -ts$'\t' file.tsv                   # pretty TSV
```

## Network

```bash
ss -tlnp                          # listening sockets + processes
ss -tnp                           # established TCP
curl -fsSL url                    # fail-silent + follow
curl -w '%{http_code}\n' -o /dev/null -s url   # only status
ping -c 3 host
dig host                          # or `getent hosts host`
nc -zv host port                  # check port
```

## Processes & system

```bash
ps -ef | grep dotnet
pgrep -af dotnet
top                                # or htop, btop
free -h                           # memory
df -h                             # disk space
du -sh *                          # per-dir size
nproc                             # CPU count
uptime
```

## Permissions

```bash
chmod 644 file                    # rw-r--r--
chmod 755 dir                     # rwxr-xr-x
chmod -R u+rw,go-w dir            # recursive symbolic
chown user:group file
```

## Archives

```bash
tar czf out.tgz dir               # gzip
tar xzf in.tgz                    # extract gzip
tar cJf out.txz dir               # xz (better compression, slower)
zstd -19 file                     # best with .zst
```

## systemd (services)

```bash
systemctl status orders-api
systemctl restart orders-api
journalctl -u orders-api -f       # follow logs
journalctl --since "2h ago"
```

## Cron

```bash
crontab -e
# m h dom mon dow
0 3 * * * /usr/local/bin/backup.sh
```

## SSH

```bash
ssh -i ~/.ssh/id_ed25519 user@host
scp file user@host:/dst/
ssh -L 5432:localhost:5432 user@host    # port forward
ssh -J jump@bastion user@target         # via jump host
```
