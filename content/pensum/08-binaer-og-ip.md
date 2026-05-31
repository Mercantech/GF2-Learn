---
title: "Binary Numbers & IP Addresses"
order: 8
topics: [binaer, netvaerk, ip, projekt]
kompetencemaal:
  - "Kan forklare sammenhængen mellem binære tal og decimaler"
  - "Kan konvertere binær ↔ decimal uden indbyggede funktioner"
  - "Kan bruge loops og conditionals til matematiske problemer"
  - "Kan forklare, hvordan en IP-adresse er opbygget"
  - "Kan bygge en IP-omformer der konverterer begge veje mellem binær og decimal"
timer: 3
---

# Binære tal og IP-adresser

Computere tænker i **binære tal** — kun 0 og 1. Du har sikkert allerede noget kendskab til det binære talsystem og hvordan man omregner mellem binær og decimal. Her kombinerer du den viden med **C#**, **loops** og **conditionals** — og bygger en rigtig **IP-omformer**.

## Binær og decimal — grundideen

I decimal tæller hvert ciffer en potens af 10 (enere, tiere, hundreder). I binær tæller hvert ciffer en potens af 2:

```
Binær  1010  =  1·8 + 0·4 + 1·2 + 0·1  =  10 (decimal)
         ↑    ↑    ↑    ↑
        8    4    2    1   (potenser af 2)
```

Hvert ciffer kaldes en **bit**. 8 bits = 1 **byte**. Tallet 255 i decimal er `11111111` i binær — alle 8 bits sat til 1.

| Decimal | Binær |
|---------|-------|
| 0 | 0 |
| 1 | 1 |
| 2 | 10 |
| 5 | 101 |
| 10 | 1010 |
| 75 | 01001011 |
| 170 | 10101010 |
| 187 | 10111011 |
| 255 | 11111111 |

**Potenser til en 8-bit oktet** (husk rækkefølgen):

```
128  64  32  16  8  4  2  1
```


## IP-adresser og oktetter

En **IPv4-adresse** består af **fire tal** (oktetter) adskilt af punktum — hver mellem 0 og 255:

```
192.168.1.1
 │    │ │ │
okt1 okt2...
```

Hver oktet er **8 bits**. Eksempel:

```
192 = 11000000
168 = 10101000
  1 = 00000001
  1 = 00000001
```

IP-adresser identificerer enheder på et netværk. **Private** adresser (fx `192.168.x.x`, `10.x.x.x`) bruges internt. **Public** adresser er unikke på internettet.


## Projekt: Binær IP-omformer

I skal bygge en **omformer**, der tager en IP-adresse med **4 talgrupper** — hver gruppe er et **8-cifret binært tal** (oktet) — og kan konvertere **begge veje**:

**Binær → decimal:**

```
10111011.01001011.10101010.01010101  →  187.75.170.85
```

**Decimal → binær:**

```
187.75.170.85  →  10111011.01001011.10101010.01010101
```

### Krav

- Input skal være **4 grupper** adskilt af punktum
- Hver gruppe skal være præcis **8 bits** (kun `0` og `1`) i binær tilstand
- Hver decimal-oktet skal være mellem **0 og 255**
- Programmet skal kunne konvertere **begge veje**
- Designet er op til jer (konsol, menu, Blazor — se nedenfor)

:::callout type="warning"
I må **ikke** bruge indbyggede konverteringsfunktioner som `Convert.ToString(n, 2)`, `Convert.ToInt32(s, 2)` eller tilsvarende. I skal bruge **if/else**, **loops** og den logik I har lært selv!
:::

:::callout type="info"
En mulig løsning kan bygges som en **Blazor-baseret** app med titlen *Binær Kodeomformer* — med inputfelter, validering og knap til at konvertere begge veje.
:::


## Binær → decimal med loop

Gå bit for bit fra venstre mod højre og akkumuler potenser af 2:

```csharp
static int OctetFraBinaer(string binaer)
{
    int[] potenser = { 128, 64, 32, 16, 8, 4, 2, 1 };
    int resultat = 0;

    for (int i = 0; i < 8; i++)
    {
        if (binaer[i] == '1')
            resultat += potenser[i];
    }
    return resultat;
}

Console.WriteLine(OctetFraBinaer("10111011")); // 187
Console.WriteLine(OctetFraBinaer("01001011")); // 75
```

For en **hel IP-adresse**: split strengen ved `.`, valider at der er 4 dele, og kør metoden på hver oktet.


## Decimal → binær med loop

Træk successivt de binære potenser fra tallet — samme princip som i pensum om løkker:

```csharp
static string OctetTilBinaer(int tal)
{
    int[] potenser = { 128, 64, 32, 16, 8, 4, 2, 1 };
    string binaer = "";

    for (int i = 0; i < 8; i++)
    {
        if (tal >= potenser[i])
        {
            binaer += "1";
            tal -= potenser[i];
        }
        else
        {
            binaer += "0";
        }
    }
    return binaer;
}

Console.WriteLine(OctetTilBinaer(187)); // 10111011
Console.WriteLine(OctetTilBinaer(75));  // 01001011
```


## Validering med if/else og loops

En robust omformer skal **validere input** før konvertering:

```csharp
// Split IP ved punktum
string[] dele = input.Split('.');

if (dele.Length != 4)
{
    Console.WriteLine("IP skal have præcis 4 dele!");
    return;
}

for (int i = 0; i < 4; i++)
{
    // Binær: præcis 8 tegn, kun 0 og 1
    if (dele[i].Length != 8)
    {
        Console.WriteLine("Hver oktet skal være 8 bits!");
        break;
    }

    for (int j = 0; j < 8; j++)
    {
        if (dele[i][j] != '0' && dele[i][j] != '1')
        {
            Console.WriteLine("Kun 0 og 1 er tilladt i binær oktet!");
            break;
        }
    }
}
```

For decimal input: brug `int.TryParse` og tjek `tal >= 0 && tal <= 255` for hver oktet.


## Programstruktur — skitse

Et godt udgangspunkt er en **menu-løkke** (`while`) der spørger brugeren:

1. Binær → decimal
2. Decimal → binær
3. Afslut

Inde i hver gren: læs input → valider med loops og if/else → konverter oktet for oktet → udskriv resultat.

:::git-step
commit: "feat: ip-omformer binær decimal"
branch: main
:::


## Hjælp og øvelse

Er du usikker på omregning, så øv dig med disse ressourcer:

- [Learn Binary Conversions — interaktiv guide (Simulations4All)](https://simulations4all.com/simulations/number-base-converter) — trin-for-trin mellem binær og decimal
- [Cisco Binary Game](https://learningnetwork.cisco.com/s/games/binary-game) — spil dig til hurtigere binær konvertering
- [Subnetting og binær — praktisk guide (CCNA Training)](https://ccnatraining.com/subnetting-from-zero-to-mental-math-how-i-went-from-binary-tears-to-mental-math-in-6-weeks/) — god læsning hvis subnetting og oktetter driller

:::callout type="info"
Dette emne kombinerer det du har lært: **variabler**, **loops**, **conditionals**, **metoder** og **strings** (`Split`, sammensætning) — anvendt på et rigtigt netværksproblem.
:::


## Binær i C# — bitvise operationer (bonus)

C# har operatorer til at arbejde direkte med bits — **må ikke bruges som erstatning i projektet**, men er nyttige at kende:

```csharp
int a = 5;   // 0101
int b = 3;   // 0011

Console.WriteLine(a & b);  // 1  — AND
Console.WriteLine(a | b);  // 7  — OR
Console.WriteLine(a ^ b);  // 6  — XOR
Console.WriteLine(a << 1); // 10 — venstreskift (× 2)
Console.WriteLine(a >> 1); // 2  — højreskift (÷ 2)
```

## Opsummering

- En IPv4-adresse har 4 oktetter à 8 bits (0–255 decimal)
- Binær → decimal: læg potenser sammen hvor bit er `1`
- Decimal → binær: træk potenser fra (128, 64, 32 …) med loops
- Projektet skal konvertere **begge veje** uden indbyggede konverteringsfunktioner
- Brug **nested loops** og **if/else** til validering af input


:::knowledge-check
---
q: Hvad er en **oktet** i en IPv4-adresse?
- Et tal mellem 0 og 1024
- Et **8-bit tal** (0–255 decimal) — én af fire dele i adressen
- Antallet af bits i hele IP-adressen
correct: 1
explain: En IPv4-adresse har **4 oktetter** adskilt af punktum. Hver oktet er **8 bits**, dvs. decimal **0–255** — fx `192.168.1.1`.
---
q: Hvad er decimalværdien af binær **`1010`**?
- 8
- **10**
- 12
correct: 1
explain: `1010` = 1×8 + 0×4 + 1×2 + 0×1 = **10**. Læg potenserne **128, 64, 32, 16, 8, 4, 2, 1** sammen, hvor bit er `1`.
---
q: Hvorfor må I **ikke** bruge `Convert.ToString(n, 2)` i IP-projektet?
- Funktionen findes ikke i C#
- Pensum kræver, at I bygger konvertering med **loops og if/else** selv
- Den giver forkerte resultater for IP-adresser
correct: 1
explain: Projektet tester jeres forståelse af **binær logik** — I skal bruge potens-array, loops og betingelser, som vist i kapitlet, ikke indbyggede konverteringsfunktioner.
---
q: Hvad skal hver **binære gruppe** i input have for IP-omformeren?
- Præcis 4 tegn
- **Præcis 8 bits** — kun `0` og `1`
- Et vilkårligt antal cifre
correct: 1
explain: Hver oktet er **8 bits**. Input som `10111011.01001011...` skal have **4 grupper à 8 tegn**, kun `0` og `1`, adskilt af punktum.
---
q: Hvordan konverterer `OctetFraBinaer` fra binær til decimal?
- Den dividerer tallet med 2 gentagne gange
- Den **lægger potenser sammen**, hvor bit på position `i` er `'1'`
- Den bruger `Convert.ToInt32`
correct: 1
explain: Metoden går bit for bit og lægger **128, 64, 32 … 1** til resultatet, når `binaer[i] == '1'`. Det er grundprincippet for binær → decimal.
---
q: Hvad er potens-rækkefølgen for en **8-bit oktet** (fra venstre)?
- 1, 2, 4, 8, 16, 32, 64, 128
- **128, 64, 32, 16, 8, 4, 2, 1**
- 10, 100, 1000 …
correct: 1
explain: Venstre bit er **128** (2⁷), højre er **1** (2⁰). Husk rækkefølgen — den bruges både ved binær → decimal og decimal → binær.
---
q: Hvad skal validering tjekke for **decimal input** per oktet?
- At tallet er et lige tal
- At tallet er mellem **0 og 255** (typisk med `TryParse`)
- At tallet har præcis 8 cifre
correct: 1
explain: En oktet kan maksimalt være **255** (`11111111`). Brug **`int.TryParse`** og tjek `tal >= 0 && tal <= 255` for hver del efter `Split('.')`.
:::
