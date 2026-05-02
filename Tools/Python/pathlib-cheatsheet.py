"""pathlib cheatsheet — never write os.path.join again."""
from pathlib import Path

p = Path("/tmp/orders/2026-04/12.json")

# Composition
out = p.parent / "13.json"

# Read / write text
text = p.read_text(encoding="utf-8")
out.write_text(text, encoding="utf-8")

# Bytes
data = p.read_bytes()
out.write_bytes(data)

# Properties
p.name        # 12.json
p.stem        # 12
p.suffix      # .json
p.suffixes    # ['.json']
p.parent      # /tmp/orders/2026-04
p.parents[0]  # same as parent
p.anchor      # /

# Existence + creation
p.exists()
p.is_file()
p.is_dir()
p.parent.mkdir(parents=True, exist_ok=True)
p.touch(exist_ok=True)

# Globbing
list(Path(".").glob("**/*.json"))                      # recursive
list(Path(".").glob("logs/*.log"))                     # one level

# Iterating directory (lazy)
for child in Path(".").iterdir():
    print(child)

# Replace / unlink
out.replace(out.with_suffix(".bak"))
out.unlink(missing_ok=True)

# Resolve & relative_to
p.resolve()
Path("/tmp/orders").joinpath("2026-04/12.json").relative_to("/tmp/orders")  # 2026-04/12.json
