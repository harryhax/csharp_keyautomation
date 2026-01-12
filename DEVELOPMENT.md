# Release & Versioning Workflow

This repository uses a **tag‑driven, automated release pipeline** built around the following principles:

- The `.csproj` file is the **single source of truth** for version numbers
- **Git tags** explicitly mark release points
- **GitHub Actions** builds and publishes binaries automatically
- Releases are **pre‑releases** for `0.x.y` versions and **stable** for `1.x.y+`
- No binaries are committed to Git

This document explains **exactly how the setup works** and **how to perform a release correctly**.

---

## High‑Level Architecture

```
Developer
  │
  ├─ edits code
  ├─ updates <Version> in csproj
  ├─ commits changes
  ├─ creates & pushes a tag
  ▼
GitHub Actions
  ├─ validates tag == csproj version
  ├─ runs Makefile
  ├─ builds OS‑specific binaries
  ├─ zips artifacts
  ├─ creates GitHub Release
  └─ uploads assets
```

Releases are **explicit and intentional**. Nothing is published automatically on normal `git push`.

---

## Source of Truth: Versioning

### Where the version lives

The project version is defined in the `.csproj` file:

```xml
<PropertyGroup>
  <Version>0.0.1</Version>
  <AssemblyVersion>0.0.1.0</AssemblyVersion>
  <FileVersion>0.0.1.0</FileVersion>
</PropertyGroup>
```

This version:
- Determines the Git tag name
- Determines release filenames
- Determines whether the release is a pre‑release

### Version rules

| Version pattern | Release type |
|-----------------|--------------|
| `0.x.y`         | Pre‑release (alpha/beta) |
| `1.x.y`+        | Stable release |

---

## Git Tags and Releases

### Why tags are required

GitHub Releases are triggered **only** by pushing tags that start with `v`:

```yaml
on:
  push:
    tags:
      - "v*"
```

This means:
- Normal `git push` does **not** create a release
- Releases only happen when you **explicitly tag a commit**

### Correct tag format

```
v<Version>
```

Examples:
- `v0.0.1`
- `v0.1.0`
- `v1.0.0`

The workflow **fails** if the tag does not exactly match the version in the `.csproj`.

---

## Makefile Responsibilities

The Makefile is responsible for:

- Reading the version from the `.csproj`
- Building binaries for each RID
- Producing versioned ZIP files

### Supported targets

- `make` – builds all distributions

This command:
1. Reads the version from the `.csproj`
2. Creates the tag `v<Version>`
3. Pushes the tag to GitHub

It **does not**:
- Increment versions
- Modify files
- Commit changes

You must update and commit the version **before** running `make tag`.

---

## GitHub Actions Workflow

The workflow file lives at:

```
.github/workflows/release.yml
```

### What the workflow does

When a tag is pushed:

1. Checks out the tagged commit
2. Reads the version from the `.csproj`
3. Verifies the tag matches the version
4. Runs `make`
5. Builds OS‑specific binaries
6. Creates a GitHub Release
7. Uploads ZIP files as release assets
8. Marks the release as pre‑release or stable automatically

### Pre‑release logic

```yaml
prerelease: ${{ startsWith(version, '0.') }}
```

This ensures:
- All `0.x` versions are marked unstable
- Stable releases begin at `1.0.0`

---

## How to Do a Release (Canonical Steps)

This is the **only correct release procedure**.

### 1. Make code changes

Commit as many development commits as needed.

---

### 2. Update the version

Edit the `.csproj`:

```xml
<Version>0.0.2</Version>
```

---

### 3. Commit the version bump

```bash
git add csharp_gta_keyautomation.csproj
git commit -m "Bump version to 0.0.2"
```

---

### 4. Create and push the tag

```bash
git tag vX.Y.Z
git push origin vX.Y.Z
```

This triggers the entire release pipeline on GitHub

---

### 5. Verify the release

- GitHub → Actions → Release → success
- GitHub → Releases → `v0.0.2`
- Confirm assets are present

---

## What Not To Do

- Do not commit binaries
- Do not create releases manually in the GitHub UI
- Do not reuse tags
- Do not push incorrect tags
- Do not rely on `git push` alone

---

## Mental Model (Keep This)

> Commits move code.
> Tags create releases.
> CI enforces correctness.

If this rule is followed, the release process is deterministic, auditable, and safe.

