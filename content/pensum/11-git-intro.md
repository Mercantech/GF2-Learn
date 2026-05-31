---
title: "Git — Introduction"
order: 11
topics: [git]
timer: 2
---

# Git intro

**Git** er et versionsstyringssystem — det gemmer historik over alle ændringer i din kode, så du kan gå tilbage, sammenligne og samarbejde uden at overskrive hinandens arbejde.

## Hvad er versionsstyring?

Uden versionsstyring ender filnavne som `projekt_final_v2_RIGTIG.psd`. Git løser det ved at gemme **snapshots** (commits) af hele projektet med tidspunkt og besked.

**Fordele:**

- Se præcis hvad der blev ændret og hvornår
- Gå tilbage til en tidligere version uden at miste nyere arbejde
- Samarbejd i team uden at sende zip-filer frem og tilbage
- Arbejd på features parallelt via branches

Git kører **lokalt** på din maskine. **GitHub** er en cloud-tjeneste, der hoster Git-repositories og tilføjer pull requests, issues og samarbejde.


## Grundlæggende Git-kommandoer

De fire kommandoer du bruger dagligt:

```bash
git init          # opret nyt repository i mappen
git add .         # stage alle ændrede filer
git commit -m "besked"  # gem snapshot med besked
git push          # upload commits til GitHub
```

**Workflow:**

1. Lav ændringer i koden
2. `git add` — vælg hvad der skal med i næste commit
3. `git commit` — gem snapshot lokalt
4. `git push` — send til GitHub

Med **GitHub Desktop** kan du gøre det samme grafisk: ændringer vises i venstre panel, skriv commit-besked, klik **Commit**, derefter **Push**.


:::callout type="info"
Commit ofte med **tydelige beskeder** — det gør det nemt at finde tilbage i historikken.
:::

## Commit-beskeder og god praksis

En commit-besked forklarer *hvad* og *hvorfor* — ikke bare *at* der er ændret:

| Dårlig | Bedre |
|--------|-------|
| `fix` | `fix: ret division by zero i BeregnGennemsnit` |
| `asdf` | `feat: tilfoej validering af brugerinput` |
| `update` | `docs: opdater readme med installationsvejledning` |

**Tips:**

- Commit efter hver logisk ændring — ikke én kæmpe commit i ugevis
- Test at koden kører *før* du committer
- Brug dansk eller engelsk konsekvent — bland ikke i samme repo


:::git-step
commit: "docs: tilfoej readme"
branch: main
Push dit projekt til GitHub efter hver logisk ændring.
:::

## Repository og .git-mappen

Når du kører `git init`, oprettes en skjult `.git`-mappe. Den indeholder hele historikken — **slet aldrig denne mappe**.

Et **repository** (repo) er din projektmappe + `.git`. Du kloner et repo fra GitHub for at få en kopi:

```
git clone https://github.com/Mercantech/GF2-CSharp
```

`.gitignore` er en fil, der fortæller Git hvilke filer den skal ignorere — fx `bin/`, `obj/`, `.vs/` (build-output og IDE-filer). Visual Studio opretter typisk `.gitignore` automatisk.

Du bruger Git sammen med **GitHub Desktop** og dit repo [GF2-CSharp](https://github.com/Mercantech/GF2-CSharp).


## Status, diff og historik

Disse kommandoer hjælper dig med at forstå, hvad der er sket:

```bash
git status        # hvilke filer er ændret/staged?
git diff          # præcis hvad er ændret i filerne?
git log           # liste over commits
git log --oneline # kompakt historik
```

I GitHub Desktop vises ændringer visuelt — grøn for tilføjet, rød for fjernet. Brug `git status` før du committer, så du ved præcis, hvad der kommer med.


:::knowledge-check
---
q: Hvad er formålet med **Git**?
- At kompilere C#-kode automatisk
- At gemme **historik over ændringer** i kode og samarbejde uden at overskrive hinanden
- At hoste websider på internettet
correct: 1
explain: **Git** er et versionsstyringssystem — det gemmer **snapshots (commits)** af dit projekt, så du kan gå tilbage, sammenligne og arbejde sammen i team.
---
q: Hvad er den korrekte rækkefølge i den daglige Git-workflow?
- push → commit → add → ændringer
- **ændringer → add → commit → push**
- commit → ændringer → push → add
correct: 1
explain: Først laver du **ændringer**, så **`git add`** (stage), **`git commit`** (gem lokalt), og til sidst **`git push`** (upload til GitHub).
---
q: Hvad gør **`git add .`**?
- Uploader commits til GitHub
- **Stager alle ændrede filer** til næste commit
- Sletter filer, der ikke er tracked
correct: 1
explain: **`git add`** vælger, hvad der skal med i næste commit. `.` betyder alle ændringer i mappen. Uden `add` kommer ændringerne ikke med i commit.
---
q: Hvad indeholder den skjulte **`.git`-mappe**?
- Kun dine kildefiler
- **Hele repository-historikken** — slet aldrig denne mappe
- Visual Studio-indstillinger
correct: 1
explain: **`.git`** oprettes ved `git init` og gemmer al commit-historik. Sletter du den, mister du versionsstyringen for projektet.
---
q: Hvad er formålet med en **`.gitignore`-fil**?
- At gemme adgangskoder sikkert
- At fortælle Git, **hvilke filer den skal ignorere** — fx `bin/` og `obj/`
- At erstatte commit-beskeder
correct: 1
explain: **`.gitignore`** holder build-output og IDE-filer ude af repoet — så I ikke committer unødvendige eller store filer.
---
q: Hvilken commit-besked følger **god praksis**?
- `fix`
- **`feat: tilfoej validering af brugerinput`**
- `asdf`
correct: 1
explain: En god besked forklarer **hvad** der ændredes og **hvorfor** — fx `feat:`, `fix:`, `docs:`. Vage beskeder som `fix` eller `update` gør historikken ubrugelig.
---
q: Hvad er forskellen på **Git** og **GitHub**?
- De er det samme produkt
- **Git** kører lokalt; **GitHub** er en cloud-tjeneste der hoster repos og tilføjer PR/issues
- GitHub kompilerer koden; Git kører den
correct: 1
explain: **Git** er værktøjet på din maskine. **GitHub** er en platform, hvor du **pusher** commits, samarbejder og laver pull requests.
:::
