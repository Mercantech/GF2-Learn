---
title: "LDAP & Active Directory"
order: 13
topics: [ldap, active-directory, enterprise]
kompetencemaal:
  - "Kan forklare, hvad Active Directory og LDAP bruges til i en enterprise-kontekst"
  - "Kan oprette en LDAP-forbindelse fra et C#-program via NuGet-pakker"
  - "Kan hente og modellere data fra AD (brugere og grupper)"
  - "Kan søge i AD baseret på kriterier som navn, email eller afdeling"
  - "Kan håndtere fejl og manglende data ved kommunikation med eksterne systemer"
timer: 3
---

# LDAP og Active Directory

Virksomheden har et **Active Directory (AD)**, og I skal bygge en C#-platform, der hurtigt kan:

1. Hente en liste af **alle grupper**
2. Hente en liste af **alle brugere**
3. Hente en **kombineret liste** af grupper og deres medlemmer
4. **Søge** efter specifikke brugere eller grupper (navn, email, afdeling osv.)

**LDAP** er protokollen, C#-programmer bruger til at læse data fra AD.


## Active Directory i enterprise

**Active Directory** er Microsofts directory service — en central database over:

- **Brugere** — navn, email, afdeling, login
- **Grupper** — fx "IT-afdeling", "GF2-elever"
- **Computere** — maskiner i domænet
- **Rettigheder** — hvem må tilgå hvilke ressourcer

I stedet for separate login på hver app logger medarbejdere ind én gang (**Single Sign-On**) og får adgang baseret på deres AD-konto. Det er standard i danske virksomheder, kommuner og skoler.

**Directory** = struktureret opslagsdatabase. Tænk på det som en telefonbog, hvor du kan slå op på navn, afdeling eller email.


## Test-miljø — fælles AD-reader

Indtil I har jeres eget AD i luften, kan I teste med denne **fælles læsebruger**:

```csharp
private static string _server = "10.133.71.100";
private static string _username = "adReader";
private static string _password = "Merc1234!";
private static string _domain = "mags.local";
```

:::callout type="warning"
Test-credentials må **kun** bruges i undervisningsmiljøet. Commit aldrig rigtige adgangskoder til GitHub — brug `appsettings.json`, miljøvariabler eller User Secrets i rigtige projekter.
:::


## NuGet og System.DirectoryServices.Protocols

Når vi programmerer mod MS AD, bruger vi **NuGet** som package manager. Hent pakken:

[System.DirectoryServices.Protocols på NuGet](https://www.nuget.org/packages/System.DirectoryServices.Protocols/)

Dokumentation:

[LdapConnection (Microsoft Docs)](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices.protocols.ldapconnection)

```bash
dotnet add package System.DirectoryServices.Protocols
```

```csharp
using System.DirectoryServices.Protocols;
using System.Net;
```


## Opret forbindelse til AD

```csharp
private static LdapConnection Connect()
{
    var credential = new NetworkCredential($"{_username}@{_domain}", _password);
    var connection = new LdapConnection(_server)
    {
        Credential = credential,
        AuthType = AuthType.Negotiate
    };

    connection.Bind();   // Test forbindelsen — kaster fejl hvis login fejler
    return connection;
}
```

**Bind** = log ind på AD. Kaldes altid før søgninger.


## Hent alle grupper

```csharp
public static List<ADGroup> GetAllGroups()
{
    var groups = new List<ADGroup>();

    using (var connection = Connect())
    {
        // Søg i domænet mags.local efter alle objekter af typen "group"
        var searchRequest = new SearchRequest(
            "DC=mags,DC=local",
            "(objectClass=group)",
            SearchScope.Subtree,
            "cn",
            "description"
        );

        try
        {
            var response = (SearchResponse)connection.SendRequest(searchRequest);

            foreach (SearchResultEntry gruppe in response.Entries)
            {
                var nyGruppe = new ADGroup
                {
                    Name = gruppe.Attributes["cn"]?[0]?.ToString() ?? "N/A",
                    Description = gruppe.Attributes["description"]?[0]?.ToString() ?? "N/A"
                };
                groups.Add(nyGruppe);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Der skete en fejl ved hentning af grupper: {ex.Message}");
        }
    }

    return groups;
}

public class ADGroup
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}
```

**LDAP-filter:** `(objectClass=group)` finder alle grupper. `SearchScope.Subtree` søger i hele domænet.


## Hent alle brugere

```csharp
public static List<ADUser> GetAllUsers()
{
    var users = new List<ADUser>();

    using (var connection = Connect())
    {
        var searchRequest = new SearchRequest(
            "DC=mags,DC=local",
            "(objectClass=user)",
            SearchScope.Subtree,
            "cn",
            "samAccountName",
            "mail",
            "department",
            "title",
            "distinguishedName"
        );

        try
        {
            var response = (SearchResponse)connection.SendRequest(searchRequest);

            foreach (SearchResultEntry entry in response.Entries)
            {
                var user = new ADUser
                {
                    Name = entry.Attributes["cn"]?[0]?.ToString() ?? "N/A",
                    Username = entry.Attributes["samAccountName"]?[0]?.ToString() ?? "N/A",
                    Email = entry.Attributes["mail"]?[0]?.ToString() ?? "N/A",
                    Department = entry.Attributes["department"]?[0]?.ToString() ?? "N/A",
                    Title = entry.Attributes["title"]?[0]?.ToString() ?? "N/A",
                    DistinguishedName = entry.Attributes["distinguishedName"]?[0]?.ToString() ?? "N/A"
                };
                users.Add(user);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Fejl ved hentning af brugere: {ex.Message}");
        }
    }

    return users;
}

public class ADUser
{
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public string Title { get; set; } = "";
    public string DistinguishedName { get; set; } = "";
}
```

Map AD-attributter til C#-klasser — så resten af programmet arbejder med typed objekter, ikke rå LDAP-strenge. Brug `?? "N/A"` når attributten mangler.


## Grupper og medlemmer (kombineret liste)

For at hente **hvem der er medlem af hvilken gruppe**, søg grupper med attributten `member` — eller søg brugerens `memberOf`:

```csharp
// Eksempel-filter for én gruppe med medlemmer
var searchRequest = new SearchRequest(
    "DC=mags,DC=local",
    "(cn=IT-Gruppe)",
    SearchScope.Subtree,
    "cn", "member"
);

// member indeholder Distinguished Names for hver bruger i gruppen
```

Byg en `Dictionary<string, List<string>>` eller en klasse `ADGroupWithMembers` der kombinerer gruppenavn med liste af medlemmer.


## Søgning med LDAP-filter

LDAP-filtre følger en specifik syntaks — brug dem til krav #4:

```
(name=Ada*)                                    — navn starter med Ada
(mail=*@mags.local)                            — email-domæne
(department=IT)                                — afdeling
(&(objectClass=user)(department=IT))           — bruger OG afdeling IT
(|(department=IT)(department=Support))         — IT ELLER Support
```

I C# bygger du filter-strengen dynamisk:

```csharp
string filter = $"(&(objectClass=user)(mail={email}))";
var searchRequest = new SearchRequest("DC=mags,DC=local", filter, SearchScope.Subtree, "cn", "mail");
```

Start med én attribut ad gangen — udvid derefter.


## Fejlhåndtering og robusthed

Kommunikation med AD kræver **defensiv programmering**:

- Wrap søgninger i `try/catch`
- Tjek attributter med `?.` og fallback (`?? "N/A"`)
- Håndter timeout og manglende forbindelse
- Log fejl — vis brugervenlig besked i UI

```csharp
try
{
    var grupper = GetAllGroups();
    foreach (var g in grupper)
        Console.WriteLine($"{g.Name}: {g.Description}");
}
catch (Exception ex)
{
    Console.WriteLine($"AD-fejl: {ex.Message}");
}
```


:::callout type="info"
Dette emne forbereder **Projekt 4 — Enterprise**, hvor I arbejder med rigtige enterprise-scenarier mod Active Directory.
:::

## Opsummering

- AD er virksomhedens centrale bruger- og gruppedatabase
- C# forbinder via **NuGet**-pakken `System.DirectoryServices.Protocols`
- **Bind** → **Search** → map resultater til C#-klasser
- Hent grupper med `(objectClass=group)`, brugere med `(objectClass=user)`
- Brug LDAP-filtre til søgning på navn, email og afdeling
- Håndter altid fejl og manglende attributter


:::knowledge-check
---
q: Hvad bruges **LDAP** til i enterprise-sammenhæng?
- At kompilere C# til maskinkode
- **Protokollen** til at læse data fra Active Directory (brugere, grupper)
- At erstatte SQL-databaser i webapps
correct: 1
explain: **LDAP** er protokollen, C#-programmer bruger til at **kommunikere med AD** — hente brugere, grupper og søge på attributter som email og afdeling.
---
q: Hvilken NuGet-pakke bruges til AD-forbindelse i kapitlet?
- `Microsoft.EntityFrameworkCore`
- **`System.DirectoryServices.Protocols`**
- `Newtonsoft.Json`
correct: 1
explain: Pakken **`System.DirectoryServices.Protocols`** giver **`LdapConnection`**, **`SearchRequest`** og relaterede typer til AD-kommunikation.
---
q: Hvad gør **`connection.Bind()`**?
- Lukker forbindelsen til AD
- **Logger ind / tester forbindelsen** — kaster fejl hvis credentials er forkerte
- Sletter brugere i AD
correct: 1
explain: **`Bind`** autentificerer mod AD-serveren. Kald den **før** søgninger — uden succesfuld bind virker `SendRequest` ikke.
---
q: Hvilket LDAP-filter henter **alle grupper** i eksemplet?
- `(objectClass=user)`
- **`(objectClass=group)`**
- `(department=IT)`
correct: 1
explain: **`(objectClass=group)`** finder objekter af typen gruppe. **`(objectClass=user)`** bruges til brugere. Filtre er centralt i AD-søgning.
---
q: Hvorfor bruges `?? "N/A"` ved mapping af AD-attributter?
- For at kryptere følsomme data
- **Fallback når attributten mangler** — undgår null-referencer
- For at konvertere binær data til tekst
correct: 1
explain: Ikke alle AD-objekter har fx **mail** eller **department**. **`?.`** og **`?? "N/A"`** giver sikker læsning uden crash ved manglende felter.
---
q: Hvad betyder **`SearchScope.Subtree`**?
- Søg kun i roden af domænet
- **Søg i hele domænet** under det angivne udgangspunkt
- Søg kun i én bruger ad gangen
correct: 1
explain: **`Subtree`** gennemsøger hele træet under fx `DC=mags,DC=local` — ikke kun ét niveau. Det er typisk det, I vil bruge til "alle brugere".
---
q: Hvad er **Single Sign-On (SSO)** i relation til AD?
- At logge ind med forskellige passwords på hver app
- **Én AD-login giver adgang** til flere systemer baseret på konto og rettigheder
- At gemme passwords i klartekst i C#
correct: 1
explain: **AD** er central brugerdatabase — medarbejdere logger ind én gang, og apps tjekker AD for identitet og gruppemedlemskab i stedet for separate brugerlister.
:::
